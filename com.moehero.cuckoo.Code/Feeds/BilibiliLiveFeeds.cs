using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Feeds
{
    internal class BilibiliLiveFeeds : FeedsBase
    {
        private long _lastStartAt = long.MinValue;
        private readonly string _roomId;

        public override string Id => "BilibiliLive";

        public override string Name => "哔哩哔哩直播";

        public BilibiliLiveFeeds(string roomId) {
            _roomId = roomId;
            Init();
        }

        public override async Task<FeedsInfo> CheckUpdate() {
            var info = await GetLiveInfo();
            if(!info.IsLive) return null;
            if(info.StartTime <= _lastStartAt) return null;
            _lastStartAt = info.StartTime;
            return new FeedsInfo {
                Slogan = GetRandomSlogan(),
                Title = info.Title,
                Url = "https://live.bilibili.com/" + _roomId,
            };
        }

        private async void Init() {
            var info = await GetLiveInfo();
            if(info.IsLive) _lastStartAt = info.StartTime;
        }

        private async Task<LiveInfo> GetLiveInfo() {
            var r = await Http.GetJson("https://api.live.bilibili.com/xlive/web-room/v1/index/getInfoByRoom?room_id=" + _roomId);
            return new LiveInfo {
                IsLive = r["data"]["room_info"]["live_status"].Value<int>() == 1,
                StartTime = r["data"]["room_info"]["live_start_time"].Value<long>(),
                Title = r["data"]["room_info"]["title"].Value<string>(),
            };
        }

        private struct LiveInfo
        {
            public bool IsLive { get; set; }

            public long StartTime { get; set; }

            public string Title { get; set; }
        }
    }
}
