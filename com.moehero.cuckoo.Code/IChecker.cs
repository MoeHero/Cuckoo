using System.Threading.Tasks;

namespace com.moehero.cuckoo.Code
{
    internal interface IChecker
    {
        Task<string> Check();
    }
}
