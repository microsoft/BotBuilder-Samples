using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// Tap to open an Uri.
    /// https://developers.line.me/en/docs/messaging-api/reference/#uri-action.
    /// </summary>
    public class UriAction : ITemplateAction
    {
        public string Type { get; } = TemplateActionTypes.Uri;

        /// <summary>
        /// Label for the action
        /// Required for templates other than image carousel.Max: 20 characters
        /// Optional for image carousel templates.Max: 12 characters.
        /// Optional for rich menus. Spoken when the accessibility feature is enabled on the client device. Max: 20 characters.
        /// Supported on LINE iOS version 8.2.0 and later.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// URI opened when the action is performed (Max: 1000 characters)
        /// Must start with http, https, line, or tel.
        /// </summary>
        public string Uri { get; set; }

        public UriAction(string label, string uri)
        {
            Label = label?.Substring(0, Math.Min(label.Length, 20));
            Uri = uri;
        }
    }
}
