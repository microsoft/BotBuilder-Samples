using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// Tap to open camera screen.
    /// https://developers.line.me/en/reference/messaging-api/#camera-action.
    /// </summary>
    public class CameraAction : ITemplateAction
    {
        public string Type { get; } = TemplateActionTypes.Camera;

        /// <summary>
        /// Label for the action
        /// Max: 20 characters.
        /// </summary>
        public string Label { get; set; }
        
        public CameraAction(string label)
        {
            Label = label.Substring(0, Math.Min(label.Length, 20));
        }
    }
}
