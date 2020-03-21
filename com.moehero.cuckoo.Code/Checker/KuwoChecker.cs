using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Checker
{
    internal class KuwoChecker : IChecker
    {
        private readonly List<string> _currentMusicList = new List<string>();

        public KuwoChecker() {
            Init();
        }

        public async Task<string> Check() {
            var msg = "";
            foreach(var info in await GetMusicInfos()) {
                if(_currentMusicList.Contains(info.Id)) continue;
                msg += CreateMessage(info);
                _currentMusicList.Add(info.Id);
            }
            return msg.Trim();
        }

        private string CreateMessage(MusicInfo info) {
            var msg = new StringBuilder();
            msg.AppendLine("丧妹新歌出炉啦! 快来听歌点赞分享噢!");
            msg.AppendLine($"《{info.Name}》");
            msg.AppendLine($"http://www.kuwo.cn/play_detail/{info.Id}");
            return msg.ToString();
        }

        private async void Init() {
            _currentMusicList.AddRange(Array.ConvertAll(await GetMusicInfos(), m => m.Id));
        }

        private async Task<MusicInfo[]> GetMusicInfos(int page = 1) {
            var musicInfos = new List<MusicInfo>();

            var cookie = new CookieContainer();
            await Http.Get("http://www.kuwo.cn/singer_detail/4185939", cookie);

            var req = (HttpWebRequest)WebRequest.Create($"http://www.kuwo.cn/api/www/artist/artistMusic?artistid=4185939&pn={page}&rn=100&reqId=da431400-6b7b-11ea-838a-659117e7515a");
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
