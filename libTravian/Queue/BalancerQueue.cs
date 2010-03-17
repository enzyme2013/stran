using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LitJson;

namespace libTravian
{
    public class BalancerQueue : IQueue
    {

        #region IQueue 成员

        public Travian UpCall { get; set; }

        [Json]
        public int VillageID { get; set; }

        public bool MarkDeleted { get; set; }

        [Json]
        public bool Paused { get; set; }

        public string Title
        {
            get { return "AutoBalancer"; }
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
            switch (state)
            {
                case states.notinitlized:
                    UpdateType();
                    break;
                case states.initlized:
                    execute();
                    break;
                //case states.executing:
                //    break;
                //case states.done:
                //    break;
                default:
                    UpdateType();
                    break;
            }
        }

        public int QueueGUID
        {
            get
            {
                return 5;
            }
        }

        #endregion

        #region fields

        private int NextExec;
        private int retrycount;

        private int BalancerGroup;//资源平衡组

        private int ignoreMarket { get; set; }//是否无视市场运输
        private int ignoreTime { get; set; }//无视市场运输的时间，即大于这个时间的不计算

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
        }

        #endregion

        #region methods
        public BalancerQueue()
        {
            groupVillages = new List<TSVillage>();
        }

        //private void debug(String message)
        //{
        //    if (message == null) return;
        //    UpCall.DebugLog(message, DebugLevel.E);
        //}

        private void UpdateType()
        {

            village = UpCall.TD.Villages[VillageID];
            if (village.Name.Contains("047"))
            {
                int debug = 3;
            }
            if (village == null) return;
            if (DateTime.Now.Subtract(lastUpdateTime).TotalSeconds > 10)
            {
                UpdateGroupVillages();
                lastUpdateTime = DateTime.Now;
            }
            /*
            if (village.isMarketInitialized() == false)
            {
                debug("market not initialized, wait 10 second");
            }
             */
            if (village.Queue == null)
            {
                type = checkAvailableMerchant();

            }
            else
            {
                ///检查建筑队列
                TResAmount source = CaculateBuildingAmount(ignoreMarket, ignoreTime);
                source.clearMinus();
                if (source.isZero())
                {
                    source = CaculatePartyResource(ignoreMarket, ignoreTime);
                }
                source.clearMinus();
                if (source.isZero())
                {
                    source = CaculateResearchAmount(ignoreMarket, ignoreTime);
                }
                //把负数的清0
                source.clearMinus();
                //TODO 检查是否有资源限制
                needRes = source;
                if (source.isZero())
                {
                    //TODO 检查爆仓
                    type = checkAvailableMerchant();
                }
                else
                {

                    UpCall.DebugLog("Auto Balancer : " + village.Name + " need res " + source.ToString(), DebugLevel.E);
                    type = villagetype.needer;
                }
            }
            state = states.initlized;
        }

