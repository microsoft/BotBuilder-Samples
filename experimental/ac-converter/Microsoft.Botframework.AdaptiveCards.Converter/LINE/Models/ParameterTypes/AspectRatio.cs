using AdaptiveCards;

using System;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models
{
    /// <summary>
    /// Aspect ratio of the image component, h/w must be in range 0-3.
    /// </summary>
    public class AspectRatio
    {
        private string separator = ":";

        public int Width { get; set; }
        public int Height { get; set; }

        public AspectRatio(int width, int height)
        {
            if (height / width < 0 || height / width > 3)
            {
                throw new ArgumentException("h/w must between 0 to 3");
            }

            this.Width = width;
            this.Height = height;
        }

        public override string ToString()
        {
            return $"{Width}{separator}{Height}";
        }
    }
}
