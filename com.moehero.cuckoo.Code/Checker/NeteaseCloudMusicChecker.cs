using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Checker
{
    internal class NeteaseCloudMusicChecker : IChecker
    {
        private readonly List<string> _currentMusicList = new List<string>();

        public NeteaseCloudMusicChecker() {
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
            msg.AppendLine($"https://music.163.com/song?id={info.Id}");
            return msg.ToString();
        }

        private async void Init() {
            _currentMusicList.AddRange(Array.ConvertAll(await GetMusicInfos(), m => m.Id));

            _currentMusicList.RemoveAt(0);
        }

        private async Task<MusicInfo[]> GetMusicInfos(int page = 1) {
            var musicInfos = new List<MusicInfo>();

            var r = await Http.Get("https://music.163.com/artist?id=31440981");
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
