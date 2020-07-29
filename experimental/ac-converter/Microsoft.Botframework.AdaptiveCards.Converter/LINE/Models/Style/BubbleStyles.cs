using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// Use the following two objects to define the style of blocks in a bubble.
    /// </summary>
    public class BubbleStyles
    {
        /// <summary>
        /// Style of the header block.
        /// (Optional).
        /// </summary>
        public BlockStyle Header { get; set; }

        /// <summary>
        /// Style of the hero block.
        /// (Optional).
        /// </summary>
        public BlockStyle Hero { get; set; }

        /// <summary>
        /// Style of the body block.
        /// (Optional).
        /// </summary>
        public BlockStyle Body { get; set; }

        /// <summary>
        /// Style of the footer block.
        /// (Optional).
        /// </summary>
        public BlockStyle Footer { get; set; }
    }
}
