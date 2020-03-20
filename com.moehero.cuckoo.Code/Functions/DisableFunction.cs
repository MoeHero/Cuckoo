﻿using Native.Sdk.Cqp.Model;

namespace com.moehero.cuckoo.Code.Functions
{
    internal class DisableFunction : BaseFunction
    {
        private readonly Group _group;
        private readonly QQ _qq;

        public DisableFunction(Group group, QQ qq) {
            _group = group;
            _qq = qq;
        }

        public override bool CanRun() => _qq == Config.OwnerNumber;

        public override void Run() {
            if(!Config.Enabled) return;
            Config.Enabled = false;
            _group.SendGroupMessage("机器人已停用!");
        }
    }
}