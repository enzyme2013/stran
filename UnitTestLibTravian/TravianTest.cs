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

		/// <summary>
		///A test for doNpcTrade
		///</summary>
		[TestMethod()]
		public void doNpcTradeTest()
		{
			// We need non-null server/user name for local DB
			Data travianData = new Data()
			{
				Server = "none",
				Username = "none"
			};

			// Init Travian object
			Dictionary<string, string> Options = new Dictionary<string, string>();
			Travian target = new Travian(travianData, Options);
			
			int villageID = 1;
			TVillage village = new TVillage();
			travianData.Villages[villageID] = village;

			village.Resource = new TResource[4];
			village.Resource[0] = new TResource(0, 0, 560000);
			village.Resource[1] = new TResource(0, 0, 560000);
			village.Resource[2] = new TResource(0, 0, 560000);
			village.Resource[3] = new TResource(0, 0, 560000);
			village.isBuildingInitialized = 2;

			// Add mock pages
			MockPageQuerier pageQuerier = new MockPageQuerier();
			target.pageQuerier = pageQuerier;
			pageQuerier.AddPage("build.php?gid=17&t=3", null, Properties.Resources.NPCTradeForm);

			Dictionary<string, string> postData = new Dictionary<string, string>();
			postData["id"] = "38";
			postData["t"] = "3";
			postData["a"] = "6";
			postData["!!!RawData!!!"] = "m2[]=404373&m1[]=234904&m2[]=519907&m1[]=303718&m2[]=404371&m1[]=230029&m2[]=0&m1[]=560000";
			pageQuerier.AddPage("build.php", postData, Properties.Resources.NPCTradeResult);


			// When the thresholds are too high
			NpcTradeOption option = new NpcTradeOption()
			{
				Threshold = new TResAmount(0, 0, 560000, 560000),
				Distribution = new TResAmount(427835, 550075, 427835, 0)
			};
			Assert.AreEqual(Travian.NpcTradeResult.Delay, target.doNpcTrade(villageID, option));

			// When the thresholds are ok
			option.Threshold = new TResAmount(0, 0, 0, 560000);
			TResAmount distribution = new TResAmount(427835, 550075, 427835, 0);
			Assert.AreEqual(Travian.NpcTradeResult.Success, target.doNpcTrade(villageID, option));
		}

		/// <summary>
		/// Provide mock pages for matching queries
		/// </summary>
		private class MockPageQuerier : IPageQuerier
		{
			private struct MockPage
			{
				public string url;
				public Dictionary<string, string> postData;
				public string result;
			}

			private List<MockPage> mockPages = new List<MockPage>();

			public void AddPage(string url, Dictionary<string, string> postData, string result)
			{
				MockPage page = new MockPage();
				page.url = url;
				page.postData = postData;
				page.result = result;
				this.mockPages.Add(page);
			}

			public string PageQuery(int villageID, string uri, Dictionary<string, string> data, bool checkLogin, bool noParser)
			{
				foreach (MockPage page in this.mockPages)
				{
					if (page.url == uri)
					{
						bool matchesAll = true;
						if (page.postData != null)
						{
							foreach (string key in page.postData.Keys)
							{
								if (data == null || !data.ContainsKey(key) || data[key] != page.postData[key])
								{
									matchesAll = false;
									break;
								}
							}
						}

						if (matchesAll)
						{
							return page.result;
						}
					}
				}

				return null;
			}
		}
	}
}
