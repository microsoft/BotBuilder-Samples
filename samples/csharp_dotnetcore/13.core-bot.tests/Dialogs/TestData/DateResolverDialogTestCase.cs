// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CoreBot.Tests.Dialogs.TestData
{
    public class DateResolverDialogTestCase
    {
        /// <summary>
        /// Gets or sets the name for the test case.
        /// </summary>
        /// <value>The test case name.</value>
        public string Name { get; set; }

        public string InitialData { get; set; }

        public string ExpectedResult { get; set; }

        public string[,] UtterancesAndReplies { get; set; }
    }
}
