using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

namespace libTravian
{
    public class BalancerQueue : IQueue
    {
//
        #region IQueue 成员

        public Travian UpCall { get; set; }

        [Json]
        public int VillageID { get; set; }

        public bool MarkDeleted { get; set; }

        [Json]
        public bool Paused { get; set; }

        public string Title
        {
            get
            {
                if (BalancerGroup.ID == 0)
                {
                    return "Default Group";
                }
                else
                {
                    return "Group " + BalancerGroup.ID.ToString();
                }
            }
        }

        public string Status
        {
            get
            {
                switch (type)
                {
                    case villagetype.full:
                        return "爆仓";
                    case villagetype.giver:
                        return "空闲";
                    case villagetype.marketnotavailable:
                        return "无空闲商人";
                    case villagetype.needer:
                        return "需求资源";
                    case villagetype.waiting:
                        return "Waiting";
                    default:
                        return "等待处理";
                }
            }
        }

        public int CountDown
        {
            get
            {
                if (--delay < 0)
                {
                    delay = 5;
                    return 0;
                }
                else
                {
                    return delay;
                }
            }
        }
        int delay = 4;

        public void Action()
        {
            UpdateType();
            execute();
            UpdateType();
        }

        public int QueueGUID { get { return 21; } }

        #endregion

        #region fields

        private int NextExec;
        private int retrycount;

        //资源平衡组
        [Json]
        public TBalancerGroup BalancerGroup
        {
            get
            {
                if (group == null)
                {
                    group = TBalancerGroup.GetDefaultTBalancerGroup();
                }
                return group;
            }
            set
            {
                group = value;
            }
        }

        private TBalancerGroup group;

        private TVillage village;//当前村庄
        private states state;//状态
        private villagetype type;//类型
        private TResAmount needRes = new TResAmount();//需要的资源

        private List<TSVillage> groupVillages;//本组的村庄
        private DateTime lastUpdateTime;

        #endregion


        #region enums

        public enum states
        {
            notinitlized = 0,
            initlized = 1,
            executing = 2,
            done = 4
        }

        public enum villagetype
        {
            needer = 1,
            giver = 2,
            marketnotavailable = 3,
            full = 4,
            unknown = 5,
            waiting = 6
        }

        public enum IgnorMarketType
        {
            notignore = 0,
            ignore = 1,
        }

        #endregion

        #region methods
        public BalancerQueue()
        {
            groupVillages = new List<TSVillage>();
        }

        private void UpdateType()
        {

            village = UpCall.TD.Villages[VillageID];
            if (village == null) return;
            if (village.Name.Contains("084"))
            {
                int debug = 0;
                debug++;
            }
            if (DateTime.Now.Subtract(lastUpdateTime).TotalSeconds > 600)
            {
                UpdateGroupVillages();
                lastUpdateTime = DateTime.Now.AddSeconds(1);
            }
            /*
            if (village.isMarketInitialized() == false)
            {
                debug("market not initialized, wait 10 second");
            }
             */
            if (village.Queue == null)
            {
                type = UpdateMarketState();

            }
            else
            {
                ///检查建筑队列
                TResAmount source = CaculateCropAmount();
                if (source.isZero())
                {
                    source = CaculateBuildingAmount(IgnorMarketType.ignore, BalancerGroup.IgnoreMarketTime);
                }
                //检查party
                if (source.isZero())
                {
                    source = CaculatePartyResource(IgnorMarketType.ignore, BalancerGroup.IgnoreMarketTime);
                }
                //检查研究
                if (source.isZero())
                {
                    source = CaculateResearchAmount(IgnorMarketType.ignore, BalancerGroup.IgnoreMarketTime);
                }
                //检查造兵
                if (source.isZero())
                {
                    source = CaculateProduceTroop(IgnorMarketType.ignore, BalancerGroup.IgnoreMarketTime);
                }
                if (source.isZero())
                {
                    type = UpdateMarketState();
                }
                else
                {
                    TResAmount totalComRes = new TResAmount();
                    foreach (TMInfo transfer in village.Market.MarketInfo)
                    {
                        if (transfer.MType == TMType.OtherCome)
                        {
                            if (BalancerGroup.IgnoreMarketTime < 0)
                            {
                                totalComRes += transfer.CarryAmount;
                            }
                            else
                            {
                                if (transfer.FinishTime.Subtract(DateTime.Now).TotalSeconds <= BalancerGroup.IgnoreMarketTime)
                                {
                                    totalComRes += transfer.CarryAmount;
                                }
                            }
                        }
                    }
                    needRes = source - totalComRes;
                    needRes.NoNegative();
                    if (needRes.isZero())
                    {
                        if (totalComRes.isZero())
                        {
                            type = UpdateMarketState();
                        }
                        else
                        {
                            type = villagetype.waiting;
                        }
                    }
                    else
                    {
                        UpCall.DebugLog("Auto Balancer : " + VillageShort(village) + " need res " + source.ToString(), DebugLevel.E);
                        type = villagetype.needer;
                    }
                }
            }
            state = states.initlized;
        }

