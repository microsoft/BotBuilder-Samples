using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Newsie.Handlers
{
    public interface IRegexHandler
    {
        Task Respond(IMessageActivity activity, Match result);
    }
}