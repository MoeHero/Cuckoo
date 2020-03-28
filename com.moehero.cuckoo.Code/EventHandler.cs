using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;

namespace com.moehero.cuckoo.Code
{
    public class EventHandler :
        IAppEnable,
        IGroupMessage
    {
        public void AppEnable(object sender, CQAppEnableEventArgs e) {
            Config.AppDirectory = e.CQApi.AppDirectory;
            FeedsManager.Init(e.CQApi);
        }

        public void GroupMessage(object sender, CQGroupMessageEventArgs e) {
            Router.Execute(e);
        }
    }
}
