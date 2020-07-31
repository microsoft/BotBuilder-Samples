using System;
using System.Collections.Generic;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// These properties are used for the quick reply feature. 
    /// Supported on LINE 8.11.0 and later for iOS and Android.
    /// See detail at https://developers.line.me/en/reference/messaging-api/#quick-reply
    /// </summary>
    public class QuickReply
    {
        /// <summary>
        /// Quick reply button objects. 
        /// Max: 13 items.
        /// </summary>
        public IList<Button> Items { get; set; }

        public QuickReply(IList<Button> items)
        {
            Items = items;
        }
    }
}