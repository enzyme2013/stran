using libTravian;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTestLibTravian
{
    /// <summary>
    ///This is a test class for TravianTest and is intended
    ///to contain all TravianTest Unit Tests
    ///</summary>
	[TestClass()]
	public class TravianTest
	{
		/// <summary>
		///A test for GetDelay for Transfer jobs
		///</summary>
		[TestMethod()]
		public void GetDelayTest()
		{
			// We need non-null server/user name for local DB
			Data TravianData = new Data() 
			{
				Server = "none",
				Username = "none"
			};

			// Init Travian object
			Dictionary<string, string> Options = new Dictionary<string, string>();
			Travian target = new Travian(TravianData, Options); 

			// Source village with resource amount/capacity = 2000/5000
			int VillageID = 1;
			TVillage village = new TVillage() { isBuildingInitialized = 2 };
			TravianData.Villages[VillageID] = village;
			for (int i = 0; i < village.Resource.Length; i++)
			{
				village.Resource[i] = new TResource(0, 2000, 5000);
			}

			// Transfer task of 1000 resource per category
			TransferOption option = new TransferOption()
			{
				ResourceAmount = new TResAmount(1000, 1000, 1000, 1000)
			};

			TQueue Q = new TQueue()
			{
				QueueType = TQueueType.Transfer,
				ExtraOptions = option.ToString(),
			};

			// No merchants
			Assert.AreEqual(5, target.GetDelay(VillageID, Q));

			// Mearchnts ready
			TMarket market = village.Market;
			market.SingleCarry = 500;
			market.ActiveMerchant = 10;
			Assert.AreEqual(0, target.GetDelay(VillageID, Q));

			// Source village has a resource lower bound
			market.LowerLimit = new TResAmount(1500, 1500, 1500, 1500);
			Assert.AreEqual(32767, target.GetDelay(VillageID, Q));

			// Some resource amount is already below the lower bound
			market.ActiveMerchant = 3;
			market.LowerLimit = new TResAmount(1000, 2500, 1000, 500);
			option.ResourceAmount = new TResAmount(500, 0, 500, 500);
			Q.ExtraOptions = option.ToString();
			Assert.AreEqual(0, target.GetDelay(VillageID, Q));
		}
	}
}
