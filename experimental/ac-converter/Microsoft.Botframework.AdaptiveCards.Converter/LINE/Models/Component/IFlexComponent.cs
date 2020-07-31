using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public interface IFlexComponent
    {
        string Type { get; }

        bool IsValid();
    }
}
