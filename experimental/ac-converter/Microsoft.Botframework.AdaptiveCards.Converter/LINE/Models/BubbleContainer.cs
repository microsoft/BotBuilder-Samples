using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public class BubbleContainer : IFlexContainer
    {
        public string Type => FlexContainerType.Bubble;

        /// <summary>
        /// Text directionality and the order of components in horizontal boxes in the container. 
        /// Specify one of the following values:
        /// ltr: Left to right
        /// rtl: Right to left
        /// The default value is ltr.
        /// <seealso cref="ComponentDirection">
        /// (Optional).
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// Header block. Specify a box component.
        /// (Optional).
        /// </summary>
        public BoxComponent Header { get; set; }

        /// <summary>
        /// Hero block. Specify an image component.
        /// (Optional).
        /// </summary>
        public ImageComponent Hero { get; set; }

        /// <summary>
        /// Body block. Specify a box component.
        /// (Optional).
        /// </summary>
        public BoxComponent Body { get; set; }

        /// <summary>
        /// Footer block. Specify a box component.
        /// (Optional).
        /// </summary>
        public BoxComponent Footer { get; set; }

        /// <summary>
        /// Style of each block. Specify a bubble style object. For more information, see Objects for the block style.
        /// (Optional).
        /// </summary>
        public BubbleStyles Styles { get; set; }
    }
}
