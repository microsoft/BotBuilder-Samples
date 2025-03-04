// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace CoreBot.Tests.Dialogs.TestData
{
    /// <summary>
    /// A class to store test case data for <see cref="DateResolverDialogTests"/>
    /// </summary>
    public class DateResolverDialogTestCase
    {
        public string Name { get; set; }

        public string InitialData { get; set; }
#pragma warning disable CA1819 // Properties should not return arrays
        public string[,] UtterancesAndReplies { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays
        public string ExpectedResult { get; set; }
    }
}
