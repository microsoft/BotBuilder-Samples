// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace Microsoft.BotBuilderSamples.Models
{
    public class Survey
    {
        public static Survey NewSurvey(string surveyId)
        {
            const string Unknown = "unknown";

            return new Survey
            {
                SurveyId = surveyId,
                Responses = new List<SurveyResponse>(),
                Question = Unknown,
                CreatedByUserId = Unknown,
                CreatedByUserName = Unknown,
            };
        }

        public string Question { get; set; }

        public string SurveyId { get; set; }

        public string CreatedByUserId { get; set; }

        public string CreatedByUserName { get; set; }

        public List<SurveyResponse> Responses { get; set; }
    }
}
