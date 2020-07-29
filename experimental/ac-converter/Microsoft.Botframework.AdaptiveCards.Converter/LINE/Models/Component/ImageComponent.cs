using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// This component draws an image.
    /// </summary>
    public class ImageComponent : IFlexComponent
    {
        public string Type => FlexComponentType.Image;

        /// <summary>
        /// Image URL (Required).
        /// Protocol: HTTPS
        /// Image format: JPEG or PNG
        /// Maximum image size: 1024×1024 pixels
        /// Maximum data size: 1 MB.
        /// (Required).
        /// </summary>
        public string Url { get; set; }

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
        ///If this component is the first component in the parent box, the margin property will be ignored.
        ///(Optional).
        /// Type is one of the <see cref="Models.Spacing"/>
        /// </summary>
        public string Margin { get; set; }

        /// <summary>
        /// Reference for offsetTop, offsetBottom, offsetStart, and offsetEnd. Specify one of the following values:
        /// relative: Use the previous box as reference.
        /// absolute: Use the top left of parent element as reference.
        /// The default value is relative.
        /// (Optional).
        /// Type is one of the <see cref="Models.Position"/> 
        /// </summary>
        public string Position { get; set; }


        /// <summary>
        /// The top offset.
        /// </summary>
        public string OffsetTop { get; set; }

        /// <summary>
        /// The bottom offset.
        /// </summary>
        public string OffsetBottom { get; set; }

        /// <summary>
        /// The left offset.
        /// </summary>
        public string OffsetStart { get; set; }

        /// <summary>
        /// The right offset.
        /// </summary>
        public string OffsetEnd { get; set; }

        /// <summary>
        /// Horizontal alignment style. 
        /// Specify one of the following values:
        /// start: Left-aligned
        /// end: Right-aligned
        /// center: Center-aligned
        /// , The default value is center.
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
        /// , The default value is top.
        /// If the layout property of the parent box is baseline, the gravity property will be ignored.
        /// (Optional).
        /// Type is one of the <see cref="Models.Gravity"/>
        /// </summary>
        public string Gravity { get; set; }

        /// <summary>
        /// Maximum size of the image width. 
        /// You can specify one of the following values: xxs, xs, sm, md, lg, xl, xxl, 3xl, 4xl, 5xl, or full. 
        /// The size increases in the order of listing. 
        /// The default value is md.
        /// (Optional).
        /// Type is one of the <see cref="Models.ImageSize"/>
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Aspect ratio of the image. 
        /// You can specify one of the following values: 1:1, 1.51:1, 1.91:1, 4:3, 16:9, 20:13, 2:1, 3:1, 3:4, 9:16, 1:2, or 1:3. 
        /// The default value is 1:1.
        /// (Optional).
        /// </summary>
        public AspectRatio AspectRatio { get; set; }

        /// <summary>
        /// Style of the image. 
        /// Specify one of the following values:
        /// cover: The image fills the entire drawing area.Parts of the image that do not fit in the drawing area are not displayed.
        /// fit: The entire image is displayed in the drawing area.The background is displayed in the unused areas to the left and right of vertical images and in the areas above and below horizontal images.
        /// The default value is fit.
        /// (Optional).
        /// Type is one of the <see cref="Models.AspectMode"/>
        /// </summary>
        public string AspectMode { get; set; }

        /// <summary>
        /// Background color of the image. Use a hexadecimal color code.
        /// (Optional).
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// Action performed when this image is tapped. Specify an action object.
        /// (Optional).
        /// </summary>
        public ITemplateAction Action { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">
        /// Image URL (Required).
        /// Protocol: HTTPS
        /// Image format: JPEG or PNG
        /// Maximum image size: 1024×1024 pixels
        /// Maximum data size: 1 MB. 
        /// </param>
        public ImageComponent(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ImageComponent()
        {
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Url);
        }
    }
}
