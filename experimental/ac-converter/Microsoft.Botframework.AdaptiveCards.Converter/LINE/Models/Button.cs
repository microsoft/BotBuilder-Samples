using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// This is a quick reply option that is displayed as a button.
    /// https://developers.line.me/en/reference/messaging-api/#quick-reply-button-object.
    /// </summary>
    public class Button
    {
        public string Type = "action";

        /// <summary>
        /// URL of the icon that is displayed at the beginning of the button
        /// Optional.
        /// Max character limit: 1000
        /// URL scheme: https
        /// Image format: PNG
        /// Aspect ratio: 1:1
        /// Data size: Up to 1 MB
        /// There is no limit on the image size.
        /// If the action property has a camera action, camera roll action, or location action, and the imageUrl property is not set, the default icon is displayed.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Action performed when this button is tapped. Specify an action object. 
        /// Required.
        /// The following is a list of the available actions:
        /// Postback action
        /// Message action
        /// Datetime picker action
        /// Camera action
        /// Camera roll action
        /// Location action
        /// </summary>
        public ITemplateAction Action { get; set; }

        public Button(ITemplateAction action, string imageUrl = null)
        {
            Action = action;
            ImageUrl = imageUrl;
        }
    }
}
