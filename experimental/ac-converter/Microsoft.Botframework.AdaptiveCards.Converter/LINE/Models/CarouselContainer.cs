using System;
using System.Collections.Generic;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public class CarouselContainer : IFlexContainer
    {
        public string Type => FlexContainerType.Carousel;

        /// <summary>
        /// Array of bubble containers. Max: 10 bubbles.
        /// (Required).
        /// </summary>
        public IList<BubbleContainer> Contents { get; set; } = new List<BubbleContainer>();

        public CarouselContainer()
        {
        }
    }
}