        //检测建筑序列所需的资源
        protected TResAmount CaculateBuildingAmount(int ignoreMarket, int ignorTime)
        {
            TResAmount resource = new TResAmount();
            if (UpCall.TD.isRomans)
            {
                //TODO罗马双造的处理
            }
            else
            {

            }
            //单造的处理
            if (village.InBuilding[0] == null && village.InBuilding[1] == null)
            {
                foreach (var task in village.Queue)
                {
                    if (task.GetType().Name == "BuildingQueue")
                    {
                        BuildingQueue Q = task as BuildingQueue;
                        TResAmount res = Buildings.Cost(village.Buildings[Q.Bid].Gid, village.Buildings[Q.Bid].Level + 1);
                        //建筑所需资源没有超过仓库上限
                        if ((larger(res, GetVillageCapacity(village))) == false)
                        {
                            resource += res;
                            break;
                        }
                    }
                    else if (task.GetType().Name == "AIQueue")
                    {
                        AIQueue Q = task as AIQueue;
                        if (Q.Gid != 0)
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
                }
            }
            resource -= GetVillageRes(village, ignoreMarket, ignoreTime);
            return resource;
        }

        //检测研究序列所需的资源
        protected TResAmount CaculateResearchAmount(int ignoreMarket, int ignorTime)
        {
            //UPAttack
            if (village.InBuilding[3] == null)
            {
                foreach (var task in village.Queue)
                {
                    if (task.GetType().Name == "ResearchQueue")
                    {
                        ResearchQueue Q = task as ResearchQueue;
                        if (Q.ResearchType == ResearchQueue.TResearchType.UpAttack)
                        {
                            TResAmount res = Buildings.UpCost[(UpCall.TD.Tribe - 1) * 10 + Q.Aid][village.Upgrades[Q.Aid].AttackLevel];
                            return res - GetVillageRes(village, ignoreMarket, ignorTime);
                        }
                    }
                }
            }
            //UPDefense
            if (village.InBuilding[4] == null)
            {
                foreach (var task in village.Queue)
                {
                    if (task.GetType().Name == "ResearchQueue")
                    {
                        ResearchQueue Q = task as ResearchQueue;
                        if (Q.ResearchType == ResearchQueue.TResearchType.UpDefence)
                        {
                            TResAmount res = Buildings.UpCost[(UpCall.TD.Tribe - 1) * 10 + Q.Aid][village.Upgrades[Q.Aid].DefenceLevel];
                            return res - GetVillageRes(village, ignoreMarket, ignorTime);
                        }
                    }
                }
            }
            //Research
            if (village.InBuilding[5] == null)
            {
                foreach (var task in village.Queue)
                {
                    if (task.GetType().Name == "ResearchQueue")
                    {
                        ResearchQueue Q = task as ResearchQueue;
                        if (Q.ResearchType == ResearchQueue.TResearchType.Research)
                        {
                            TResAmount res = Buildings.ResearchCost[(UpCall.TD.Tribe - 1) * 10 + Q.Aid];
                            return res - GetVillageRes(village, ignoreMarket, ignorTime);
                        }
                    }
                }
            }
            return new TResAmount();
        }

        //检测Party所需的资源
        protected TResAmount CaculatePartyResource(int ignoreMarket, int ignorTime)
        {
            if (village.InBuilding[6] == null)
            {
                foreach (var task in village.Queue)
                {
                    if (task.GetType().Name == "PartyQueue")
                    {
                        PartyQueue Q = task as PartyQueue;
                        TResAmount res = Buildings.PartyCos[(int)Q.PartyType - 1];
                        if ((larger(res, GetVillageCapacity(village))) == false)
                        {
                            return res -= GetVillageRes(village, ignoreMarket, ignoreTime);
                        }
                    }
                }
            }
            return new TResAmount();
        }

        //返回目标村庄的资源上限
        protected TResAmount GetVillageCapacity(TVillage village)
        {
            return village.ResourceCapacity;
        }

        //返回村庄当前资源
        protected TResAmount GetVillageRes(TVillage village, int ignoreMarket, int ignorTime)
        {
            TResAmount res = new TResAmount(village.ResourceCurrAmount);
            //TODO ignoreMarket和ignoreTime的处理
            foreach (TMInfo transfer in village.Market.MarketInfo)
            {
                res += transfer.CarryAmount;
            }
            return res;
        }

        //检查市场
        private villagetype checkAvailableMerchant()
        {
            if (village.Market.ActiveMerchant <= 0)
            {
                return villagetype.marketnotavailable;
            }
            /*
             //TODO 爆仓处理
            else if (village.full())
            {
                return villagetype.full;
            }
            */

            else
            {
                return villagetype.giver;
            }
        }

        private void execute()
        {
            //TODO 从push模式改成pull模式

            if (type == villagetype.needer)
            {
                TResAmount res = new TResAmount(needRes);
                foreach (var tsv in groupVillages)
                {
                    //tsv.queue.UpdateState();
                    TVillage fromVillage = UpCall.TD.Villages[tsv.VillageID];
                    if (tsv.queue.type == villagetype.giver
                        || tsv.queue.type == villagetype.full)
                    {

                        TResAmount r = GetVillageRes(fromVillage, ignoreMarket, ignoreTime);
                        int marketCarry = fromVillage.Market.ActiveMerchant * fromVillage.Market.SingleCarry;
                        //资源和商人都充足
                        if (res.TotalAmount < marketCarry && smaller(res, r))
                        {
                            DoTranfer(fromVillage, this.village, res);
                            res -= res;
                        }
                        else
                        {
                            int totalSend = 0;
                            int[] sendRes = new int[r.Resources.Length];
                            for (int i = 0; i < sendRes.Length; i++)
                            {
                                int thisTypeCount = needRes.Resources[i];
                                if (thisTypeCount > marketCarry)
                                {
                                    sendRes[i] = (marketCarry > r.Resources[i]) ? r.Resources[i] : marketCarry;
                                }
                                else
                                {
                                    sendRes[i] = (r.Resources[i] > thisTypeCount) ? thisTypeCount : r.Resources[i];
                                }
                                marketCarry -= sendRes[i];
                            }
                            TResAmount r2 = new TResAmount(sendRes);
                            DoTranfer(fromVillage, this.village, r2);
                            res -= r2;
                        }
                    }

                    if (res.isZero())
                    {
                        break;
                    }
                }
            }
            UpdateType();
        }

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

        private void DoTranfer(TVillage from, TVillage to, TResAmount res)
        {
            UpCall.DebugLog("Balancer : " + from.Name + " => " + to.Name + " " + res.ToString(), DebugLevel.E);
            TransferQueue transfer = new TransferQueue()
            {
                UpCall = this.UpCall,
                VillageID = from.ID,
                TargetPos = to.Coord,
                ResourceAmount = res
            };
            transfer.Action();
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
                    TSVillage one = new TSVillage
                    {
                        VillageID = vid,
                        coord = village.Coord,
                        distance = village.Coord * this.village.Coord,
                        queue = Q
                    };
                    groupVillages.Add(one);
                }
            }
            groupVillages.Sort();
            foreach (var v in groupVillages)
            {
                TVillage TV = UpCall.TD.Villages[v.VillageID];
                //UpCall.DebugLog(TV.Name + v.distance,DebugLevel.E);
            }
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

        //用于比较距离，记录坐标和距离
        public class TSVillage : IComparable<TSVillage>
        {
            public int VillageID;
            public TPoint coord;
            public double distance;
            public BalancerQueue queue;

            #region IComparable<TBVillage> 成员

            public int CompareTo(TSVillage other)
            {
                return (int)(distance - other.distance);
            }

            #endregion

            public String toString()
            {
                return queue.ToString() + VillageID;
            }
        }

        public String ToString()
        {
            return village.Name + type;
        }
    }
}