        protected bool notInBuilding(TInBuilding inbuilding)
        {
            if (inbuilding == null) return true;
            return inbuilding.end(-BalancerGroup.ReadyTime);
        }

        protected TResAmount CaculateCropAmount()
        {
            TResAmount amount = new TResAmount();

            if (village.Resource[3].Produce < 0 || village.Resource[3].CurrAmount <= 0)
            {
                int maxcrop = village.Resource[3].Capacity;
                amount.Resources[3] = maxcrop / 2;
            }
            amount -= GetVillageRes(village,BalancerGroup.IgnoreMarket,BalancerGroup.IgnoreMarketTime);
            amount.NoNegative();
            return amount;
        }

        //检测建筑序列所需的资源
        protected TResAmount CaculateBuildingAmount(IgnorMarketType ignoreMarket, int ignoreTime)
        {
            TResAmount resource = new TResAmount();
            if (UpCall.TD.isRomans)
            {   //罗马双造的处理
                //分批处理即可
                //外城处理
                if (notInBuilding(village.InBuilding[0]))
                {
                    resource = FindBuildingRes(resource, RomaOutRule.GetRule());
                }
                if (resource.isZero())
                {
                    //内城处理
                    if (notInBuilding(village.InBuilding[1]))
                    {
                        resource = FindBuildingRes(resource, RomaInnerRule.GetRule());
                    }
                }
            }
            else
            {//单造的处理
                if (notInBuilding(village.InBuilding[0]) && notInBuilding(village.InBuilding[1]))
                {
                    resource = FindBuildingRes(resource, NotRomaRule.GetRule());
                }
            }
            resource -= GetVillageRes(village, ignoreMarket, BalancerGroup.IgnoreMarketTime);
            resource.NoNegative();
            //if (resource.isZero() == false)
            //{
            //    UpCall.DebugLog(VillageShort(village) + " Building Need " + resource, DebugLevel.E);
            //}
            return resource;
        }

        private TResAmount FindBuildingRes(TResAmount resource, TGidRules GidRules)
        {
            foreach (var task in village.Queue)
            {
                if (task.GetType().Name == "BuildingQueue")
                {
                    BuildingQueue Q = task as BuildingQueue;
                    //UpCall.DebugLog(VillageShort(village) + " BuildingQ " + Q.Gid, DebugLevel.E);
                    if (GidRules.allow(Q.Gid))
                    {
                        TResAmount res = Buildings.Cost(village.Buildings[Q.Bid].Gid, village.Buildings[Q.Bid].Level + 1);
                        //建筑所需资源没有超过仓库上限
                        if ((larger(res, GetVillageCapacity(village))) == false)
                        {
                            resource += res;
                            break;
                        }
                    }
                }
                else if (task.GetType().Name == "AIQueue")
                {
                    AIQueue Q = task as AIQueue;
                    if (GidRules.allow(Q.Gid))
                    {
                        //UpCall.DebugLog(VillageShort(village) + " AIQueue " + Q.Gid, DebugLevel.E);
                        TResAmount res = Buildings.Cost(village.Buildings[Q.Bid].Gid, village.Buildings[Q.Bid].Level + 1);
                        //建筑所需资源没有超过仓库上限
                        if ((larger(res, GetVillageCapacity(village))) == false)
                        {
                            resource += res;
                            break;
                        }
                    }
                }
            }
            return resource;
        }

