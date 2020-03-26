using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Feeds
{
    internal class KuwoFeeds : FeedsBase
    {
        private readonly List<string> _currentMusicList = new List<string>();
        private readonly string _singerId;

        public override string Id => "Kuwo";

        public override string Name => "酷我";

        public KuwoFeeds(string singerId) {
            _singerId = singerId;
            Init();
        }

        public override async Task<FeedsInfo> CheckUpdate() {
            foreach(var info in await GetMusicInfos()) {
                if(_currentMusicList.Contains(info.Id)) continue;
                _currentMusicList.Add(info.Id);
                return new FeedsInfo {
                    Title = "丧妹新歌出炉啦! 快来听歌点赞分享噢!",
                    Description = info.Name,
                    Url = "http://www.kuwo.cn/play_detail/" + info.Id,
                };
            }
            return null;
        }

        private async void Init() {
            _currentMusicList.AddRange(Array.ConvertAll(await GetMusicInfos(), m => m.Id));
        }

        private async Task<MusicInfo[]> GetMusicInfos(int page = 1) {
            var musicInfos = new List<MusicInfo>();

            var cookie = new CookieContainer();
            await Http.Get("http://www.kuwo.cn/singer_detail/" + _singerId, cookie);

            var req = (HttpWebRequest)WebRequest.Create($"http://www.kuwo.cn/api/www/artist/artistMusic?artistid={_singerId}&pn={page}&rn=100");
            req.Proxy = null;
            req.KeepAlive = false;
            req.CookieContainer = cookie;
            req.Headers.Add("csrf", cookie.GetCookies(new Uri("http://www.kuwo.cn"))?["kw_token"]?.Value);

            var response = await req.GetResponseAsync();
            var streamReader = new StreamReader(response.GetResponseStream());
            var r = JObject.Parse(await streamReader.ReadToEndAsync());
            streamReader.Close();

            foreach(var info in r["data"]["list"]) {
                musicInfos.Add(new MusicInfo { Id = info["rid"].Value<string>(), Name = info["name"].Value<string>() });
            }
            return musicInfos.ToArray();
        }

        private struct MusicInfo
        {
            public string Id { get; set; }

            public string Name { get; set; }
        }
    }
}
