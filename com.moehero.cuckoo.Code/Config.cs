using Native.Tool.IniConfig.Linq;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace com.moehero.cuckoo.Code
{
    //TODO 重构
    internal static class Config
    {
        private static IniSection ini;
        private static IniObject iniObject;

        private static void InitConfig() {
            var path = AppDirectory + "Config.ini";
            if(!File.Exists(path)) File.WriteAllText(path, "");
            iniObject = IniObject.Load(path);
            ini = iniObject.Find(s => s.Name == "Application") ?? new IniSection("Application");

            EnabledGroups = GetValue<long>("EnabledGroups");
        }

        public static long OwnerNumber { get; } = 562416714;

        private static string appDirectory;

        public static string AppDirectory {
            get => appDirectory;
            set {
                appDirectory = value;
                InitConfig();
            }
        }

        /// <summary>
        /// 机器人开关
        /// </summary>
        internal static bool Enabled {
            get => GetValue(true);
            set => SetValue(value);
        }

        private static ObservableCollection<long> enabledGruops;

        /// <summary>
        /// 群启用列表
        /// </summary>
        internal static ObservableCollection<long> EnabledGroups {
            get {
                if(enabledGruops == null) EnabledGroups = new ObservableCollection<long>();
                return enabledGruops;
            }
            set {
                if(enabledGruops != null) enabledGruops.CollectionChanged -= CollectionChanged;
                enabledGruops = value;
                SetValue(enabledGruops, "EnabledGroups");
                enabledGruops.CollectionChanged += CollectionChanged;

                void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
                    SetValue(enabledGruops, "EnabledGroups");
                }
            }
        }

        private static ObservableCollection<long> admins;

        /// <summary>
        /// 管理员列表
        /// </summary>
        internal static ObservableCollection<long> Admins {
            get {
                if(admins == null) Admins = new ObservableCollection<long>();
                return admins;
            }
            set {
                if(admins != null) admins.CollectionChanged -= CollectionChanged;
                admins = value;
                SetValue(admins, "Admins");
                admins.CollectionChanged += CollectionChanged;

                void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
                    SetValue(admins, "Admins");
                }
            }
        }

        #region 获取&设置

        private static string GetValue(string defaultValue = "", [CallerMemberName] string key = "") {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            return ini[key]?.ToString() ?? defaultValue;
        }

        private static bool GetValue(bool defaultValue = false, [CallerMemberName] string key = "") {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            if(!bool.TryParse(GetValue("", key), out bool value)) return defaultValue;
            return value;
        }

        private static long GetValue(long defaultValue = 0, [CallerMemberName] string key = "") {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            if(!long.TryParse(GetValue("", key), out long value)) return defaultValue;
            return value;
        }

        private static ushort GetValue(ushort defaultValue = 0, [CallerMemberName] string key = "") {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            if(!ushort.TryParse(GetValue("", key), out ushort value)) return defaultValue;
            return value;
        }

        private static ObservableCollection<T> GetValue<T>(string key) {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            var value = GetValue("", key).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if(value.Length == 0) return new ObservableCollection<T>();
            var values = Array.ConvertAll(value, v => (T)Convert.ChangeType(v, typeof(T)));
            return new ObservableCollection<T>(values);
        }

        private static void SetValue(string value, [CallerMemberName] string key = "") {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            if(ini.ContainsKey(key)) ini[key] = new IniValue(value);
            else ini.Add(key, value);
            //保存
            if(iniObject.Exists(s => s.Name == "Application")) iniObject["Application"] = ini;
            else iniObject.Add(ini);
            iniObject.Save();
        }

        private static void SetValue(bool value, [CallerMemberName] string key = "") {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            SetValue(value.ToString(), key);
        }

        private static void SetValue(long value, [CallerMemberName] string key = "") {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            SetValue(value.ToString(), key);
        }

        private static void SetValue(ushort value, [CallerMemberName] string key = "") {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            SetValue(value.ToString(), key);
        }

        private static void SetValue<T>(ObservableCollection<T> value, string key) {
            if(string.IsNullOrEmpty(key)) throw new NotImplementedException("动态获取Key失败");
            SetValue(value.Aggregate("", (current, p) => $"{current}{p},").TrimEnd(','), key);
        }

        #endregion
    }
}
