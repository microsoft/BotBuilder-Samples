using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// This is a component that defines the layout of child components. You can also include a box in a box.
    /// </summary>
    public class BoxComponent : IFlexComponent
    {
        /// <summary>
        /// Box.
        /// </summary>
        public string Type => FlexComponentType.Box;

        /// <summary>
        /// The placement style of components in this box. Specify one of the following values.
        /// </summary>
        /// <param name="layout">
        /// The placement style of components in this box. Specify one of the following values.
        /// Type is one of the <see cref="Models.BoxLayout"/>
        /// </param>
        public BoxComponent(string layout)
        {
            Layout = layout;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public BoxComponent()
        {
            Layout = BoxLayout.Vertical;
        }

        /// <summary>
        /// The placement style of components in this box. Specify one of the following values.
        /// (Required).
        /// Type is one of the <see cref="Models.BoxLayout"/>
        /// </summary>
        public string Layout { get; set; }

        /// <summary>
        /// Components in this box. Here are the types of components available:
        /// - When the layout property is horizontal or vertical: Box, button, filler, image, separator, and text components.
        /// - When the layout property is baseline: filler, icon, and text components.
        /// (Required).
        /// </summary>
        public IList<IFlexComponent> Contents { get; set; } = new List<IFlexComponent>();

        /// <summary>
        /// Width of the box.
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// Height of the box.
        /// </summary>
        public string Height { get; set; }


        /// <summary>
        /// The ratio of the width or height of this box within the parent box. 
        /// The default value for the horizontal parent box is 1, and the default value for the vertical parent box is 0. 
        /// (Optional).
        /// </summary>
        public int? Flex { get; set; }

        /// <summary>
        /// Minimum space between components in this box. 
        /// You can specify one of the following values: none, xs, sm, md, lg, xl, or xxl. 
        /// none does not set a space while the other values set a space whose size increases in the order of listing. 
        /// The default value is none. 
        /// To override this setting for a specific component, set the margin property of that component.
        /// (Optional).
        /// Type is one of the <see cref="Models.Spacing"/>
        /// </summary>
        public string Spacing { get; set; }

        /// <summary>
        /// Minimum space between this box and the previous component in the parent box. 
        /// You can specify one of the following values: none, xs, sm, md, lg, xl, or xxl. 
        /// none does not set a space while the other values set a space whose size increases in the order of listing. 
        /// The default value is the value of the spacing property of the parent box. 
        /// If this box is the first component in the parent box, the margin property will be ignored.
        /// (Optional).
        /// Type is one of the <see cref="Models.Spacing"/>
        /// </summary>
        public string Margin { get; set; }

        /// <summary>
        /// Action performed when this box is tapped.
        /// Specify an action object. This property is supported on the following versions of LINE.
        /// LINE for iOS and Android: 8.11.0 and later
        /// LINE for Windows and macOS: 5.9.0 and later.
        /// 
        /// </summary>
        public ITemplateAction Action { get; set; }

        public bool IsValid()
        {
            return Contents?.Any() ?? false;
        }
    }
}
