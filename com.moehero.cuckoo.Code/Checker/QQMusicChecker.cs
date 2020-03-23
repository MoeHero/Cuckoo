using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Checker
{
    internal class QQMusicChecker : IChecker
    {
        private DateTime lastChecker = new DateTime(2019, 12, 1);

        public async Task<string> Check() {
            var msg = "";
            var r = await Http.GetJson($"https://u.y.qq.com/cgi-bin/musicu.fcg?format=json&inCharset=utf8&outCharset=utf-8&data=%7B%22singerSongList%22%3A%7B%22method%22%3A%22GetSingerSongList%22%2C%22param%22%3A%7B%22singerMid%22%3A%22000Sd6U5452rG5%22%7D%2C%22module%22%3A%22musichall.song_list_server%22%7D%7D");
            foreach(var i in r["singerSongList"]["data"]["songList"]) {
                var info = i["songInfo"];
                if(info["time_public"].Value<DateTime>() < lastChecker) break;
                msg += CreateMessage(info["title"].Value<string>(), info["mid"].Value<string>());
            }
            lastChecker = DateTime.Now;
            return msg.Trim();
        }

        private string CreateMessage(string title, string id) {
            var msg = new StringBuilder();
            msg.AppendLine("丧妹新歌出炉啦! 快来听歌点赞分享噢!");
            msg.AppendLine($"《{title}》");
            msg.AppendLine($"https://y.qq.com/n/yqq/song/{id}.html");
            return msg.ToString();
        }
    }
}
