using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Feeds
{
    internal class BilibiliVideoFeeds : FeedsBase
    {
        private long _lastPostAt;
        private readonly string _uid;

        public override string Id => "BilibiliVideo";

        public override string Name => "哔哩哔哩";

        public BilibiliVideoFeeds(string uid) {
            _uid = uid;
            _lastPostAt = GetTimestamp(DateTime.Now);
        }

        public override async Task<FeedsInfo> CheckUpdate() {
            var r = await Http.GetJson($"https://api.bilibili.com/x/space/arc/search?mid={_uid}&ps=5&pn=1&order=pubdate");
            if(r["code"].Value<int>() != 0) return null;
            foreach(var info in r["data"]["list"]["vlist"]) {
                var postAt = info["created"].Value<long>();
                if(postAt <= _lastPostAt) return null;
                _lastPostAt = postAt;
                return new FeedsInfo {
                    Title = "丧妹更新投稿啦! 等你来三连支持噢!",
                    Description = r["title"].Value<string>(),
                    Url = "https://www.bilibili.com/video/av" + info["aid"].Value<string>(),
                };
            }
            return null;
        }

        private long GetTimestamp(DateTime dateTime) {
            var date1970 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (long) (dateTime - date1970).TotalSeconds;
        }
}
}
