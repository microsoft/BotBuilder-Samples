using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace NewsieBot.Handlers
{
    public interface IRegexHandler
    {
        Task Respond(IMessageActivity activity, Match result);
    }
}