using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// This component draws a separator between components in the parent box.
    /// </summary>
    public class SeparatorComponent : IFlexComponent
    {
        public string Type => FlexComponentType.Separator;

        /// <summary>
        /// Minimum space between this component and the previous component in the parent box. 
        /// You can specify one of the following values: none, xs, sm, md, lg, xl, or xxl. 
        /// none does not set a space while the other values set a space whose size increases in the order of listing. 
        /// The default value is the value of the spacing property of the parent box. 
        /// If this component is the first component in the parent box, the margin property will be ignored.
        /// (Optional).
        /// Type is one of the <see cref="Models.Spacing"/>
        /// </summary>
        public string Margin { get; set; }

        /// <summary>
        /// Color of the separator. Use a hexadecimal color code.
        /// (Optional).
        /// </summary>
        public string Color { get; set; }

        public bool IsValid()
        {
            return true;
        }
    }
}
