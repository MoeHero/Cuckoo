using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Checker
{
    internal class BilibiliVideoChecker : IChecker
    {
        private DateTime lastChecker = new DateTime(2020, 3, 1);

        public async Task<string> Check() {
            var msg = "";
            var r = await Http.GetJson("https://api.bilibili.com/x/space/arc/search?mid=4548018&ps=5&pn=1&order=pubdate");
            if(r["code"].Value<int>() != 0) return null;
            foreach(var v in r["data"]["list"]["vlist"]) {
                var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                var timeStamp = (long)(lastChecker - startTime).TotalSeconds;

                if(v["created"].Value<int>() < timeStamp) break;
                msg += CreateMessage(v["title"].Value<string>(), v["aid"].Value<string>());
            }
            lastChecker = DateTime.Now;
            return msg.Trim();
        }

        private string CreateMessage(string title, string aid) {
            var msg = new StringBuilder();
            msg.AppendLine("丧妹更新投稿啦! 等你来三连支持噢!");
            msg.AppendLine($"《{title}》");
            msg.AppendLine($"https://www.bilibili.com/video/av{aid}");
            return msg.ToString();
        }
    }
}
