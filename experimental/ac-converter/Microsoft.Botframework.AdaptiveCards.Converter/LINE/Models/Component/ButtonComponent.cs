using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// This component draws a button. When the user taps a button, a specified action is performed.
    /// </summary>
    public class ButtonComponent : IFlexComponent
    {
        public string Type => FlexComponentType.Button;

        /// <summary>
        /// Action performed when this button is tapped. Specify an action object.
        /// (Required).
        /// </summary>
        public ITemplateAction Action { get; set; }

        /// <summary>
        /// The ratio of the width or height of this component within the parent box. 
        /// The default value for the horizontal parent box is 1, and the default value for the vertical parent box is 0. For more information, see Width and height of components.
        /// (Optional).
        /// </summary>
        public int? Flex { get; set; }

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
        /// Height of the button. You can specify sm or md. The default value is md.
        /// (Optional).
        /// Type is one of the <see cref="Models.ButtonHeight"/>
        /// </summary>
        public string Height { get; set; }

        /// <summary>
        /// Style of the button. 
        /// Specify one of the following values:
        /// - link: HTML link style
        /// - primary: Style for dark color buttons
        /// - secondary: Style for light color buttons.
        /// The default value is link.
        /// (Optional).
        /// Type is one of the <see cref="Models.ButtonStyle"/>
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// Character color when the style property is link. 
        /// Background color when the style property is primary or secondary. Use a hexadecimal color code.
        /// (Optional).
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Vertical alignment style. 
        /// Specify one of the following values:
        /// - top: Top-aligned
        /// - bottom: Bottom-aligned
        /// - center: Center-aligned.
        /// The default value is top.
        /// If the layout property of the parent box is baseline, the gravity property will be ignored.
        /// (Optional).
        /// Type is one of the <see cref="Models.Gravity"/>
        /// </summary>
        public string Gravity { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="action">
        /// Action performed when this button is tapped. Specify an action object.
        /// </param>
        public ButtonComponent(ITemplateAction action)
        {
            Action = action;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ButtonComponent()
        {
        }

        public bool IsValid()
        {
            return Action != null;
        }
    }
}
