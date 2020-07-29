using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// Block Style.
    /// </summary>
    public class BlockStyle
    {
        /// <summary>
        /// Background color of the block. Use a hexadecimal color code.
        /// (Optional).
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// true to place a separator above the block. 
        /// true will be ignored for the first block in a container because you cannot place a separator above the first block. 
        /// The default value is false. 
        /// (Optional).
        /// </summary>
        
        public bool Separator { get; set; }

        /// <summary>
        /// Color of the separator. Use a hexadecimal color code.
        /// (Optional).
        /// </summary>
        public string SeparatorColor { get; set; }
    }
}
