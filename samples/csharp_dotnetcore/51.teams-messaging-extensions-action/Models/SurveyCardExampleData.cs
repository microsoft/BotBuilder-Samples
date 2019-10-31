// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples.Models
{
    public class SurveyCardExampleData
    {
        public SurveyCardExampleData()
        {
            IsMultiSelect = "false";
        }

        public string SubmitLocation { get; set; }

        public string Question { get; set; }

        public string IsMultiSelect { get; set; }

        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }
    }
}
