// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
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
        public const string DefaultCardTitle = "Did you mean:";
        public const string DefaultCardNoMatchText = "None of the above.";
        public const string DefaultCardNoMatchResponse = "Thanks for the feedback.";
        private readonly IBotServices _services;
        private readonly IConfiguration _configuration;
        private readonly string DefaultAnswer = "";

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAMakerBaseDialog"/> class.
        /// Dialog helper to generate dialogs.
        /// </summary>
        /// <param name="services">Bot Services.</param>
        public QnAMakerBaseDialog(IBotServices services, IConfiguration configuration) : base()
        {
            this._configuration = configuration;
            this._services = services;

            if (!string.IsNullOrWhiteSpace(configuration["DefaultAnswer"]))
            {
                this.DefaultAnswer = configuration["DefaultAnswer"];
            }
        }

        protected async override Task<IQnAMakerClient> GetQnAMakerClientAsync(DialogContext dc)
        {
            return _services?.QnAMakerService;
        }

        protected override Task<QnAMakerOptions> GetQnAMakerOptionsAsync(DialogContext dc)
        {
            return Task.FromResult(new QnAMakerOptions
            {
                ScoreThreshold = (float)DefaultThreshold,
                Top = DefaultTopN,
                QnAId = 0,
                RankerType = "Default",
                IsTest = false,

                /*
                * Legacy and V2 preview metadata usage - Uncomment below secrion to apply metadata strictFilters
                */

                /*StrictFilters = new[] { new Metadata { Name= "a", Value ="b" }, new Metadata { Name = "c", Value = "d" }},
                StrictFiltersJoinOperator = JoinOperator.OR,*/

                /*
                * Language Service metadata usage - Uncomment below section to apply filters 
                */

                /*Filters = new Bot.Builder.AI.QnA.Models.Filters
                {
                   MetadataFilter = new Bot.Builder.AI.QnA.Models.MetadataFilter
                   {
                       Metadata = GetMetadata(),
                       LogicalOperation = JoinOperator.AND.ToString()
                   },
                   SourceFilter = (new[] { "GithubSampleActiveLearning.tsv", "SampleActiveLearningImport.tsv" }).ToList(),
                   LogicalOperation = JoinOperator.OR.ToString()
                },*/

                EnablePreciseAnswer = this.IsPreciseAnswerEnabled,

                /*
                * For all v2 and language service bots, IncludeUnstructuredSources is set to true by default
                * To exclude unstructured content from answers, set IncludeUnstructuredSources = false
                */

                IncludeUnstructuredSources = this.IsNotLegacyService
            });
        }

        protected async override Task<QnADialogResponseOptions> GetQnAResponseOptionsAsync(DialogContext dc)
        {
            var defaultAnswerActivity = MessageFactory.Text(this.DefaultAnswer);

            var cardNoMatchResponse = (Activity)MessageFactory.Text(DefaultCardNoMatchResponse);

            var responseOptions = new QnADialogResponseOptions
            {
                ActiveLearningCardTitle = DefaultCardTitle,
                CardNoMatchText = DefaultCardNoMatchText,
                NoAnswer = defaultAnswerActivity,
                CardNoMatchResponse = cardNoMatchResponse,
                DisplayPreciseAnswerOnly = this.DisplayPreciseAnswerOnly
            };

            return responseOptions;
        }

        /// <summary>
        /// Helper method to construct metadata in expected format
        /// </summary>
        /// <returns></returns>
        private System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>> GetMetadata()
        {
            var metadata = new List<System.Collections.Generic.KeyValuePair<string, string>>();

            // For e.g, metadata pairs "category":"api", "language":"csharp" can be specified as below
            // metadata.Add(new KeyValuePair<string, string>("category", "api"));
            // metadata.Add(new KeyValuePair<string, string>("language", "csharp"));
            return metadata;
        }

        private bool IsPreciseAnswerEnabled
        {
            get
            {
                if (IsNotLegacyService)
                {
                    var rawEnablePreciseAnswer = _configuration["EnablePreciseAnswer"];
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
                var rawDisplayPreciseAnswerOnly = _configuration["DisplayPreciseAnswerOnly"];
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

        private bool IsNotLegacyService
        {
            get
            {
                var qnaServiceType = _configuration["QnAServiceType"];
                if (string.Equals(qnaServiceType, "v2", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(qnaServiceType, "language", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
