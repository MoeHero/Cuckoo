using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Checker
{
    internal class BilibiliLiveChecker : IChecker
    {
        private DateTime lastStartTime = DateTime.MinValue;

        public async Task<string> Check() {
            var r = await Http.GetJson("https://api.live.bilibili.com/xlive/web-room/v1/index/getInfoByRoom?room_id=48499");
            if(r["code"].Value<int>() != 0 || r["data"]["room_info"]["live_status"].Value<int>() != 1) return null;
            var startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            var timeStamp = (long)(lastStartTime - startTime).TotalSeconds;
            if(r["data"]["room_info"]["live_start_time"].Value<long>() <= timeStamp) return null;
            lastStartTime = DateTime.Now;
            return CreateMessage(r["data"]["room_info"]["title"].Value<string>());
        }

        private string CreateMessage(string title) {
            var msg = new StringBuilder();
            msg.AppendLine("丧妹直播啦! 大家快来看直播噢!");
            msg.AppendLine($"《{title}》");
            msg.AppendLine($"https://live.bilibili.com/48499");
            return msg.ToString();
        }
    }
}
