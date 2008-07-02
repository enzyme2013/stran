using Stran2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace TestStran2
{
    
    
    /// <summary>
    ///这是 UserLoaderTest 的测试类，旨在
    ///包含所有 UserLoaderTest 单元测试
    ///</summary>
	[TestClass()]
	public class UserLoaderTest
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
		///LoadUser 的测试
		///</summary>
		[TestMethod()]
		public void LoadUserTest()
		{
			TravianDataCenter TDC = new TravianDataCenter();
			string AccountName = "..\\..\\..\\TestStran2\\Account"; // TODO: 初始化为适当的值
			TDC.Users.Add("chihsun@speed.travian.tw", new UserData());
			UserLoader.LoadUser(TDC, AccountName);
			Assert.AreEqual<int>(TDC.Users.Count, 2);
			Assert.AreEqual<string>(TDC.Users["msg7086@a1.travian.cn"].StringProperties["Username"], "msg7086");
			Assert.AreEqual<string>(TDC.Users["msg7086@a1.travian.cn"].StringProperties["Server"], "a1.travian.cn");
			Assert.AreEqual<int>(TDC.Users["chihsun@speed.travian.tw"].Int32Properties["Tribe"], 0);
		}
	}
}
