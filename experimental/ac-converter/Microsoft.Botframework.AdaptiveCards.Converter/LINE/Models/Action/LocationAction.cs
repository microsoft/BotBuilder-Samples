using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// This action can be configured only with quick reply buttons. When a button associated with this action is tapped, the location screen in the LINE app is opened.
    /// https://developers.line.me/en/reference/messaging-api/#location-action.
    /// </summary>
    public class LocationAction : ITemplateAction
    {
        public string Type { get; } = TemplateActionTypes.Location;

        public string Label { get; set; }
        
        public LocationAction(string label)
        {
            Label = label.Substring(0, Math.Min(label.Length, 20));
        }
    }
}
