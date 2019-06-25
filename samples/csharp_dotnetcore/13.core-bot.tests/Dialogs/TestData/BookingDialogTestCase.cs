// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.BotBuilderSamples;

namespace CoreBot.Tests.Dialogs.TestData
{
    public class BookingDialogTestCase
    {
        /// <summary>
        /// Gets or sets the name for the test case.
        /// </summary>
        /// <value>The test case name.</value>
        public string Name { get; set; }

        public BookingDetails InitialBookingDetails { get; set; }

        public string[,] UtterancesAndReplies { get; set; }

        public BookingDetails ExpectedBookingDetails { get; set; }
    }
}
