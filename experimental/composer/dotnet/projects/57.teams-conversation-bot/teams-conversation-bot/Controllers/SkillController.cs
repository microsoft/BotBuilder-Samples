using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.Composer.Samples.TeamsConversationBot.Controllers
{
    /// <summary>
    /// A controller that handles skill replies to the bot.
    /// </summary>
    [ApiController]
    [Route("api/skills")]
    public class SkillController : ChannelServiceController
    {
        private readonly ILogger<SkillController> _logger;

        public SkillController(ChannelServiceHandlerBase handler, ILogger<SkillController> logger)
            : base(handler)
        {
            _logger = logger;
        }

        public override Task<IActionResult> ReplyToActivityAsync(string conversationId, string activityId, Activity activity)
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"ReplyToActivityAsync: conversationId={conversationId}, activityId={activityId}");
                }

                return base.ReplyToActivityAsync(conversationId, activityId, activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ReplyToActivityAsync: {ex}");
                throw;
            }
        }

        public override Task<IActionResult> SendToConversationAsync(string conversationId, Activity activity)
        {
            try
            {
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug($"SendToConversationAsync: conversationId={conversationId}");
                }

                return base.SendToConversationAsync(conversationId, activity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"SendToConversationAsync: {ex}");
                throw;
            }
        }
    }
}
