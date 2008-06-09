using System.Collections.Generic;
using libTravian;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestLibTravian
{
	/// <summary>
	///This is a test class for TransferOptionTest and is intended
	///to contain all TransferOptionTest Unit Tests
	///</summary>
	[TestClass()]
	public class TransferOptionTest
	{
		/// <summary>
		///A test for Status
		///</summary>
		[TestMethod()]
		public void StatusTest()
		{
			TransferOption target = new TransferOption() { ResourceAmount = new TResAmount(750, 250, 500, 500) };
			string actual;

			target.Distribution = ResourceDistributionType.None;
			actual = target.Status;
			Assert.AreEqual("0/Inf=>750/250/500/500", actual);

			target.MaxCount = 3;
			target.Count = 2;
			actual = target.Status;
			Assert.AreEqual("2/3=>750/250/500/500", actual);

			target.Distribution = ResourceDistributionType.BalanceSource;
			actual = target.Status;
			Assert.AreEqual("2/3=S=>750/250/500/500", actual);
		}

		/// <summary>
		///A test for FromString
		///</summary>
		[TestMethod()]
		public void FromStringTest()
		{
			string s = "0&0&0&0&0&0&0&0&0&None&False";
			TransferOption expected = new TransferOption();
			TransferOption actual;
			actual = TransferOption.FromString(s);
			Assert.AreEqual(expected.TargetVillageID, actual.TargetVillageID);
			Assert.AreEqual(expected.MaxCount, actual.MaxCount);
			Assert.AreEqual(expected.Distribution, actual.Distribution);
			Assert.AreEqual(expected.NoCrop, actual.NoCrop);
			Assert.AreEqual(expected.MinimumDelay, actual.MinimumDelay);
		}

		/// <summary>
		///A test for ToString
		///</summary>
		[TestMethod()]
		public void ToStringTest()
		{
			TransferOption target = new TransferOption();
			string expected = "0&0&0&0&0&0&0&0&0&None&False&0";
			string actual;
			actual = target.ToString();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		///A test for CalculateResourceAmount
		///</summary>
		[TestMethod()]
		public void CalculateResourceAmountTest()
		{
			TransferOption target = new TransferOption() { TargetVillageID = 2 };
			Data travianData = new Data();
			int sourceVillageID = 1;

			// No balance
			target.Distribution = ResourceDistributionType.None;
			target.ResourceAmount = new TResAmount(3000, 4000, 3000, 2375);
			target.CalculateResourceAmount(travianData, sourceVillageID);
			Assert.AreEqual(new TResAmount(3000, 4000, 3000, 2375), target.ResourceAmount);

			// Uniform distribution
			target.Distribution = ResourceDistributionType.EvenDistribution;
			target.NoCrop = false;
			target.CalculateResourceAmount(travianData, sourceVillageID);
			Assert.AreEqual(new TResAmount(3096, 3093, 3093, 3093), target.ResourceAmount);

			target.NoCrop = true;
			target.CalculateResourceAmount(travianData, sourceVillageID);
			Assert.AreEqual(new TResAmount(4125, 4125, 4125, 0), target.ResourceAmount);

			// Balance source village
			TVillage sourceVillage = new TVillage();
			sourceVillage.Resource = new TResource[4];
			sourceVillage.Resource[0] = new TResource(0, 5000, 8000);
			sourceVillage.Resource[1] = new TResource(0, 2000, 8000);
			sourceVillage.Resource[2] = new TResource(0, 3000, 8000);
			sourceVillage.Resource[3] = new TResource(0, 4000, 8000);
			sourceVillage.isBuildingInitialized = 2;
			travianData.Villages[sourceVillageID] = sourceVillage;

			target.Distribution = ResourceDistributionType.BalanceSource;
			target.NoCrop = false;
			target.CalculateResourceAmount(travianData, sourceVillageID);
			Assert.AreEqual(new TResAmount(4725, 1550, 2550, 3550), target.ResourceAmount);

			target.NoCrop = true;
			target.CalculateResourceAmount(travianData, sourceVillageID);
			Assert.AreEqual(new TResAmount(5875, 2750, 3750, 0), target.ResourceAmount);

			// Balance destination village
			TVillage destinationVillage = new TVillage();
			destinationVillage.Resource = new TResource[4];
			destinationVillage.Resource[0] = new TResource(0, 5000, 80000);
			destinationVillage.Resource[1] = new TResource(0, 2000, 80000);
			destinationVillage.Resource[2] = new TResource(0, 3000, 80000);
			destinationVillage.Resource[3] = new TResource(0, 4000, 80000);
			destinationVillage.isBuildingInitialized = 2;
			travianData.Villages[target.TargetVillageID] = destinationVillage;

			target.Distribution = ResourceDistributionType.BalanceTarget;
			target.NoCrop = false;
			target.CalculateResourceAmount(travianData, sourceVillageID);
			Assert.AreEqual(new TResAmount(1550, 4725, 3550, 2550), target.ResourceAmount);

			target.NoCrop = true;
			target.CalculateResourceAmount(travianData, sourceVillageID);
			Assert.AreEqual(new TResAmount(2450, 5475, 4450, 0), target.ResourceAmount);
		}

		/// <summary>
		///A test for ExceedTargetCapacity
		///</summary>
		[TestMethod()]
		public void ExceedTargetCapacityTest()
		{
			TransferOption target = new TransferOption()
			{
				TargetVillageID = 2,
				ResourceAmount = new TResAmount(3000, 4000, 3000, 2375),
				Distribution = ResourceDistributionType.BalanceTarget
			};
			Data travianData = new Data();

			int sourceVillageID = 1;
			travianData.Villages[sourceVillageID] = new TVillage();

			TVillage destinationVillage = new TVillage();
			destinationVillage.Resource = new TResource[4];
			destinationVillage.Resource[0] = new TResource(0, 5000, 7000);
			destinationVillage.Resource[1] = new TResource(0, 2000, 7000);
			destinationVillage.Resource[2] = new TResource(0, 3000, 7000);
			destinationVillage.Resource[3] = new TResource(0, 4000, 7000);
			destinationVillage.isBuildingInitialized = 2;
			travianData.Villages[target.TargetVillageID] = destinationVillage;

			// Tests with 0 production rate
			target.CalculateResourceAmount(travianData, sourceVillageID);
			Assert.AreEqual(new TResAmount(1550, 4725, 3550, 2550), target.ResourceAmount);
			Assert.IsFalse(target.ExceedTargetCapacity(travianData, sourceVillageID));

			target.NoCrop = true;
			target.CalculateResourceAmount(travianData, sourceVillageID);
			Assert.AreEqual(new TResAmount(2450, 5475, 4450, 0), target.ResourceAmount);
			Assert.IsTrue(target.ExceedTargetCapacity(travianData, sourceVillageID));

			destinationVillage.isBuildingInitialized = 0;
			target.Distribution = ResourceDistributionType.None;
			Assert.IsFalse(target.ExceedTargetCapacity(travianData, sourceVillageID));

			target.Distribution = ResourceDistributionType.BalanceTarget;
			Assert.IsTrue(target.ExceedTargetCapacity(travianData, sourceVillageID));

			// Tests with positive production rate
			destinationVillage.isBuildingInitialized = 2;
			destinationVillage.Coord = new TPoint(1, 1);
			destinationVillage.Resource[0] = new TResource(100, 200, 200);
			destinationVillage.Resource[1] = new TResource(100, 0, 200);
			destinationVillage.Resource[2] = new TResource(100, 0, 200);
			destinationVillage.Resource[3] = new TResource(100, 0, 200);
			target.ResourceAmount = new TResAmount(0, 200, 200, 200);
			Assert.IsTrue(target.ExceedTargetCapacity(travianData, sourceVillageID));

			target.ResourceAmount = new TResAmount(0, 100, 100, 100);
			Assert.IsFalse(target.ExceedTargetCapacity(travianData, sourceVillageID));
		}

		/// <summary>
		///A test for GetTitle
		///</summary>
		[TestMethod()]
		public void GetTitleTest()
		{
			TransferOption target = new TransferOption()
				{
					TargetVillageID = 1,
					TargetPos = new TPoint(99, 101)
				};

			// Unknown target village
			Data travianData = new Data();
			string expected = "99/101";
			string actual;
			actual = target.GetTitle(travianData);
			Assert.AreEqual(expected, actual);

			// Target village is one of my own
			travianData.Villages[target.TargetVillageID] = new TVillage()
			{
				ID = target.TargetVillageID,
				Name = "Fargo"
			};
			expected = "99/101 Fargo";
			actual = target.GetTitle(travianData);
			Assert.AreEqual(expected, actual);
		}
	}
}
