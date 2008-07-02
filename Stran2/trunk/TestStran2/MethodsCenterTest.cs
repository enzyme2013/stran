using Stran2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace TestStran2
{
    
    
    /// <summary>
    ///这是 MethodsCenterTest 的测试类，旨在
    ///包含所有 MethodsCenterTest 单元测试
    ///</summary>
	[TestClass()]
	public class MethodsCenterTest : IPlugin
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
		///RegisterParser 的测试
		///</summary>
		[TestMethod()]
		public void RegisterParserTest()
		{
			MethodsCenter target = new MethodsCenter(); // TODO: 初始化为适当的值
			ParserPluginCall Call = new ParserPluginCall(dummyparser);
			UserData users = new UserData();
			target.RegisterParser(Call);
			try
			{
				target.RegisterParser(Call);
				Assert.Fail();
			}
			catch(InvalidOperationException)
			{ }
			target.CallParser("hello", users, 0);
			Assert.AreEqual<string>(users.StringProperties["testok"], "hello");
			target.CallParser("hello", null, 0);
		}

		/// <summary>
		///RegisterMethod 的测试
		///</summary>
		[TestMethod()]
		public void RegisterMethodTest()
		{
			MethodsCenter target = new MethodsCenter();
			target.ReadyToRegisterFor("test", this);
			MethodInfo MI = GetType().GetMethod("dummymethod");
			target.RegisterMethod(MI);
			try
			{
				target.RegisterMethod(MI);
				Assert.Fail();
			}
			catch(InvalidOperationException)
			{ }
			int result = (int)target.CallMethod("test", "dummymethod", "bcd", "mmjb");
			Assert.AreEqual<int>(result, 12);
			result = (int)target.CallMethod("test", "dummymethod", new Dictionary<string, object> { { "arg1", "bcd" }, { "arg2", "mmjb" } });
			Assert.AreEqual<int>(result, 12);
			try
			{
				target.CallMethod("test", "dummymethod", "a");
				Assert.Fail();
			}
			catch(ArgumentException)
			{ }
			try
			{
				target.CallMethod("test", "dummymethod", new Dictionary<string, object> { { "arg1", "bcd" } });
				Assert.Fail();
			}
			catch(ArgumentException)
			{ }
			try
			{
				target.CallMethod("test", "dummymethod", new Dictionary<string, object> { { "arg1", 35 } });
				Assert.Fail();
			}
			catch(ArgumentException)
			{ }
			try
			{
				target.CallMethod("test", "nomethod", "a");
				Assert.Fail();
			}
			catch(InvalidOperationException)
			{ }
			try
			{
				target.CallMethod("test", "nomethod", new Dictionary<string, object> { { "arg1", 35 } });
				Assert.Fail();
			}
			catch(InvalidOperationException)
			{ }
		}

		/// <summary>
		///RegisterAction 的测试
		///</summary>
		[TestMethod()]
		public void RegisterActionTest()
		{
			MethodsCenter target = new MethodsCenter(); // TODO: 初始化为适当的值
			ActionPluginCall Call = new ActionPluginCall(dummyaction);
			UserData users = new UserData();
			target.RegisterAction(Call);
			try
			{
				target.RegisterAction(Call);
				Assert.Fail();
			}
			catch(InvalidOperationException)
			{ }
			target.CallAction(null, users, 5);
			Assert.AreEqual<string>(users.StringProperties["testok"], "5");
			target.CallAction(null, null, 5);
		}

		public void dummyparser(string pageData, UserData users, int villageId)
		{
			if(users == null)
				throw new ArgumentNullException();
			users.StringProperties["testok"] = pageData;
		}

		public void dummyaction(ITask task, UserData users, int villageId)
		{
			if(users == null)
				throw new ArgumentNullException();
			users.StringProperties["testok"] = villageId.ToString();
		}

		public int dummymethod(string arg1, string arg2)
		{
			return arg1.Length * arg2.Length;
		}

		#region IPlugin 成员

		public void Initialize(MethodsCenter MC)
		{
			
		}

		#endregion
	}
}
