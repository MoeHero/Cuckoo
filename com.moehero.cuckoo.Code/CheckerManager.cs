using com.moehero.cuckoo.Code.Checker;
using Native.Sdk.Cqp;
using System.Collections.Generic;
using System.Timers;

namespace com.moehero.cuckoo.Code
{
    internal static class CheckerManager
    {
        private static readonly Timer _timer = new Timer(10000);
        private static CQApi _api;

        private static readonly List<IChecker> Checkers = new List<IChecker> {
            //new BilibiliVideoChecker(),
            //new BilibiliLiveChecker(),
            //new KugouChecker("959210"),
            //new KugouChecker("845109"),
            //new KuwoChecker(),
            new NeteaseCloudMusicChecker(),
        };

        internal static void Init(CQApi cqApi) {
            _timer.Elapsed += (sender, e) => RunChecker();
            _timer.Start();

            _api = cqApi;
        }

        private async static void RunChecker() {
            foreach(var checker in Checkers) {
                var msg = await checker.Check();
                if(string.IsNullOrEmpty(msg)) continue;
                SendMessageToEnabledGroup(msg);
            }
        }

        private static void SendMessageToEnabledGroup(string msg) {
            foreach(var group in Config.EnabledGroups) _api.SendGroupMessage(group, msg);
        }
    }
}
