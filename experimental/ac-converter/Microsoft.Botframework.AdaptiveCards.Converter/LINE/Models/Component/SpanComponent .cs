using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public class SpanComponent : IFlexComponent
    {
        public string Type => FlexComponentType.Span;

        /// <summary>
        /// Text.
        /// (Required).
        /// </summary>
        public string Text { get; set; }

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
        /// Font weight.
        /// You can specify one of the following values: regular, or bold.
        /// Specifying bold makes the font bold. The default value is regular.
        /// (Optional).
        /// Type is one of the <see cref="Models.Weight"/>
        /// </summary>
        public string Weight { get; set; }

        /// <summary>
        /// Font color. Use a hexadecimal color code.
        /// (Optional).
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Style of the text.
        /// Specify one of the following values: normal, or italic.
        /// The default value is normal.
        /// (Optional).
        /// Type is one of the <see cref="Models.Style"/>
        /// </summary>
        public string Style { get; set; }


        /// <summary>
        /// Decoration of the text.
        /// Specify one of the following values: none, underline, or line-through.
        /// The default value is none.
        /// (Optional).
        /// Type is one of the <see cref="Models.Decoration"/>
        /// </summary>
        public string Decoration { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">Text.</param>
        public SpanComponent(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpanComponent()
        {
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Text);
        }
    }
}
