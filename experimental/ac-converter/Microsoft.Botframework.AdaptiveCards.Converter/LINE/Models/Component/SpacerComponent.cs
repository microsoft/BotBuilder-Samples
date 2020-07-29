using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    public class SpacerComponent : IFlexComponent
    {
        public string Type => FlexComponentType.Spacer;

        /// <summary>
        /// Size of the space.(Required).  
        /// You can specify one of the following values: xs, sm, md, lg, xl, or xxl. 
        /// The size increases in the order of listing. The default value is md.
        /// (Required).
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="size">
        /// /// Size of the space.(Required).  
        /// You can specify one of the following values: xs, sm, md, lg, xl, or xxl. 
        /// The size increases in the order of listing. The default value is md.
        /// </param>
        public SpacerComponent(string size)
        {
            Size = size;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SpacerComponent()
        {
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Size);
        }
    }
}
