using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.AI.QnA;

namespace QnAMakerActiveLearningBot.QnAMakerServices
{

    /// <summary>
    /// Represents references to external services.
    ///
    /// For example, the QnA Maker service is kept here as a singleton. This external service is configured
    /// using the <see cref="Microsoft.Bot.Configuration.BotConfiguration"/> class
    /// (based on the contents of your ".bot" file).
    /// </summary>
    public class BotServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        /// <param name="qnaServices">A dictionary of named <see cref="QnAMaker"/> instances for usage within the bot.</param>
        public BotServices(Dictionary<string, QnAMaker> qnaServices, QnAMakerEndpoint qnaEndpoint)
        {
            QnAServices = qnaServices ?? throw new ArgumentNullException(nameof(qnaServices));
            QnaEndpoint = qnaEndpoint;
        }

        /// <summary>
        /// Get QnA Maker endpoint details
        /// </summary>
        public QnAMakerEndpoint QnaEndpoint { get; set; }

        /// <summary>
        /// Gets the (potential) set of QnA Services used.
        /// Given there can be multiple QnA services used in a single bot,
        /// QnA is represented as a Dictionary.  This is also modeled in the
        /// ".bot" file since the elements are named (string).
        /// This sample only uses a single QnA instance.
        /// </summary>
        /// <value>
        /// A QnAMaker client instance created based on configuration in the .bot file.
        /// </value>
        public Dictionary<string, QnAMaker> QnAServices { get; } = new Dictionary<string, QnAMaker>();
    }
}
