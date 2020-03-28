using Native.Tool.IniConfig.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace com.moehero.cuckoo.Code
{
    internal static class Config
    {
        private static IniSection _config;
        private static IniObject _iniFile;

        private static string _appDirectory;
        private static ObservableCollection<long> _enabledGruops;
        private static ObservableCollection<long> _adminList;

        private static void InitConfig() {
            var path = AppDirectory + "Config.ini";
            if(!File.Exists(path)) File.WriteAllText(path, "");
            _iniFile = IniObject.Load(path);
            _config = _iniFile.Find(s => s.Name == "Application") ?? new IniSection("Application");
        }

        public static long OwnerNumber { get; } = 562416714;

        public static string AppDirectory {
            get => _appDirectory;
            set {
                _appDirectory = value;
                InitConfig();
            }
        }

        /// <summary>
        /// 机器人开关
        /// </summary>
        internal static bool Enabled {
            get => Get(true);
            set => Set(value);
        }

        /// <summary>
        /// 群启用列表
        /// </summary>
        internal static ObservableCollection<long> EnabledGroups {
            get => _enabledGruops ?? (_enabledGruops = Get<long>());
        }

        /// <summary>
        /// 管理员列表
        /// </summary>
        internal static ObservableCollection<long> AdminList {
            get => _adminList ?? (_adminList = Get<long>());
        }

        #region Get & Set

        internal static string Get(string defaultValue = "", [CallerMemberName] string key = "") {
            var value = _config[key];
            if(value == IniValue.Empty) return defaultValue;
            return value.ToString();
        }

        internal static bool Get(bool defaultValue = false, [CallerMemberName] string key = "") {
            var value = Get(defaultValue.ToString(), key);
            if(!bool.TryParse(value, out bool v)) return defaultValue;
            return v;
        }

        internal static long Get(long defaultValue = 0, [CallerMemberName] string key = "") {
            var value = Get(defaultValue.ToString(), key);
            if(!long.TryParse(value, out long v)) return defaultValue;
            return v;
        }

        internal static ushort Get(ushort defaultValue = 0, [CallerMemberName] string key = "") {
            var value = Get(defaultValue.ToString(), key);
            if(!ushort.TryParse(value, out ushort v)) return defaultValue;
            return v;
        }

        internal static ObservableCollection<T> Get<T>(T[] defaultValue = null, [CallerMemberName] string key = "") {
            ObservableCollection<T> result;
            var value = Get("", key).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if(value.Length == 0) {
                result = new ObservableCollection<T>(defaultValue ?? new T[0]);
                result.CollectionChanged += (sender, e) => Set(result, key);
                return result;
            }
            var values = Array.ConvertAll(value, v => (T)Convert.ChangeType(v, typeof(T)));
            result = new ObservableCollection<T>(values);
            result.CollectionChanged += (sender, e) => Set(result, key);
            return result;
        }

        internal static void Set(string value, [CallerMemberName] string key = "") {
            _config[key] = new IniValue(value);
            _iniFile.Exists(s => s.Name == "Application");
            //保存
            if(_iniFile.Exists(s => s.Name == "Application")) _iniFile["Application"] = _config;
            else _iniFile.Add(_config);
            _iniFile.Save();
        }

        internal static void Set(bool value, [CallerMemberName] string key = "") => Set(value.ToString(), key);

        internal static void Set(long value, [CallerMemberName] string key = "") => Set(value.ToString(), key);

        internal static void Set(ushort value, [CallerMemberName] string key = "") => Set(value.ToString(), key);

        internal static void Set<T>(ObservableCollection<T> value, string key) {
            Set(value.Aggregate("", (current, p) => $"{current}{p},").TrimEnd(','), key);
        }

        #endregion
    }
}