        //检测研究序列所需的资源
        protected TResAmount CaculateResearchAmount(IgnorMarketType ignoreMarket, int ignorTime)
        {
            TResAmount result = new TResAmount();
            //UPAttack
            if (notInBuilding(village.InBuilding[3]))
            {
                foreach (var task in village.Queue)
                {
                    if (task.GetType().Name == "ResearchQueue")
                    {
                        ResearchQueue Q = task as ResearchQueue;
                        if (Q.ResearchType == ResearchQueue.TResearchType.UpAttack)
                        {
                            TResAmount res = Buildings.UpCost[(UpCall.TD.Tribe - 1) * 10 + Q.Aid][village.Upgrades[Q.Aid].AttackLevel];
                            result = res - GetVillageRes(village, ignoreMarket, ignorTime);
                            break;
                        }
                    }
                }
            }

            //UPDefense
            if (result.isZero())
            {
                if (notInBuilding(village.InBuilding[4]))
                {
                    foreach (var task in village.Queue)
                    {
                        if (task.GetType().Name == "ResearchQueue")
                        {
                            ResearchQueue Q = task as ResearchQueue;
                            if (Q.ResearchType == ResearchQueue.TResearchType.UpDefence)
                            {
                                TResAmount res = Buildings.UpCost[(UpCall.TD.Tribe - 1) * 10 + Q.Aid][village.Upgrades[Q.Aid].DefenceLevel];
                                result = res - GetVillageRes(village, ignoreMarket, ignorTime);
                                break;
                            }
                        }
                    }
                }
                //Research
                if (result.isZero())
                {
                    if (notInBuilding(village.InBuilding[5]))
                    {
                        foreach (var task in village.Queue)
                        {
                            if (task.GetType().Name == "ResearchQueue")
                            {
                                ResearchQueue Q = task as ResearchQueue;
                                if (Q.ResearchType == ResearchQueue.TResearchType.Research)
                                {
                                    TResAmount res = Buildings.ResearchCost[(UpCall.TD.Tribe - 1) * 10 + Q.Aid];
                                    result = res - GetVillageRes(village, ignoreMarket, ignorTime);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            result.NoNegative();
            if (result.isZero() == false)
            {
                //UpCall.DebugLog(VillageShort(village) + " Research Need " + result, DebugLevel.E);
            }
            return result;
        }

        //检测Party所需的资源
        protected TResAmount CaculatePartyResource(IgnorMarketType ignoreMarket, int ignoreTime)
        {
            TResAmount result = new TResAmount();
            if (notInBuilding(village.InBuilding[6]))
            {
                foreach (var task in village.Queue)
                {
                    if (task.GetType().Name == "PartyQueue")
                    {
                        PartyQueue Q = task as PartyQueue;
                        //UpCall.DebugLog(VillageShort(village) + " PartyQ " + Q.PartyType, DebugLevel.E);
                        TResAmount res = Buildings.PartyCos[(int)Q.PartyType - 1];
                        if ((larger(res, GetVillageCapacity(village))) == false)
                        {
                            result = res - GetVillageRes(village, ignoreMarket, ignoreTime);
                            break;
                        }
                    }
                }
            }
            result.NoNegative();
            if (result.isZero() == false)
            {
                //UpCall.DebugLog(VillageShort(village) + " Party Need " + result, DebugLevel.E);
            }
            return result;
        }

        protected TResAmount CaculateProduceTroop(IgnorMarketType ignoreMarket, int ignoreTime)
        {
            TResAmount result = new TResAmount();
            foreach (var task in village.Queue)
            {
                if (task.GetType().Name == "ProduceTroopQueue")
                {
                    ProduceTroopQueue Q = task as ProduceTroopQueue;

                    var CV = UpCall.TD.Villages[VillageID];
                    int Aid = Q.Aid;
                    int Amount = Q.Amount;
                    int key = (UpCall.TD.Tribe - 1) * 10 + Aid;
                    int timecost;
                    TResAmount TroopRes;
                    if (Aid == 9 || Aid == 10)
                        TroopRes = Buildings.TroopCost[key] * Amount;
                    else
                        TroopRes = Buildings.TroopCost[key] * Amount * (Q.GRt ? 3 : 1);

                    result = TroopRes - GetVillageRes(village, ignoreMarket, ignoreTime);
                    break;
                }
            }
            result.NoNegative();
            if (result.isZero() == false)
            {
                //UpCall.DebugLog(VillageShort(village) + " Produce Troop Need " + result, DebugLevel.E);
            }
            return result;
        }

        //返回目标村庄的资源上限
        protected TResAmount GetVillageCapacity(TVillage village)
        {
            return village.ResourceCapacity;
        }

        //返回村庄可用资源
        //用于计算是否可以向外运输
        protected TResAmount GetVillageAvailableRes(TVillage village)
        {
            TResAmount res = new TResAmount(village.ResourceCurrAmount);
            if (village.Market.LowerLimit != null)
            {
                res -= village.Market.LowerLimit;
            }
            res.NoNegative();
            return res;
        }

        //返回村庄当前资源(包含市场)
        //用于计算是否接受资源
        protected TResAmount GetVillageRes(TVillage village, IgnorMarketType ignoreMarket, int ignorTime)
        {
            TResAmount res = new TResAmount(village.ResourceCurrAmount);
            //ignoreMarket和ignoreTime的处理
            if (ignoreMarket == IgnorMarketType.notignore)
            {
                foreach (TMInfo transfer in village.Market.MarketInfo)
                {
                    if (transfer.MType == TMType.OtherCome)
                    {
                        if (ignorTime < 0)
                        {
                            res += transfer.CarryAmount;
                        }
                        else
                        {
                            if (transfer.FinishTime.Subtract(DateTime.Now).TotalSeconds <= ignorTime)
                            {
                                res += transfer.CarryAmount;
                            }
                        }
                    }
                }
            }
            return res;
        }

        //检查市场
        private villagetype UpdateMarketState()
        {
            if (village.Market.ActiveMerchant <= 0)
            {
                return villagetype.marketnotavailable;
            }
            else
            {
                TResAmount res = GetVillageRes(village, BalancerGroup.IgnoreMarket, BalancerGroup.IgnoreMarketTime);
                for (int i = 0; i < village.Resource.Length; i++)
                {
                    if (res.Resources[i] >= village.Resource[i].Capacity)
                    {
                        return villagetype.full;
                    }
                    else if (village.Market.UpperLimit != null)
                    {
                        if (res.Resources[i] >= village.Market.UpperLimit.Resources[i])
                        {
                            return villagetype.full;
                        }
                    }
                }
                return villagetype.giver;
            }
        }

        private void execute()
        {
            if (type == villagetype.needer)
            {
                NeedPull();
            }
            else if (type == villagetype.full)
            {
                FullPush();
            }
        }

        //PULL模式，自动寻找最近的资源点pull资源
        private void NeedPull()
        {
            TResAmount res = new TResAmount(needRes);
            foreach (var tsv in groupVillages)
            {
                res.NoNegative();
                //tsv.queue.UpdateState();
                TVillage fromVillage = UpCall.TD.Villages[tsv.VillageID];
                var queue = fromVillage.getBalancer();
                if (queue.type == villagetype.giver
                    || queue.type == villagetype.full)
                {

                    //TResAmount r = fromVillage.ResourceCurrAmount;
                    TResAmount r = GetVillageAvailableRes(fromVillage);
                    int marketCarry = fromVillage.Market.ActiveMerchant * fromVillage.Market.SingleCarry;
                    //资源和商人都充足
                    if (res.TotalAmount < marketCarry && smaller(res, r) && res.isZero() == false)
                    {
                        if (DoTranfer(fromVillage, this.village, res))
                        {
                            res -= res;
                            needRes -= res;
                        }
                    }
                    else
                    {
                        int[] sendRes = new int[r.Resources.Length];
                        int total = 0;
                        for (int i = 0; i < sendRes.Length; i++)
                        {
                            int thisTypeCount = needRes.Resources[i];
                            if (thisTypeCount > marketCarry)
                            {
                                thisTypeCount = (marketCarry > r.Resources[i]) ? r.Resources[i] : marketCarry;
                            }
                            else
                            {
                                thisTypeCount = (r.Resources[i] > thisTypeCount) ? thisTypeCount : r.Resources[i];
                            }
                            if (total + thisTypeCount > BalancerGroup.MaxSendResource)
                            {
                                thisTypeCount = BalancerGroup.MaxSendResource - total;
                            }
                            if (thisTypeCount < 0)
                            {
                                thisTypeCount = 0;
                            }
                            total += thisTypeCount;
                            sendRes[i] = thisTypeCount;
                            marketCarry -= sendRes[i];
                            if (total >= BalancerGroup.MaxSendResource)
                            {
                                break;
                            }
                        }
                        TResAmount r2 = new TResAmount(sendRes);
                        if (r2.TotalAmount >= BalancerGroup.MinSendResource)
                        {
                            if (DoTranfer(fromVillage, this.village, r2))
                            {
                                res -= r2;
                                needRes -= r2;
                            }
                        }
                    }
                }

                if (res.isZero())
                {
                    break;
                }
            }
        }

        //push模式，爆仓的村庄自动寻找资源最少的村子进行push
        private void FullPush()
        {
            if (village.Market.ActiveMerchant > 0)
            {
                int outResType = -1;
                TResAmount res = GetVillageRes(village, BalancerGroup.IgnoreMarket, BalancerGroup.IgnoreMarketTime);
                for (int i = 0; i < village.Resource.Length; i++)
                {
                    if (village.Market.UpperLimit != null)
                    {
                        if (res.Resources[i] >= village.Market.UpperLimit.Resources[i])
                        {
                            outResType = i;
                            break;
                        }
                    }
                    if (res.Resources[i] >= village.Resource[i].Capacity)
                    {
                        outResType = i;
                        break;
                    }
                }

                if (outResType != -1)
                {
                    int outResCurrAmount = village.Resource[outResType].CurrAmount;
                    int outResCap = village.Resource[outResType].Capacity;
                    double minCap = 100.0;
                    int targetVillageID = -1;

                    foreach (var tsv in groupVillages)
                    {
                        TVillage tv = UpCall.TD.Villages[tsv.VillageID];
                        TResAmount tvres = GetVillageRes(tv, BalancerGroup.IgnoreMarket, BalancerGroup.IgnoreMarketTime);
                        TResAmount tvcap = GetVillageCapacity(tv);
                        double rate = tvres.Resources[outResType] * 100.0 / tvcap.Resources[outResType];
                        //double rate = tv.Resource[outResType].CurrAmount * 100.0 / tv.Resource[outResType].Capacity;
                        if (rate < minCap && rate < 80)
                        {
                            minCap = rate;
                            targetVillageID = tsv.VillageID;
                        }
                    }

                    if (targetVillageID != -1)
                    {
                        //计算运送的资源
                        TVillage targetVillage = UpCall.TD.Villages[targetVillageID];
                        TResAmount tvcurrent = GetVillageRes(targetVillage, BalancerGroup.IgnoreMarket, BalancerGroup.IgnoreMarketTime);
                        TResAmount tvcap = GetVillageCapacity(targetVillage);

                        //计算平均rate
                        double rate = (tvcurrent.Resources[outResType] + outResCurrAmount) * 100.0 / (tvcap.Resources[outResType] + outResCap);
                        //double rate = (targetVillage.Resource[outResType].CurrAmount + village.Resource[outResType].CurrAmount) * 100.0 / (targetVillage.Resource[outResType].Capacity + village.Resource[outResType].Capacity);
                        //int maxReceiveRes = targetVillage.Resource[outResType].Capacity * rate - targetVillage.Resource[outResType].CurrAmount;
                        //maxReceiveRes = maxReceiveRes * 8 / 10;
                        int maxReceiveRes = Convert.ToInt32((tvcap.Resources[outResType] * rate - tvcurrent.Resources[outResType]) / 100.0);
                        int maxCarry = village.Market.ActiveMerchant * village.Market.SingleCarry;
                        int maxSend = village.Resource[outResType].CurrAmount;
                        if (village.Market.LowerLimit != null)
                        {
                            if (village.Resource[outResType].CurrAmount - maxSend < village.Market.LowerLimit.Resources[outResType])
                            {
                                maxSend = village.Resource[outResType].CurrAmount - village.Market.LowerLimit.Resources[outResType];
                            }
                        }
                        maxSend = (maxCarry < maxSend) ? maxCarry : maxSend;
                        maxSend = (maxReceiveRes < maxSend) ? maxReceiveRes : maxSend;

                        if (targetVillage.Market.UpperLimit != null)
                        {
                            if (maxSend + tvcurrent.Resources[outResType] > targetVillage.Market.UpperLimit.Resources[outResType])
                            {
                                maxSend = targetVillage.Market.UpperLimit.Resources[outResType] - tvcurrent.Resources[outResType];
                            }
                        }

                        TResAmount sendRes = new TResAmount();
                        sendRes.Resources[outResType] = maxSend;
                        sendRes.NoNegative();
                        if (sendRes.isZero() == false)
                        {
                            if (sendRes.TotalAmount >= this.BalancerGroup.MinSendResource)
                            {
                                UpCall.DebugLog("push res from " + VillageShort(village) + " => " + VillageShort(targetVillage) + " " + sendRes, DebugLevel.E);
                                DoTranfer(village, targetVillage, sendRes);
                            }
                        }
                    }
                }

            }
            else
            {
                UpCall.DebugLog(VillageShort(village) + " resource is full " + village.ResourceCurrAmount, DebugLevel.E);
            }
        }

        //检测运送资源需要多少商人
        private int GetMarketMan(int totalSend, int carry)
        {
            int c = totalSend / carry;
            int r = totalSend % carry;
            if (r == 0)
            {
                return c;
            }
            else
            {
                return c + 1;
            }
        }

        private bool DoTranfer(TVillage from, TVillage to, TResAmount res)
        {
            res.NoNegative();
            if (res.isZero())
            {
                return false;
            }
            UpCall.DebugLog(VillageShort(from) + " => " + VillageShort(to) + " " + res.ToString(), DebugLevel.E);
            TransferQueue transfer = new TransferQueue()
            {
                UpCall = this.UpCall,
                VillageID = from.ID,
                TargetPos = to.Coord,
                ResourceAmount = res
            };
            if (transfer.ExceedTargetCapacity(UpCall.TD)) return false;
            transfer.Action();
            return true;
        }

        //按照距离更新村庄列表
        public void UpdateGroupVillages()
        {
            if (groupVillages == null)
            {
                groupVillages = new List<TSVillage>();
            }
            else
            {
                groupVillages.Clear();
            }
            foreach (var vid in UpCall.TD.Villages.Keys)
            {
                TVillage village = UpCall.TD.Villages[vid];

                BalancerQueue Q = village.getBalancer();
                if (Q != null)
                {
                    if (Q.BalancerGroup.ID == this.BalancerGroup.ID && village.ID != this.village.ID)
                    {
                        TSVillage one = new TSVillage
                        {
                            VillageID = vid,
                            coord = village.Coord,
                            distance = village.Coord * this.village.Coord,
                            //queue = Q
                        };
                        groupVillages.Add(one);
                    }
                }
            }
            groupVillages.Sort();
            //debug("Auto Balancer : " + this.village.Name + " Update Group VillageList, size = " + groupVillages.Count);
        }

        #endregion


        public static bool larger(TResAmount r1, TResAmount r2)
        {
            for (int i = 0; i < r1.Resources.Length; i++)
            {
                if (r1.Resources[i] < r2.Resources[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool smaller(TResAmount r1, TResAmount r2)
        {
            for (int i = 0; i < r1.Resources.Length; i++)
            {
                if (r1.Resources[i] > r2.Resources[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static void CalcAllVillages(Travian UpCall)
        {

            int[] TotalCurrent = new int[4]{0,0,0,0};
            int[] TotalCapacity = new int[4]{0,0,0,0};
            int[] TotalProduce = new int[4]{0,0,0,0};
            //for(int i=0;i<4;i++){
                //Total[i] = new TResource();
                //TotalCapacity[i] = new TResource();
                //TotalProduce[i] = new TResource();
            //}
            foreach (var x in UpCall.TD.Villages)
            {
                TVillage TV = x.Value;
                for (int i = 3; i >= 0; i--)
                {
                    TotalCurrent[i] += TV.Resource[i].CurrAmount;
                    TotalCapacity[i] += TV.Resource[i].Capacity;
                    TotalProduce[i] += TV.Resource[i].Produce;
                }
            }

            String[] NAME = {"WOOD","CLAY","IRON","CROP"};
            for (int i = 0; i < 4; i++)
            {
                UpCall.DebugLog(NAME[i] + ":" + TotalCurrent[i] + "/" + TotalCapacity[i] + " " + TotalProduce[i], DebugLevel.E);
            }
        }

        //用于比较距离，记录坐标和距离
        public class TSVillage : IComparable<TSVillage>
        {
            public int VillageID;
            public TPoint coord;
            public double distance;
            //public BalancerQueue queue;

            #region IComparable<TBVillage> 成员

            public int CompareTo(TSVillage other)
            {
                return (int)(distance - other.distance);
            }

            #endregion

            //public String toString()
            //{
            //    return queue.ToString() + VillageID;
            //}
        }

        public override String ToString()
        {
            return village.Name + type;
        }

        public static string VillageShort(TVillage CV)
        {
            return string.Format("{0} ({1}|{2})", CV.Name, CV.Coord.X, CV.Coord.Y);
        }

    }

    //平衡组设定
    public class TBalancerGroup
    {
        private static int generated = 1;

        private static TBalancerGroup DefaultTBalancerGroup;
        public static TBalancerGroup GetDefaultTBalancerGroup()
        {
            if (DefaultTBalancerGroup == null)
            {
                DefaultTBalancerGroup = new TBalancerGroup()
                {
                    ID = 0,
                };
            }
            return DefaultTBalancerGroup;
        }
        public TBalancerGroup()
        {
            ID = generated++;
            IgnoreMarket = BalancerQueue.IgnorMarketType.notignore;
            IgnoreMarketTime = 60 * 60 * 2;
            ReadyTime = 60;
            MinSendResource = 200;
            MaxSendResource = 100000;
        }

        [Json]
        public String desciption;
        [Json]
        public int ID;
        [Json]
        public BalancerQueue.IgnorMarketType IgnoreMarket { get; set; }//是否无视市场运输，1为无视
        [Json]
        public int IgnoreMarketTime { get; set; }//无视市场运输的时间，即大于这个时间的不计算，默认为一小时,如果小于0则不计算
        [Json]
        public int ReadyTime { get; set; } //提前计算的时间
        [Json]
        public int MinSendResource { get; set; }//最小运送资源
        [Json]
        public int MaxSendResource { get; set; }//最大运送资源
    }
    public interface TGidRules
    {
        bool allow(int Gid);
    }

    public class RomaOutRule : TGidRules
    {
        private static TGidRules rule;

        public static TGidRules GetRule()
        {
            if (rule == null)
            {
                rule = new RomaOutRule();
            }
            return rule;
        }
        private RomaOutRule()
        {
        }
        #region TGidRules 成员

        public bool allow(int Gid)
        {
            return Gid < 5 && Gid != 0;
        }

        #endregion
    }

    public class RomaInnerRule : TGidRules
    {
        private static TGidRules rule;

        public static TGidRules GetRule()
        {
            if (rule == null)
            {
                rule = new RomaInnerRule();
            }
            return rule;
        }

        private RomaInnerRule()
        {
        }
        #region TGidRules 成员

        public bool allow(int Gid)
        {
            return Gid > 4;
        }

        #endregion
    }

    public class NotRomaRule : TGidRules
    {
        private static TGidRules rule;

        public static TGidRules GetRule()
        {
            if (rule == null)
            {
                rule = new NotRomaRule();
            }
            return rule;
        }

        private NotRomaRule()
        {
        }
        #region TGidRules 成员

        bool TGidRules.allow(int Gid)
        {
            return Gid != 0;
        }

        #endregion
    }

}
