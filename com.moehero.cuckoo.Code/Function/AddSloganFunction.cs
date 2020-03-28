using Native.Sdk.Cqp.Model;

namespace com.moehero.cuckoo.Code.Function
{
    internal class AddSloganFunction : FunctionBase
    {
        private readonly Group _group;
        private readonly QQ _qq;
        private readonly string _feedsId;
        private readonly string _slogan;

        public AddSloganFunction(Group group, QQ qq, string feedsId, string slogan) {
            _group = group;
            _qq = qq;
            _feedsId = feedsId;
            _slogan = slogan;
        }

        public override bool CanRun() => Config.AdminList.Contains(_qq.Id) && FeedsManager.FeedsIds.Contains(_feedsId);

        public override void Run() {
            if(!FeedsBase.Slogans.ContainsKey(_feedsId)) FeedsBase.Slogans.Add(_feedsId, Config.Get<string>(null, $"{_feedsId}Slogans"));
            var slogans = FeedsBase.Slogans[_feedsId];
            if(slogans.Contains(_slogan)) return;
            slogans.Add(_slogan);
            _group.SendGroupMessage("添加成功!");
        }
    }
}
