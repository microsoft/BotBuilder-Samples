using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public class IconComponent : IFlexComponent
    {
        public string Type => FlexComponentType.Icon;

        /// <summary>
        /// Image URL.
        /// Protocol: HTTPS
        /// Image format: JPEG or PNG
        /// Maximum image size: 240×240 pixels
        /// Maximum data size: 1 MB.
        /// 
        /// (Required).
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Minimum space between this component and the previous component in the parent box. 
        /// You can specify one of the following values: none, xs, sm, md, lg, xl, or xxl. 
        /// none does not set a space while the other values set a space whose size increases in the order of listing. 
        /// The default value is the value of the spacing property of the parent box.
        /// If this component is the first component in the parent box, the margin property will be ignored.
        /// (Optional).
        /// Type is one of the <see cref="Spacing"/>
        /// </summary>
        public string Margin { get; set; }

        /// <summary>
        /// Maximum size of the icon width. 
        /// You can specify one of the following values: xxs, xs, sm, md, lg, xl, xxl, 3xl, 4xl, or 5xl. 
        /// The size increases in the order of listing. The default value is md.
        /// (Optional).
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Aspect ratio of the icon. You can specify one of the following values: 1:1, 2:1, or 3:1. The default value is 1:1.
        /// (Optional).
        /// </summary>
        /// Type is one of the <see cref="Models.AspectRatio"/>
        public AspectRatio aspectRatio { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="url">
        /// Image URL.
        /// Protocol: HTTPS
        /// Image format: JPEG or PNG
        /// Maximum image size: 240×240 pixels
        /// Maximum data size: 1 MB.
        /// 
        /// </param>
        public IconComponent(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public IconComponent()
        {
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Url);
        }
    }
}
