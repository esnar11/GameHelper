using System.Threading;
using System.Threading.Tasks;

namespace GameHelper.Interfaces
{
    public interface ITranslateService
    {
        Task<string> Translate(string value, CancellationToken cancellationToken = default);
    }
}
