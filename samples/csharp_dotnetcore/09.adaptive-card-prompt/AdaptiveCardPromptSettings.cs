using System.Collections.Generic;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Dialogs
{
    /// <summary>
    /// Settings to control the behavior of AdaptiveCardPrompt.
    /// </summary>
    public class AdaptiveCardPromptSettings
    {
        /// <summary>
        /// Gets or sets the card to send the user.
        /// </summary>
        /// <value>
        /// An Adaptive Card. Can be input here or in constructor.
        /// </value>
        public Attachment Card { get; set; }

        /// <summary>
        /// Gets or sets the message sent (if not null) when user uses text input instead of Adaptive Card Input.
        /// </summary>
        /// <value>
        /// The message sent (if not null) when user uses text input instead of Adaptive Card Input.
        /// </value>
        /// <remarks>
        /// Defaults to: 'Please fill out the Adaptive Card'.
        /// </remarks>
        public string InputFailMessage { get; set; }

        /// <summary>
        /// Gets or sets the array of strings matching IDs of required input fields.
        /// </summary>
        /// <value>
        /// The message sent (if not null) when user uses text input instead of Adaptive Card Input.
        /// </value>
        /// <remarks>
        /// The ID strings must exactly match those used in the Adaptive Card JSON Input IDs
        /// For JSON:
        /// ```json
        /// {
        ///   "type": "Input.Text",
        ///   "id": "myCustomId",
        /// },
        /// ```
        ///   You would use `"myCustomId"` if you want that to be a required input.
        /// </remarks>
        public List<string> RequiredInputIds { get; set; }

        /// <summary>
        /// Gets or sets the message sent (if not null) when user doesn't submit a required input.
        /// </summary>
        /// <value>
        /// The message sent (if not null) when user doesn't submit a required input.
        /// <Each, Missing, Input> gets appended to the end of the string
        /// </value>
        /// <remarks>
        /// Defaults to: The following inputs are required'
        /// </remarks>
        public string MissingRequiredInputsMessage { get; set; }

        /// <summary>
        /// Gets or sets the number of attempts before the card is redisplayed/re-prompted.
        /// </summary>
        /// <value>
        /// The number of attempts before the card is redisplayed/re-prompted.
        /// </value>
        /// <remarks>
        /// Card will not be redisplayed/re-prompted unless:
        ///   * PromptOptions includes a retryPrompt with a card, or
        ///   * Number of attempts per displayed card equals this value
        /// This is meant to prevent the user from providing input to the original,
        ///   and not the re-prompted card.
        /// Defaults to 3.
        /// </remarks>
        public int? AttemptsBeforeCardRedisplayed { get; set; }

        /// <summary>
        /// Gets or sets the ID specific to this prompt.
        /// </summary>
        /// <value>
        /// The ID specific to this prompt.
        /// </value>
        /// <remarks>
        /// Card input is only accepted if SubmitAction.data.promptId matches the promptId.
        /// This is set to a random GUID and randomizes again on reprompts by default.
        ///   If set manually, will not change between reprompts.
        /// </remarks>
        public string PromptId { get; set; }
    }
}
