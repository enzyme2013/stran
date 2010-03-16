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

        public Travian UpCall {get;set;}

        [Json]
        public int VillageID { get;set;}

        public bool MarkDeleted { get; set; }

        [Json]
        public bool Paused { get; set; }

        public string Title
        {
            get { return "AutoBalancer"; }
        }

        public string Status
        {
            get {
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
                        return "";
                }
            }
        }

        public int CountDown
        {
            get {
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
        int delay = 10;

        public void Action()
        {
            switch (state)
            {
                case states.notinitlized:
                    doInitlize();
                    break;
                case states.initlized:
                    execute();
                    break;
                case states.executing:
                    break;
                case states.done:
                    break;
            }
        }

        public int QueueGUID
        {
            get {
                return 6;
            }
        }

        #endregion

        private int NextExec;
        private int retrycount;

        private int BalancerGroup;//资源平衡组

        private TVillage village;
        private states state;//状态
        private villagetype type;//类型
        private TResAmount needRes = new TResAmount();//需要的资源
        

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

        public enum givertype
        {

        }

        private void debug(String message)
        {
            if (message == null) return;
            UpCall.DebugLog(message, DebugLevel.E);
        }

        private void doInitlize()
        {
            village = UpCall.TD.Villages[VillageID];
            if (village == null) return;
            /*
            if (village.isMarketInitialized() == false)
            {
                debug("market not initialized, wait 10 second");
            }
             */
            else if (village.Queue == null)
            {
                type = checkAvailableMerchant();
               
            } else {
                type = villagetype.unknown;
                ///检查建筑队列
                TInBuilding[] Cinb = village.InBuilding;
                //如果已经在建筑的无视
                //TODO 罗马双造
                //如果没在建筑，则检查是否有建筑队列
                if (Cinb[1] == null){
                    foreach (var task in village.Queue)
                    {
                        if (task.GetType().Name == "BuildingQueue")
                        {
                            BuildingQueue Q = task as BuildingQueue;
                            type = villagetype.needer;
                            needRes += Buildings.Cost(village.Buildings[Q.Bid].Gid, village.Buildings[Q.Bid].Level + 1)
                                - village.ResourceCurrAmount;
                            //把负数的清0
                            needRes.clearMinus();
                            //TODO1 检查市场里是否已经有了运送的资源
                            //TODO2 检查是否有资源限制
                            //TODO3 检查当前商人和运载量

                            debug("Auto Balancer => " + village.Name  + needRes.ToString());
                            break;
                        }
                    }
                }
                //TODO Party Queues

                //TODO Research Queue

                if (type == villagetype.unknown)
                {
                    type = checkAvailableMerchant();
                }
            }
            state = states.initlized;
        }

        private villagetype checkAvailableMerchant()
        {
            if (village.Market.ActiveMerchant > 0)
            {
                return villagetype.giver;
            }
            else
            {
                return villagetype.marketnotavailable;
            }
        }

        private void execute()
        {
            if (type == villagetype.giver)
            {
                foreach (var vid in UpCall.TD.Villages.Keys)
                {
                    var CV = UpCall.TD.Villages[vid];
                    BalancerQueue queue = CV.getBalancer();
                    if (queue != null)
                    {
                        //TODO1 增加Balancer Group设定
                        //TODO2 增加自动寻找最近的村子
                        if (queue.type == villagetype.needer)
                        {
                            TResAmount targetRes = queue.needRes;
                            //计算运送的资源
                            TResAmount sendRes = targetRes;
                            TransferQueue transfer = new TransferQueue()
                            {
                                UpCall = this.UpCall,
                                VillageID = this.VillageID,
                            };
                            transfer.TargetPos = new TPoint(queue.village.X, queue.village.Y);
                            transfer.ResourceAmount = sendRes;
                            transfer.Action();

                        }
                    }
                }
            }
            state = states.notinitlized;
        }
        

    }
}
