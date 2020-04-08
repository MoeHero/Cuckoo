using com.moehero.cuckoo.Code.Feeds;
using Native.Sdk.Cqp;
using System.Collections.Generic;
using System.Timers;
using System.Linq;
using System;

namespace com.moehero.cuckoo.Code
{
    internal static class FeedsManager
    {
        private const int CHECKER_INTERVAL = 60 * 1000;

        public static List<string> FeedsIds = new List<string>();


        private static readonly List<FeedsBase> _feedsList = new List<FeedsBase> {
            new BilibiliLiveFeeds("48499"),
            new BilibiliVideoFeeds("4548018"),
            new DouyinFeeds("97244550128"),
            new KugouFeeds("959210"),
            new KugouFeeds("845109"),
            new KuwoFeeds("4185939"),
            new NeteaseCloudMusicFeeds("31440981"),
            new QQMusicFeeds("000Sd6U5452rG5"),
        };
        private static readonly Timer _timer = new Timer(CHECKER_INTERVAL);
        private static CQApi _api;

        internal static void Init(CQApi cqApi) {
            _api = cqApi;

            FeedsIds.AddRange(from f in _feedsList select f.Id);
            _timer.Elapsed += (sender, e) => CheckUpdates();
            _timer.Start();
        }

        private async static void CheckUpdates() {
            foreach(var feeds in _feedsList) {
                var feedsInfo = await feeds.CheckUpdate();
                if(feedsInfo == null) continue;
                SendMessageToEnabledGroup(feedsInfo.ToString());
            }
        }

        private static void SendMessageToEnabledGroup(string msg) {
            foreach(var group in Config.EnabledGroups) _api.SendGroupMessage(group, CQApi.CQCode_AtAll() + Environment.NewLine, msg);
        }
    }
}
