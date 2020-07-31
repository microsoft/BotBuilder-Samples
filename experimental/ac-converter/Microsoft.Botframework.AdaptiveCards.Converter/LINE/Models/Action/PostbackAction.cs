using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// Tap to get postback event contains specified string in data field.
    /// If you have included the text field, the string in the text field is sent as a message from the user.
    /// https://developers.line.me/en/docs/messaging-api/reference/#postback-action.
    /// </summary>
    public class PostbackAction : ITemplateAction
    {
        public string Type { get; } = TemplateActionTypes.Postback;

        /// <summary>
        /// Label for the action
        /// Required for templates other than image carousel.Max: 20 characters
        /// Optional for image carousel templates. Max: 12 characters.
        /// Optional for rich menus. Spoken when the accessibility feature is enabled on the client device. Max: 20 characters. 
        /// Supported on LINE iOS version 8.2.0 and later.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// String returned via webhook in the postback.data property of the postback event
        /// Max: 300 characters.
        /// </summary>
        public string Data { get; }

        /// <summary>
        /// Deprecated. Text displayed in the chat as a message sent by the user when the action is performed.
        /// Max: 300 characters
        /// The displayText and text fields cannot both be used at the same time.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Text displayed in the chat as a message sent by the user when the action is performed.
        /// Max: 300 characters
        /// The displayText and text fields cannot both be used at the same time.
        /// </summary>
        public string DisplayText { get; }

        /// <summary>
        /// Constructor.
        /// !import text property is deprecated.
        /// </summary>
        public PostbackAction(string label, string data, string text = null, bool useDisplayText = true)
        {
            Data = data.Substring(0, Math.Min(data.Length, 300));
            Label = label?.Substring(0, Math.Min(label.Length, 20));

            if (useDisplayText)
            {
                DisplayText = text?.Substring(0, Math.Min(text.Length, 300));
            }
            else
            {
                Text = text?.Substring(0, Math.Min(text.Length, 300));
            }

        }
    }
}
