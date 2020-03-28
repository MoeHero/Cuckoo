using Native.Sdk.Cqp.Model;
using System.Text;

namespace com.moehero.cuckoo.Code.Function
{
    internal class ListSloganFunction : FunctionBase
    {
        private readonly Group _group;
        private readonly QQ _qq;
        private readonly string _feedsId;

        public ListSloganFunction(Group group, QQ qq, string feedsId) {
            _group = group;
            _qq = qq;
            _feedsId = feedsId;
        }

        public override bool CanRun() => Config.AdminList.Contains(_qq.Id) && FeedsManager.FeedsIds.Contains(_feedsId);

        public override void Run() {
            if(!FeedsBase.Slogans.ContainsKey(_feedsId)) FeedsBase.Slogans.Add(_feedsId, Config.Get<string>(null, $"{_feedsId}Slogans"));
            var slogans = FeedsBase.Slogans[_feedsId];
            if(slogans.Count == 0) {
                _group.SendGroupMessage($"{_feedsId}没有设置任何口号!");
                return;
            }
            var msg = new StringBuilder();
            msg.AppendLine($"{_feedsId}使用以下口号:");
            for(var i = 0; i < slogans.Count; i++) msg.AppendLine($"{i}. {slogans[i]}");
            _group.SendGroupMessage(msg.ToString().Trim());
        }
    }
}
