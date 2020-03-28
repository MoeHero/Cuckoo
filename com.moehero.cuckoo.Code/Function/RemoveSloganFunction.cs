using Native.Sdk.Cqp.Model;

namespace com.moehero.cuckoo.Code.Function
{
    internal class RemoveSloganFunction : FunctionBase
    {
        private readonly Group _group;
        private readonly QQ _qq;
        private readonly string _feedsId;
        private readonly string _sloganOrIndex;

        public RemoveSloganFunction(Group group, QQ qq, string feedsId, string sloganOrIndex) {
            _group = group;
            _qq = qq;
            _feedsId = feedsId;
            _sloganOrIndex = sloganOrIndex;
        }

        public override bool CanRun() => Config.AdminList.Contains(_qq.Id) && FeedsManager.FeedsIds.Contains(_feedsId);

        public override void Run() {
            if(!FeedsBase.Slogans.ContainsKey(_feedsId)) FeedsBase.Slogans.Add(_feedsId, Config.Get<string>(null, $"{_feedsId}Slogans"));
            var slogans = FeedsBase.Slogans[_feedsId];
            if(int.TryParse(_sloganOrIndex, out int i)) {
                slogans.RemoveAt(i);
            } else {
                var slogan = _sloganOrIndex;
                if(!slogans.Contains(slogan)) return;
                slogans.Remove(slogan);
            }
            _group.SendGroupMessage("删除成功!");
        }
    }
}
