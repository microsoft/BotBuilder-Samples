using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// The placement style of components in this box. 
    /// Specify one of the following values.
    /// </summary>
    public class BoxLayout
    {
        /// <summary>
        /// Components are placed horizontally. 
        /// The direction property of the bubble container specifies the order.
        /// </summary>
        public const string Horizontal = "horizontal";

        /// <summary>
        /// Components are placed vertically from top to bottom.
        /// </summary>
        public const string Vertical = "vertical";

        /// <summary>
        /// Components are placed in the same way as horizontal is specified except the baselines of the components are aligned.
        /// </summary>
        public const string Baseline = "baseline";
    }
}
