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

		public int dummymethod2(IList<int> data, IPlugin iplugin)
		{
			return data[0];
		}

		public int dummymethod3(Exception e)
		{
			return -1;
		}

		#region IPlugin 成员

		public void Initialize(MethodsCenter MC)
		{

		}

		#endregion


		/// <summary>
		///RegisterParser 的测试
		///</summary>
		[TestMethod()]
		public void ParserTest()
		{
			MethodsCenter target = MethodsCenter.Instance; // TODO: 初始化为适当的值
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
			Assert.AreEqual<int>(target.ParserCount, 1);
			target.CallParser("hello", users, 0);
			Assert.AreEqual<string>(users.StringProperties["testok"], "hello");
			target.CallParser("hello", null, 0);
		}

		/// <summary>
		///RegisterAction 的测试
		///</summary>
		[TestMethod()]
		public void ActionTest()
		{
			MethodsCenter target = MethodsCenter.Instance; // TODO: 初始化为适当的值
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
			Assert.AreEqual<int>(target.ActionCount, 1);
			target.CallAction(null, users, 5);
			Assert.AreEqual<string>(users.StringProperties["testok"], "5");
			target.CallAction(null, null, 5);
		}

		/// <summary>
		///RegisterMethod 的测试
		///</summary>
		[TestMethod()]
		public void MethodTest()
		{
			MethodsCenter target = MethodsCenter.Instance;
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
			Assert.AreEqual<int>(target.MethodsCount, 1);
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
		///IsMethodExists 的测试
		///</summary>
		[TestMethod()]
		public void IsMethodExistsTest()
		{
			MethodsCenter target = MethodsCenter.Instance; // TODO: 初始化为适当的值
			target.ReadyToRegisterFor("test", this);
			target.RegisterMethod(GetType().GetMethod("dummymethod"));
			target.RegisterMethod(GetType().GetMethod("dummymethod2"));
			target.RegisterMethod(GetType().GetMethod("dummymethod3"));
			string PluginName = "test"; // TODO: 初始化为适当的值
			string MethodName = "dummymethod"; // TODO: 初始化为适当的值
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName), true);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(string), typeof(string) }), true);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(string), typeof(int) }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(string) }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "arg2", typeof(string) }, { "dummy", typeof(int) }, { "arg1", typeof(string) } }), true);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "arg2", typeof(int) }, { "arg1", typeof(string) } }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "arg2", typeof(string) } }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "arg2", typeof(string) }, { "dummy", typeof(int) } }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, "what"), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, "what", new Type[] { typeof(string) }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, "what", new Dictionary<string, Type> { { "arg2", typeof(string) } }), false);
			MethodName += "2";
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName), true);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(List<int>), typeof(IPlugin) }), true);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(int[]), typeof(IPlugin) }), true);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(int), typeof(IPlugin) }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(string[]), typeof(IPlugin) }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(int[]), typeof(ITask) }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(int[]), typeof(MethodsCenterTest) }), true);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "data", typeof(int[]) }, { "iplugin", typeof(IPlugin) } }), true);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "data", typeof(int) }, { "iplugin", typeof(IPlugin) } }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "data", typeof(string[]) }, { "iplugin", typeof(IPlugin) } }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "data", typeof(int[]) }, { "iplugin", typeof(ITask) } }), false);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "data", typeof(int[]) }, { "iplugin", typeof(MethodsCenterTest) } }), true);
			MethodName = "dummymethod3";
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Type[] { typeof(InvalidOperationException) }), true);
			Assert.AreEqual(target.IsMethodExists(PluginName, MethodName, new Dictionary<string, Type> { { "e", typeof(InvalidOperationException) } }), true);
		}
	}
}
