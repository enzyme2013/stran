using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Stran2
{
	public class MethodsCenter
	{
		string nowPluginName;

		Dictionary<string, MethodInfo> Methods = new Dictionary<string, MethodInfo>();
		Dictionary<string, ParserPluginCall> ParserPlugins = new Dictionary<string, ParserPluginCall>();
		Dictionary<string, ActionPluginCall> ActionPlugins = new Dictionary<string, ActionPluginCall>();
		Dictionary<string, IPlugin> PluginObjects = new Dictionary<string, IPlugin>();


		public void RegisterMethod(MethodInfo MI)
		{
			var key = getMethodKey(nowPluginName, MI.Name);
			if(Methods.ContainsKey(key))
				throw new InvalidOperationException(string.Format("Method already exists on loading function {1} from module {0}",
					nowPluginName, MI.Name));
			else
				Methods.Add(key, MI);
		}

		public void RegisterParser(ParserPluginCall Call)
		{
			var key = getMethodKey(nowPluginName, Call.Method.Name);
			if(ParserPlugins.ContainsKey(key))
				throw new InvalidOperationException(string.Format("Parser already exists: function {1} from module {0}",
					nowPluginName, Call.Method.Name));
			else
				ParserPlugins.Add(key, Call);
		}

		public void RegisterAction(ActionPluginCall Call)
		{
			var key = getMethodKey(nowPluginName, Call.Method.Name);
			if(ActionPlugins.ContainsKey(key))
				throw new InvalidOperationException(string.Format("Action already exists: function {1} from module {0}",
					nowPluginName, Call.Method.Name));
			else
				ActionPlugins.Add(key, Call);
		}

		public void ReadyToRegisterFor(string Name, IPlugin PluginObject)
		{
			nowPluginName = Name;
			PluginObjects.Add(Name, PluginObject);
		}

		private string getMethodKey(string PluginName, string MethodName)
		{
			return PluginName + "|" + MethodName;
		}

		public void CallParser(string pageData, UserData users, int villageId)
		{
			foreach(var m in ParserPlugins)
			{
				try
				{
					m.Value.Invoke(pageData, users, villageId);
				}
				catch(Exception)
				{
				}
			}
		}

		public void CallAction(ITask task, UserData users, int villageId)
		{
			foreach(var m in ActionPlugins)
			{
				try
				{
					m.Value.Invoke(task, users, villageId);
				}
				catch(Exception)
				{
				}
			}
		}

		public object CallMethod(string PluginName, string MethodName, params object[] args)
		{
			var key = getMethodKey(nowPluginName, MethodName);
			if(Methods.ContainsKey(key))
			{
				var pinfo = Methods[key].GetParameters();
				if(args.Length != pinfo.Length)
					throw new ArgumentException(string.Format("Parameter count mismatch when calling {0}-{1}",
						PluginName, MethodName));
				return Methods[key].Invoke(PluginObjects[nowPluginName], args);
			}
			else
				throw new InvalidOperationException(string.Format("Function {0}-{1} not found.",
					PluginName, MethodName));
		}

		public object CallMethod(string PluginName, string MethodName, IDictionary<string, object> NamedParameters)
		{
			var key = getMethodKey(nowPluginName, MethodName);
			if(Methods.ContainsKey(key))
			{
				var pinfo = Methods[key].GetParameters();
				object[] args = new object[pinfo.Length];
				for(int i = 0; i < pinfo.Length; i++)
				{
					Type t = pinfo[i].ParameterType;
					if(NamedParameters.ContainsKey(pinfo[i].Name))
					{
						var pobj = NamedParameters[pinfo[i].Name];
						if(t.IsInstanceOfType(pobj))
							args[i] = pobj;
						else
							throw new ArgumentException(string.Format("'{0}' parameter invalid type given when calling {1}-{2}, Type of '{3}' needed.",
								pinfo[i].Name, PluginName, MethodName, t.Name));
					}
					else
						throw new ArgumentException(string.Format("'{0}' parameter miss when calling {1}-{2}, Type of '{3}' needed.",
							pinfo[i].Name, PluginName, MethodName, t.Name));
				}
				return Methods[key].Invoke(PluginObjects[nowPluginName], args);
			}
			else
				throw new InvalidOperationException(string.Format("Function {0}-{1} not found.",
					PluginName, MethodName));
		}

		public bool IsMethodExists(string PluginName, string MethodName)
		{
			var key = getMethodKey(nowPluginName, MethodName);
			if(Methods.ContainsKey(key))
				return true;
			else
				return false;
		}

		public bool IsMethodExists(string PluginName, string MethodName, IList<Type> ParameterTypes)
		{
			var key = getMethodKey(nowPluginName, MethodName);
			if(Methods.ContainsKey(key))
			{
				var pinfo = Methods[key].GetParameters();
				if(ParameterTypes.Count != pinfo.Length)
					return false;
				for(int i = 0; i < pinfo.Length; i++)
				{
					if(pinfo[i].ParameterType.IsInterface)
					{
						if(ParameterTypes[i].IsInterface)
						{
							if(ParameterTypes[i].FullName == pinfo[i].ParameterType.FullName)
								continue;
							else
								return false;
						}
						else
						{
							Type ActualType = ParameterTypes[i].GetInterface(pinfo[i].ParameterType.Name);
							Type NeedType = pinfo[i].ParameterType;
							if(ActualType == null && !ParameterTypes[i].Equals(NeedType))
								return false;
							if(ActualType.IsGenericType && !ActualType.FullName.Equals(NeedType.FullName))
									return false;
						}
					}
					else
						if(!ParameterTypes[i].IsSubclassOf(pinfo[i].ParameterType) && !ParameterTypes[i].Equals(pinfo[i].ParameterType))
							return false;
				}
				return true;
			}
			else
				return false;
		}

		public bool IsMethodExists(string PluginName, string MethodName, IDictionary<string, Type> NamedParameterTypes)
		{
			var key = getMethodKey(nowPluginName, MethodName);
			if(Methods.ContainsKey(key))
			{
				var pinfo = Methods[key].GetParameters();
				if(NamedParameterTypes.Count < pinfo.Length)
					return false;
				for(int i = 0; i < pinfo.Length; i++)
				{
					string k = pinfo[i].Name;
					if(!NamedParameterTypes.ContainsKey(k))
						return false;
					Type t = NamedParameterTypes[k];
					if(pinfo[i].ParameterType.IsInterface)
					{
						if(t.IsInterface)
						{
							if(t.FullName == pinfo[i].ParameterType.FullName)
								continue;
							else
								return false;
						}
						else
						{
							Type ActualType = t.GetInterface(pinfo[i].ParameterType.Name);
							Type NeedType = pinfo[i].ParameterType;
							if(ActualType == null && !t.Equals(NeedType))
								return false;
							if(ActualType.IsGenericType && !ActualType.FullName.Equals(NeedType.FullName))
								return false;
						}
					}
					else
						if(!t.IsSubclassOf(pinfo[i].ParameterType) && !t.Equals(pinfo[i].ParameterType))
							return false;
				}
				return true;
			}
			else
				return false;
		}

	}
}
