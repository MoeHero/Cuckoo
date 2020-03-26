using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code
{
    internal abstract class FeedsBase
    {
        public abstract string Id { get; }

        /// <summary>
        /// 源名称
        /// </summary>
        public virtual string Name { get; } = "";

        /// <summary>
        /// 检查更新
        /// </summary>
        /// <returns>更新消息</returns>
        public abstract Task<FeedsInfo> CheckUpdate();

        private string GetRandomTitle() {
            return "";
        }
    }

    internal class FeedsInfo
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public Image Image { get; set; }

        public override string ToString() {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(Title);
            stringBuilder.AppendLine($"《{Description}》");
            stringBuilder.AppendLine(Url);
            //TODO Image
            return stringBuilder.ToString();
        }
    }
}
