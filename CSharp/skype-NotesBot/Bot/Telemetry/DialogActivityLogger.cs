using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;

namespace Bot.Telemetry
{
    public class DialogActivityLogger : IActivityLogger
    {
        private readonly IBotData _botData;

        public DialogActivityLogger(IBotData botData = null)
        {
            _botData = botData;
        }

        public async Task LogAsync(IActivity activity)
        {
            TelemetryLogger.TrackActivity(activity, _botData);
        }
    }
}