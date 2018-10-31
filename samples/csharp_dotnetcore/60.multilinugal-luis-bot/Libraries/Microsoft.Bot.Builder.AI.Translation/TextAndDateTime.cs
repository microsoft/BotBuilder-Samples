// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Microsoft.Bot.Builder.AI.Translation
{
    /// <summary>
    /// TextAndDateTime Class used to store  text and date time object
    /// from Microsoft Recognizer recognition result.
    /// </summary>
    internal class TextAndDateTime
    {
        public string Text { get; set; }

        public DateTime DateTime { get; set; }

        public string Type { get; set; }

        public bool Range { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}
