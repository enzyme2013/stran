using Stran2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace TestStran2
{
    
    
    /// <summary>
    ///这是 Plugin_VillageTest 的测试类，旨在
    ///包含所有 Plugin_VillageTest 单元测试
    ///</summary>
	[TestClass()]
	public class Plugin_VillageTest
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
		///Initialize 的测试
		///</summary>
		[TestMethod()]
		public void InitializeTest()
		{
			Plugin_Village target = new Plugin_Village();
			MethodsCenter MC = MethodsCenter.Instance;
			MC.ReadyToRegisterFor("test", target);
			target.Initialize(MC);
			Assert.AreEqual<int>(MC.ParserCount, 1);
			Assert.AreEqual<int>(MC.MethodsCount, 0);
			Assert.AreEqual<int>(MC.ActionCount, 0);
			target.PQ = new Plugin_Village_Web_Simulater();
			Assert.Inconclusive();
		}
	}

	public class Plugin_Village_Web_Simulater : IPageQuerier
	{
		#region IPageQuerier 成员

		public string Get(UserData UD, int VillageID, string Uri)
		{
			throw new NotImplementedException();
		}

		public string Post(UserData UD, int VillageID, string Uri, System.Collections.Generic.Dictionary<string, string> Data)
		{
			throw new NotImplementedException();
		}

		public string GetEx(UserData UD, int VillageID, string Uri, System.Collections.Generic.Dictionary<string, string> Data, bool CheckLogin, bool NoParser)
		{
			string b = "..\\..\\..\\test\\Plugin_Village";
		}

		#endregion
	}
}
