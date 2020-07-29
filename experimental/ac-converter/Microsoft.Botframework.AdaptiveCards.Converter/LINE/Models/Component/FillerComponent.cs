using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// This is an invisible component to fill extra space between components.
    /// </summary>
    public class FillerComponent : IFlexComponent
    {
        public string Type => FlexComponentType.Filler;

        public bool IsValid()
        {
            return true;
        }
    }
}
