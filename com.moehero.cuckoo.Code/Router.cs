using com.moehero.cuckoo.Code.Function;
using Native.Sdk.Cqp;
using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Group = Native.Sdk.Cqp.Model.Group;

namespace com.moehero.cuckoo.Code
{
    internal class Router
    {
        /// <summary>
        /// 群路由列表
        /// </summary>
        public static readonly List<Route> GroupRoutes = new List<Route> {
            new Route { URI = "启用Cuckoo", Function = typeof(EnableFunction), IgnoreDisable = true },
            new Route { URI = "停用Cuckoo", Function = typeof(DisableFunction) },
            new Route { URI = "本群启用", Function = typeof(GroupEnableFunction), IgnoreDisable = true },
            new Route { URI = "本群停用", Function = typeof(GroupDisableFunction) },
            new Route { URI = "添加管理员{AdminQQ}", Function = typeof(AddAdminFunction) },
            new Route { URI = "移除管理员{AdminQQ}", Function = typeof(RemoveAdminFunction) },
            new Route { URI = "添加口号{FeedsId}{Slogan}", Function = typeof(AddSloganFunction) },
            new Route { URI = "删除口号{FeedsId}{SloganOrIndex}", Function = typeof(RemoveSloganFunction) },
            new Route { URI = "口号列表{FeedsId}", Function = typeof(ListSloganFunction) },
        };

        public static void Execute(CQGroupMessageEventArgs e) {
            foreach(var route in GroupRoutes) {
                //判断路由是否可以执行
                if((!Config.Enabled || !Config.EnabledGroups.Contains(e.FromGroup.Id)) && !route.IgnoreDisable) continue;
                //判断路由是否匹配当前消息
                if(!route.IsMatch(e.Message.Text)) continue;
                //获得功能类实例
                var function = route.GetFunctionInstance(e);
                //判断可以运行后运行
                if(function?.CanRun() == true) function.Run();
                if(function?.Handled == true) return;
            }
        }
    }

    internal class Route
    {
        private static readonly char[] _separator = new[] { ' ', ',' };
        private Type _functions;

        public string URI { get; set; }

        public Type Function {
            get => _functions;
            set {
                if(value.IsInstanceOfType(typeof(FunctionBase))) throw new InvalidOperationException("Function指定的类必须继承自FunctionBase");
                if(value.GetConstructors().Length != 1) throw new Exception("Function指定的类只能有1个构造函数");
                _functions = value;
            }
        }

        public bool OnlyPrivate { get; set; } = false;

        public bool OnlyGroup { get; set; } = false;

        public bool IgnoreDisable { get; set; } = false;

        public bool IsMatch(string message) {
            var endIndex = URI.IndexOf("{");
            if(endIndex == -1) endIndex = URI.Length - 1;
            return message.StartsWith(URI.Substring(0, endIndex));
        }

        public FunctionBase GetFunctionInstance(CQGroupMessageEventArgs e) {
            var parameterDictionary = GetParameterDictionary(e.Message.Text);
            if(parameterDictionary == null) return null;

            //获取构造函数并按照参数列表指定参数
            var parameters = new List<object>();
            var constructor = Function.GetConstructors()[0];
            foreach(var p in constructor.GetParameters()) {
                if(p.ParameterType == typeof(Group)) parameters.Add(e.FromGroup);
                else if(p.ParameterType == typeof(QQ)) parameters.Add(e.FromQQ);
                else if(p.ParameterType == typeof(QQMessage)) parameters.Add(e.Message);
                else if(p.ParameterType == typeof(CQApi)) parameters.Add(e.CQApi);
                else if(p.ParameterType == typeof(CQLog)) parameters.Add(e.CQLog);
                else if(p.ParameterType == typeof(CQGroupMessageEventArgs)) parameters.Add(e);
                else if(p.ParameterType == typeof(string) && parameterDictionary.TryGetValue(p.Name.ToLower(), out string value)) parameters.Add(value);
                else parameters.Add(null);
            }
            //调用构造函数 返回实例
            return (FunctionBase)constructor.Invoke(parameters.ToArray());
        }

        private Dictionary<string, string> GetParameterDictionary(string message) {
            var result = new Dictionary<string, string>();
            //寻找{判断是否存在参数
            var startIndex = URI.IndexOf("{");
            if(startIndex == -1) return result;
            //将AT替换为对应QQ号
            message = new Regex("\\[CQ:at,\\s*qq=(.+?)\\]").Replace(message, " $1 ");
            //解析消息中的参数列表
            var msg_param = message.Substring(startIndex).Split(_separator, StringSplitOptions.RemoveEmptyEntries);
            if(msg_param.Length == 0) return null;
            //解析URI中的参数列表
            var uri_param = Regex.Matches(URI, @"{(.+?)}");
            var index = 1;
            foreach(Match m in uri_param) {
                var p = m.Groups[1].Value;
                //如果消息的参数列表全部循环完就直接返回
                if(msg_param.Length < index) return result;
                result.Add(p.ToLower(), msg_param[index - 1]);
                index++;
            }
            return result;
        }
    }
}
