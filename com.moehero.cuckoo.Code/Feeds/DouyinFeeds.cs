using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Feeds
{
    internal class DouyinFeeds : FeedsBase
    {
        private readonly List<string> _currentVideoList = new List<string>();
        private readonly string _uid;

        public override string Id => "Douyin";

        public override string Name => "抖音";

        public DouyinFeeds(string uid) {
            _uid = uid;
            Init();
        }

        public override async Task<FeedsInfo> CheckUpdate() {
            foreach(var info in await GetVideoInfos()) {
                if(_currentVideoList.Contains(info.Id)) continue;
                _currentVideoList.Add(info.Id);
                return new FeedsInfo {
                    Title = "丧妹新歌出炉啦! 快来听歌点赞分享噢!",
                    Description = info.Description,
                    Url = $"https://www.iesdouyin.com/share/video/{info.Id}/?mid",
                };
            }
            return null;
        }

        private async void Init() {
            _currentVideoList.AddRange(Array.ConvertAll(await GetVideoInfos(), m => m.Id));
        }

        private async Task<VideoInfo[]> GetVideoInfos(int page = 1) {
            var videoInfos = new List<VideoInfo>();
            var s = await Http.GetJson("https://api.anoyi.com/api/signature/ies/" + _uid);
            var r = await Http.GetJson($"https://api.anoyi.com/api/signature/ies/{_uid}/post?s={s["sign"].Value<string>()}");

            foreach(var info in r["aweme_list"]) {
                videoInfos.Add(new VideoInfo { Id = info["aweme_id"].Value<string>(), Description = info["desc"].Value<string>() });
            }
            return videoInfos.ToArray();
        }

        private struct VideoInfo
        {
            public string Id { get; set; }

            public string Description { get; set; }
        }
    }
}
