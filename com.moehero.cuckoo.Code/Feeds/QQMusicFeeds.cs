using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Feeds
{
    internal class QQMusicFeeds : FeedsBase
    {
        private DateTime _lastPostAt = DateTime.Now;
        private readonly string _singerId;

        public override string Id => "QQMusic";

        public override string Name => "QQ音乐";

        public QQMusicFeeds(string singerId) {
            _singerId = singerId;
        }

        public override async Task<FeedsInfo> CheckUpdate() {
            var r = await Http.GetJson($"https://u.y.qq.com/cgi-bin/musicu.fcg?format=json&inCharset=utf8&outCharset=utf-8&data=%7B%22singerSongList%22%3A%7B%22method%22%3A%22GetSingerSongList%22%2C%22param%22%3A%7B%22singerMid%22%3A%22{_singerId}%22%7D%2C%22module%22%3A%22musichall.song_list_server%22%7D%7D");
            foreach(var i in r["singerSongList"]?["data"]?["songList"]) {
                var info = i["songInfo"];
                var postAt = info["time_public"].Value<DateTime>();
                if(postAt <= _lastPostAt) return null;
                _lastPostAt = postAt;
                return new FeedsInfo {
                    Slogan = GetRandomSlogan(),
                    Title = info["title"].Value<string>(),
                    Url = $"https://y.qq.com/n/yqq/song/{info["mid"].Value<string>()}.html",
                };
            }
            return null;
        }
    }
}
