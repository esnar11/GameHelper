using System.Collections.Generic;

namespace GameHelper.Interfaces
{
    public interface IChatHistory
    {
    }

    public interface IChatHistoryReadonly: IChatHistory
    {
        IReadOnlyCollection<string> SearchChannel(string filter);
    }
}
