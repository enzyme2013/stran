using System;
using libTravian;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestLibTravian
{
	/// <summary>
	///This is a test class for NpcTradeOptionTest and is intended
	///to contain all NpcTradeOptionTest Unit Tests
	///</summary>
	[TestClass()]
	public class NpcTradeOptionTest
	{
		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			NpcTradeOption target = new NpcTradeOption(); 
			string expected = "0&0&0&0&0&0&0&0&0"; 
			string actual;

			// Empty (invalid) option
			actual = target.ToString();
			Assert.AreEqual(expected, actual);

			// An non-empty option
			target = new NpcTradeOption()
			{
				Threshold = new TResAmount(0, 0, 0, 10000),
				Distribution = new TResAmount(700, 900, 700, 0)
			};

			expected = "0&0&0&0&10000&700&900&700&0";
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for FromString
		///</summary>
		[TestMethod()]
		public void FromStringTest()
		{
			string s = string.Empty; 
			NpcTradeOption actual = NpcTradeOption.FromString(s);

			// Empty string convert to empty option
			Assert.AreEqual(new TResAmount(), actual.Threshold);
			Assert.AreEqual(new TResAmount(), actual.Distribution);

			// Non empty option
			s = "0&0&0&0&10000&700&900&700&0";
			actual = NpcTradeOption.FromString(s);

			Assert.AreEqual(new TResAmount(0, 0, 0, 10000), actual.Threshold);
			Assert.AreEqual(new TResAmount(700, 900, 700, 0), actual.Distribution);
		}

		/// <summary>
		///A test for IsValid
		///</summary>
		[TestMethod()]
		public void IsValidTest()
		{
			// Empty option is invalid
			NpcTradeOption target = new NpcTradeOption(); 
			Assert.IsFalse(target.IsValid);

			// Otherwise, it's valid
			target.Threshold = new TResAmount(0, 0, 0, 10000);
			target.Distribution = new TResAmount(700, 900, 700, 0);
			Assert.IsTrue(target.IsValid);
		}

		/// <summary>
		///A test for Status
		///</summary>
		[TestMethod()]
		public void StatusTest()
		{
			NpcTradeOption target = new NpcTradeOption()
			{
				Threshold = new TResAmount(0, 0, 0, 10000),
				Distribution = new TResAmount(700, 900, 700, 0)
			};

			Assert.AreEqual("700|900|700|0", target.Status);
		}

		/// <summary>
		///A test for Title
		///</summary>
		[TestMethod()]
		public void TitleTest()
		{
			NpcTradeOption target = new NpcTradeOption()
			{
				Threshold = new TResAmount(0, 0, 0, 10000),
				Distribution = new TResAmount(700, 900, 700, 0)
			};

			Assert.AreEqual("0|0|0|10000->", target.Title);
		}

		/// <summary>
		///A test for GetDelay
		///</summary>
		[TestMethod()]
		public void GetDelayTest()
		{
			NpcTradeOption target = new NpcTradeOption() 
			{ 
				Threshold = new TResAmount(0, 0, 0, 10000),
				Distribution = new TResAmount(1900, 1900, 1900, 0)
			};

			Data travianData = new Data();
			int villageID = 1;
			TVillage village = new TVillage();
			travianData.Villages[villageID] = village;

			village.Resource = new TResource[4];
			village.Resource[0] = new TResource(1000, 1000, 2000);
			village.Resource[1] = new TResource(1000, 1000, 2000);
			village.Resource[2] = new TResource(1000, 1000, 2000);
			village.Resource[3] = new TResource(2000, 7000, 10000);
			village.isBuildingInitialized = 2;

			// Don't have enough resource
			Assert.AreEqual(3000 * 3600 / 2000, target.GetDelay(travianData, villageID));

			// Redistribution busted!!
			village.Resource[3] = new TResource(0, 10000, 10000);
			Assert.AreEqual(86400, target.GetDelay(travianData, villageID));

			// Now have the resource and capacity
			village.Resource[0] = new TResource(0, 0, 2000);
			village.Resource[1] = new TResource(0, 0, 2000);
			village.Resource[2] = new TResource(0, 0, 2000);
			Assert.AreEqual(0, target.GetDelay(travianData, villageID));

			// The task is postphoned
			target.MinimumDelay = 100;
			Assert.AreEqual(100, target.GetDelay(travianData, villageID));
		}

		/// <summary>
		///A test for RedistributeResources
		///</summary>
		[TestMethod()]
		public void RedistributeResourcesTest()
		{
			NpcTradeOption target = new NpcTradeOption() 
			{ 
				Threshold = new TResAmount(0, 0, 0, 10000) ,
				Distribution = new TResAmount(1000, 1500, 1000, 0)
			};
			Data travianData = new Data();
			int villageID = 1;
			TVillage village = new TVillage();
			travianData.Villages[villageID] = village;

			village.Resource = new TResource[4];
			village.Resource[0] = new TResource(1000, 1000, 2000);
			village.Resource[1] = new TResource(1000, 1000, 2000);
			village.Resource[2] = new TResource(1000, 1000, 2000);
			village.Resource[3] = new TResource(2000, 10000, 10000);
			village.isBuildingInitialized = 2;

			// Less than target distribution
			int sum = 2800;
			TResAmount expected = new TResAmount(800, 1200, 800, 0);
			TResAmount actual = target.RedistributeResources(travianData, villageID, sum);
			Assert.AreEqual(expected, actual);

			// More than target distribution, but under capacity
			sum = 4200;
			expected = new TResAmount(1200, 1800, 1200, 0);
			actual = target.RedistributeResources(travianData, villageID, sum);
			Assert.AreEqual(expected, actual);

			// Exceed capacity for resource 2
			sum = 4900;
			expected = new TResAmount(1450, 2000, 1450, 0);
			actual = target.RedistributeResources(travianData, villageID, sum);
			Assert.AreEqual(expected, actual);

			// Now even resource 4 got some distribution
			sum = 7000;
			expected = new TResAmount(2000, 2000, 2000, 1000);
			actual = target.RedistributeResources(travianData, villageID, sum);
			Assert.AreEqual(expected, actual);

			// Busted!!!
			sum = 11001;
			expected = null;
			actual = target.RedistributeResources(travianData, villageID, sum);
			Assert.AreEqual(expected, actual);
		}
	}
}
