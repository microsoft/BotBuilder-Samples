using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public interface IFlexContainer
    {
        /// <summary>
        /// Flex container type.
        /// Type is one of the <see cref="Models.FlexContainerType"/>
        /// </summary>
        string Type { get; }
    }
}
