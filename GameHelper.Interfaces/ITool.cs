using System.Threading;
using System.Threading.Tasks;

namespace GameHelper.Interfaces
{
    public interface ITool
    {
        Task RunAsync(CancellationToken cancellationToken);
    }
}
