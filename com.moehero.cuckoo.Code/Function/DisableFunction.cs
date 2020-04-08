﻿using Native.Sdk.Cqp.Model;

namespace com.moehero.cuckoo.Code.Function
{
    internal class DisableFunction : FunctionBase
    {
        private readonly Group _group;
        private readonly QQ _qq;

        public DisableFunction(Group group, QQ qq) {
            _group = group;
            _qq = qq;
        }

        public override bool CanRun() => Config.AdminList.Contains(_qq.Id);

        public override void Run() {
            if(!Config.Enabled) return;
            Config.Enabled = false;
            _group.SendGroupMessage("机器人已停用!");
        }
    }
}
