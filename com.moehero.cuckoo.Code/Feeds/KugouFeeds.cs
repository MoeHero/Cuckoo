using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code.Feeds
{
    internal class KugouFeeds : FeedsBase
    {
        private DateTime _lastPostAt = DateTime.Now;
        private readonly string _singerId;

        public override string Id => "Kugou";

        public override string Name => "酷狗";

        public KugouFeeds(string singerId) {
            _singerId = singerId;
        }

        public override async Task<FeedsInfo> CheckUpdate() {
            var r = await Http.GetJson("http://mobilecdnbj.kugou.com/api/v3/singer/song?sorttype=1&pagesize=5&singerid=" + _singerId);
            foreach(var info in r["data"]["info"]) {
                var postAt = info["publish_date"].Value<DateTime>();
                if(postAt <= _lastPostAt) return null;
                _lastPostAt = postAt;
                return new FeedsInfo {
                    Slogan = GetRandomSlogan(),
                    Title = r["filename"].Value<string>(),
                    Url = $"https://www.kugou.com/song/#hash={info["hash"].Value<string>()}&",
                };
            }
            return null;
        }
    }
}
