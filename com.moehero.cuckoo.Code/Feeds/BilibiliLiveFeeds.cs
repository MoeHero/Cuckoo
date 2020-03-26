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
        }

        public override async Task<FeedsInfo> CheckUpdate() {
            var r = await Http.GetJson("https://api.live.bilibili.com/xlive/web-room/v1/index/getInfoByRoom?room_id=" + _roomId);
            if(r["code"].Value<int>() != 0 || r["data"]["room_info"]["live_status"].Value<int>() != 1) return null;
            var startAt = r["data"]["room_info"]["live_start_time"].Value<long>();
            if(startAt <= _lastStartAt) return null;
            _lastStartAt = startAt;
            return new FeedsInfo {
                Title = "丧妹直播啦! 大家快来看直播噢!",
                Description = r["data"]["room_info"]["title"].Value<string>(),
                Url = "https://live.bilibili.com/" + _roomId,
            };
        }
    }
}
