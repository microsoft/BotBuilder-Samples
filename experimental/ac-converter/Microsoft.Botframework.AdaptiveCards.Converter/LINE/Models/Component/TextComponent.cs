using System;
using System.Collections.Generic;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public class TextComponent : IFlexComponent
    {
        public string Type => FlexComponentType.Text;

        /// <summary>
        /// Text.
        /// (Required).
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Array of spans. 
        /// Be sure to set either one of the text property or contents property. If you set the contents property, text is ignored.
        /// </summary>
        public IList<IFlexComponent> Contents { get; set; } = new List<IFlexComponent>();

        /// <summary>
        /// The ratio of the width or height of this component within the parent box.
        /// The default value for the horizontal parent box is 1, and the default value for the vertical parent box is 0. 
        /// For more information, see Width and height of components.
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
        /// Font size. 
        /// You can specify one of the following values: xxs, xs, sm, md, lg, xl, xxl, 3xl, 4xl, or 5xl. 
        /// The size increases in the order of listing. 
        /// The default value is md.
        /// (Optional).
        /// Type is one of the <see cref="Models.FontSize"/>
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Horizontal alignment style.
        /// Specify one of the following values:
        /// start: Left-aligned
        /// end: Right-aligned
        /// center: Center-aligned
        /// , The default value is start.
        /// (Optional).
        /// Type is one of the <see cref="Models.Align"/>
        /// </summary>
        public string Align { get; set; }

        /// <summary>
        /// Vertical alignment style. 
        /// Specify one of the following values:
        /// top: Top-aligned
        /// bottom: Bottom-aligned
        /// center: Center-aligned
        /// The default value is top.
        /// If the layout property of the parent box is baseline, the gravity property will be ignored.
        /// (Optional).
        /// Type is one of the <see cref="Models.Gravity"/>
        /// </summary>
        public string Gravity { get; set; }

        /// <summary>
        /// true to wrap text. The default value is false. If set to true, you can use a new line character (\n) to begin on a new line.
        /// (Optional).
        /// </summary>
        public bool? Wrap { get; set; }

        /// <summary>
        /// Max number of lines. 
        /// If the text does not fit in the specified number of lines, an ellipsis (…) is displayed at the end of the last line. 
        /// If set to 0, all the text is displayed. 
        /// The default value is 0. 
        /// This property is supported on the following versions of LINE.
        /// LINE for iOS and Android: 8.11.0 and later
        /// LINE for Windows and macOS: 5.9.0 and later.
        /// (Optional).
        /// </summary>
        public int? MaxLines { get; set; }

        /// <summary>
        /// Font weight. You can specify one of the following values: regular, or bold. Specifying bold makes the font bold. The default value is regular.
        /// (Optional).
        /// Type is one of the <see cref="Models.Weight"/>
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// Font weight. You can specify one of the following values: regular, or bold. Specifying bold makes the font bold. The default value is regular.
        /// (Optional).
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Action performed when this text is tapped. Specify an action object.
        /// (Optional).
        /// </summary>
        public ITemplateAction Action { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Text.</param>
        public TextComponent(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TextComponent()
        {
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Text);
        }
    }
}
