using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Zummer.Handlers
{
    public interface IRegexHandler
    {
        Task Respond(IMessageActivity activity, Match result);
    }
}