using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Feeds
{
    internal class NeteaseCloudMusicFeeds : FeedsBase
    {
        private readonly List<string> _currentMusicList = new List<string>();
        private readonly string _singerId;

        public override string Id => "NeteaseCloud";

        public override string Name => "网易云音乐";

        public NeteaseCloudMusicFeeds(string singerId) {
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
                    Url = "https://music.163.com/song/" + info.Id,
                };
            }
            return null;
        }

        private async void Init() {
            _currentMusicList.AddRange(Array.ConvertAll(await GetMusicInfos(), m => m.Id));
        }

        private async Task<MusicInfo[]> GetMusicInfos(int page = 1) {
            var musicInfos = new List<MusicInfo>();

            var r = await Http.Get("https://music.163.com/artist/" + _singerId);
            foreach(Match m in Regex.Matches(r, @"song\?id=(\d+)"">(.+?)<", RegexOptions.Multiline)) {
                musicInfos.Add(new MusicInfo { Id = m.Groups[1].Value, Name = m.Groups[2].Value });
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
