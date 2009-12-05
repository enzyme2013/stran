using libTravian;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace UnitTests
{
  
    /// <summary>
    ///This is a test class for TravianTest and is intended
    ///to contain all TravianTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TravianTest
    {

        /// <summary>
        ///A test for GetBuildingLevel
        ///</summary>
        [TestMethod()]
        public void GetBuildingLevelCN()
        {
            Travian target = new Travian();
            int gid = 17;
            string pageContent = "<h1>市场 <span class=\"level\">等级 20</span></h1>";
            target.SetGidLang(gid, "市场");
            Assert.AreEqual(20, target.GetBuildingLevel(gid, pageContent));

            target.SetGidLang(gid, "XX");
            Assert.AreEqual(-1, target.GetBuildingLevel(gid, pageContent));
        }

        /// <summary>
        ///A test for GetBuildingLevel
        ///</summary>
        [TestMethod()]
        public void GetBuildingLevelUS()
        {
            Travian target = new Travian();
            int gid = 5;
            string pageContent = "<h1>Main Building <span class=\"level\">level 3</span></h1>";
            target.SetGidLang(gid, "Main Building");
            Assert.AreEqual(3, target.GetBuildingLevel(gid, pageContent));
        }
    }
}
