using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// Tap to send the text.
    /// https://developers.line.me/en/docs/messaging-api/reference/#message-action.
    /// </summary>
    public class MessageAction : ITemplateAction
    {
        public string Type { get; } = TemplateActionTypes.Message;

        /// <summary>
        /// Label for the action
        /// Required for templates other than image carousel.Max: 20 characters
        /// Optional for image carousel templates.Max: 12 characters.
        /// Optional for rich menus. Spoken when the accessibility feature is enabled on the client device. Max: 20 characters. 
        /// Supported on LINE iOS version 8.2.0 and later.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Text sent when the action is performed
        /// Max: 300 characters.
        /// </summary>
        public string Text { get; set; }

        public MessageAction(string label, string text)
        {
            Label = label?.Substring(0, Math.Min(label.Length, 20));
            Text = text.Substring(0, Math.Min(text.Length, 300));
        }
    }
}
