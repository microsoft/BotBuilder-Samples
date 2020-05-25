// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.AI.QnA.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace Microsoft.BotBuilderSamples.Dialog
{
    /// <summary>
    /// QnAMaker action builder class
    /// </summary>
    public class QnAMakerBaseDialog : QnAMakerDialog
    {
        // Dialog Options parameters
        public const string DefaultNoAnswer = "No QnAMaker answers found.";
        public const string DefaultCardTitle = "Did you mean:";
        public const string DefaultCardNoMatchText = "None of the above.";
        public const string DefaultCardNoMatchResponse = "Thanks for the feedback.";
        private readonly IBotServices _services;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAMakerBaseDialog"/> class.
        /// Dialog helper to generate dialogs.
        /// </summary>
        /// <param name="services">Bot Services.</param>
        public QnAMakerBaseDialog(IBotServices services, IConfiguration config): base()
        {
            this._services = services;
            this._config = config;
        }

        protected async override Task<IQnAMakerClient> GetQnAMakerClientAsync(DialogContext dc)
        {
            return this._services?.QnAMakerService;
        }

        protected override Task<QnAMakerOptions> GetQnAMakerOptionsAsync(DialogContext dc)
        {
            return Task.FromResult(new QnAMakerOptions
            {
                ScoreThreshold = DefaultThreshold,
                Top = DefaultTopN,
                QnAId = 0,
                RankerType = "Default",
                IsTest = false,
                EnablePreciseAnswer = this.EnablePreciseAnser
            }); 
        }

        protected async override Task<QnADialogResponseOptions> GetQnAResponseOptionsAsync(DialogContext dc)
        {
            var noAnswer = (Activity)Activity.CreateMessageActivity();
            noAnswer.Text = DefaultNoAnswer;

            var cardNoMatchResponse = (Activity)MessageFactory.Text(DefaultCardNoMatchResponse);

            var responseOptions = new QnADialogResponseOptions
            {
                ActiveLearningCardTitle = DefaultCardTitle,
                CardNoMatchText = DefaultCardNoMatchText,
                NoAnswer = noAnswer,
                CardNoMatchResponse = cardNoMatchResponse,
                DisplayPreciseAnswerOnly = this.DisplayPreciseAnswerOnly
            };

            return responseOptions;
        }
        private bool EnablePreciseAnser
        {
            get
            {
                var qnaServiceType = _config["QnAServiceType"];
                if (string.Equals("v2",qnaServiceType, StringComparison.OrdinalIgnoreCase))
                {
                    var rawEnablePreciseAnswer = _config["EnablePreciseAnswer"];
                    if (!string.IsNullOrWhiteSpace(rawEnablePreciseAnswer))
                    {
                        return bool.Parse(rawEnablePreciseAnswer);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private bool DisplayPreciseAnswerOnly
        {
            get
            {               
                var rawDisplayPreciseAnswerOnly = _config["DisplayPreciseAnswerOnly"];
                if (!string.IsNullOrWhiteSpace(rawDisplayPreciseAnswerOnly))
                {
                    return bool.Parse(rawDisplayPreciseAnswerOnly);
                }
                else
                {
                    return true;
                }                
            }
        }
    }
}
