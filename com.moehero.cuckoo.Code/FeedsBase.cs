using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code
{
    internal abstract class FeedsBase
    {
        public static Dictionary<string, ObservableCollection<string>> Slogans = new Dictionary<string, ObservableCollection<string>>();

        public abstract string Id { get; }

        /// <summary>
        /// 源名称
        /// </summary>
        public virtual string Name { get; } = "";

        /// <summary>
        /// 检查更新
        /// </summary>
        /// <returns>更新消息</returns>
        public abstract Task<FeedsInfo> CheckUpdate();

        protected string GetRandomSlogan() {
            if(!Slogans.ContainsKey(Id)) Slogans.Add(Id, Config.Get<string>(null, $"{Id}Slogans"));
            var slogans = Slogans[Id];
            if(slogans.Count == 0) return "丧妹更新啦!";
            return slogans[new Random().Next(slogans.Count)];
        }
    }

    internal class FeedsInfo
    {
        public string Slogan { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public Image Image { get; set; }

        public override string ToString() {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Slogan);
            stringBuilder.AppendLine($"《{Title}》");
            stringBuilder.AppendLine(Url);
            //TODO Image
            return stringBuilder.ToString();
        }
    }
}
