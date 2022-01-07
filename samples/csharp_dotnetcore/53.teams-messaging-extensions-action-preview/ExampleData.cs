// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    public class ExampleData
    {
        public ExampleData()
        {
            MultiSelect = "true";
            UserAttributionSelect = "true";
        }

        public string SubmitLocation { get; set; }

        public string Question { get; set; }

        public string MultiSelect { get; set; }

        public string Option1 { get; set; }

        public string Option2 { get; set; }

        public string Option3 { get; set; }

        public string UserAttributionSelect { get; set; }
    }
}
