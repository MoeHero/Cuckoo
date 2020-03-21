using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Checker
{
    internal class KugouChecker : IChecker
    {
        private DateTime lastChecker = new DateTime(2019, 12, 1);
        private string _singerId;

        public KugouChecker(string singerId) {
            _singerId = singerId;
        }

        public async Task<string> Check() {
            var msg = "";
            var r = await Http.GetJson($"http://mobilecdnbj.kugou.com/api/v3/singer/song?sorttype=1&pagesize=5&singerid={_singerId}");
            foreach(var info in r["data"]["info"]) {
                if(info["publish_date"].Value<DateTime>() < lastChecker) break;
                msg += CreateMessage(info["filename"].Value<string>(), info["hash"].Value<string>());
            }
            lastChecker = DateTime.Now;
            return msg.Trim();
        }

        private string CreateMessage(string title, string hash) {
            var msg = new StringBuilder();
            msg.AppendLine("丧妹新歌出炉啦! 快来听歌点赞分享噢!");
            msg.AppendLine($"《{title}》");
            msg.AppendLine($"https://www.kugou.com/song/#hash={hash}&");
            return msg.ToString();
        }
    }
}
