using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// Tap to open camera roll.
    /// https://developers.line.me/en/reference/messaging-api/#camera-roll-action.
    /// </summary>
    public class CameraRollAction : ITemplateAction
    {
        public string Type { get; } = TemplateActionTypes.CameraRoll;

        /// <summary>
        /// Max: 20 characters.
        /// </summary>
        public string Label { get; set; }

        public CameraRollAction(string label)
        {
            Label = label.Substring(0, Math.Min(label.Length, 20));
        }
    }
}
