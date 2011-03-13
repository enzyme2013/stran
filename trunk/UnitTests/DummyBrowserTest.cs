using libTravian;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
namespace UnitTests
{
    
    
    /// <summary>
    ///这是 DummyBrowserTest 的测试类，旨在
    ///包含所有 DummyBrowserTest 单元测试
    ///</summary>
	[TestClass()]
	public class DummyBrowserTest
	{


		private TestContext testContextInstance;

		/// <summary>
		///获取或设置测试上下文，上下文提供
		///有关当前测试运行及其功能的信息。
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region 附加测试属性
		// 
		//编写测试时，还可使用以下属性:
		//
		//使用 ClassInitialize 在运行类中的第一个测试前先运行代码
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//使用 ClassCleanup 在运行完类中的所有测试后再运行代码
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//使用 TestInitialize 在运行每个测试前先运行代码
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//使用 TestCleanup 在运行完每个测试后运行代码
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion


		/// <summary>
		///DummyCheck 的测试
		///</summary>
		[TestMethod()]
		public void DummyCheckTest()
		{
			string PageData = File.ReadAllText("..\\..\\PAGEDUMP_634355902211875000.htm");
			MockPQ mpq = new MockPQ();
			DummyBrowser.DummyCheck(PageData, mpq);
			Assert.AreEqual(3, mpq.Page.Count);
			Assert.AreEqual("stats.php?p=c97&login=7217&b=22", mpq.Page[2]);
			//Assert.Inconclusive("无法验证不返回值的方法。");
		}
	}

	class MockPQ : IPageQuerier
	{
		public List<string> Page = new List<string>();
		#region IPageQuerier 成员

		string refuri;
		public string PageQuery(int VillageID, string Uri, Dictionary<string, string> Data, bool CheckLogin, bool NoParser)
		{
			return this.PageQuery(VillageID, Uri, Data, CheckLogin, NoParser, false);
		}

		public string PageQuery(int VillageID, string Uri, Dictionary<string, string> Data, bool CheckLogin, bool NoParser, bool N)
		{
			Page.Add(Uri);
			refuri = Uri;
			return string.Empty;
		}

		public string Referer
		{
			get
			{
				return string.Format("http://{0}/{1}", "speed.travian.cc", "dorf1.php");
			}
		}

		#endregion
	}
}
