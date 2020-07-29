using AdaptiveCards;
using Microsoft.Botframework.AdaptiveCards.Converter.LINE.Models;

namespace Microsoft.Botframework.AdaptiveCards.Converter.LINE
{
    public static class LineCardConverterHelper
    {
        public static string ToAlign(AdaptiveHorizontalAlignment adaptiveHorizontalAlignment)
        {
            switch (adaptiveHorizontalAlignment)
            {
                case AdaptiveHorizontalAlignment.Left:
                    return Align.Start;
                case AdaptiveHorizontalAlignment.Center:
                    return Align.Center;
                case AdaptiveHorizontalAlignment.Right:
                    return Align.End;
                default:
                    return Align.Start;
            }
        }

        public static string ToImageSize(AdaptiveImageSize adaptiveImageSize)
        {
            switch (adaptiveImageSize)
            {
                case AdaptiveImageSize.Small:
                    return ImageSize.Xxs;
                case AdaptiveImageSize.Medium:
                    return ImageSize.Xs;
                case AdaptiveImageSize.Large:
                    return ImageSize.Lg;
                case AdaptiveImageSize.Stretch:
                    return ImageSize.Full;
                default:

                    // Default Adaptive Card image size is 'auto'. But Line cannot support.
                    // We choose 'md' as default size.
                    return ImageSize.Md;
            }
        }

        public static string ToColorHex(AdaptiveTextColor adaptiveTextColor)
        {
            switch (adaptiveTextColor)
            {
                case AdaptiveTextColor.Default:
                    return ColorCode.Black;
                case AdaptiveTextColor.Dark:
                    return ColorCode.Black;
                case AdaptiveTextColor.Light:
                    return ColorCode.White;
                case AdaptiveTextColor.Accent:
                    return ColorCode.Accent;
                case AdaptiveTextColor.Good:
                    return ColorCode.Good;
                case AdaptiveTextColor.Warning:
                    return ColorCode.Warning;
                case AdaptiveTextColor.Attention:
                    return ColorCode.Red;
                default:
                    return ColorCode.Black;
            }
        }

        public static string ToSubtleColorHex(AdaptiveTextColor adaptiveTextColor)
        {
            switch (adaptiveTextColor)
            {
                case AdaptiveTextColor.Default:
                    return ColorCode.SubtleBlack;
                case AdaptiveTextColor.Dark:
                    return ColorCode.SubtleDark;
                case AdaptiveTextColor.Light:
                    return ColorCode.SubtleLight;
                case AdaptiveTextColor.Accent:
                    return ColorCode.SubtleAccent;
                case AdaptiveTextColor.Good:
                    return ColorCode.SubtleGood;
                case AdaptiveTextColor.Warning:
                    return ColorCode.SubtleWarning;
                case AdaptiveTextColor.Attention:
                    return ColorCode.SubtleAttention;
                default:
                    return ColorCode.SubtleBlack;
            }
        }

        public static string ToWeight(AdaptiveTextWeight adaptiveTextWeight)
        {
            switch (adaptiveTextWeight)
            {
                case AdaptiveTextWeight.Default:
                    return Weight.Regular;
                case AdaptiveTextWeight.Bolder:
                    return Weight.Bold;
                default:
                    return Weight.Regular;
            }
        }

        public static string ToTextSize(AdaptiveTextSize adaptiveTextSize)
        {
            switch (adaptiveTextSize)
            {
                case AdaptiveTextSize.Small:
                    return FontSize.Xxs;
                case AdaptiveTextSize.Default:
                    return FontSize.Xs;
                case AdaptiveTextSize.Medium:
                    return FontSize.Sm;
                case AdaptiveTextSize.Large:
                    return FontSize.Md;
                case AdaptiveTextSize.ExtraLarge:
                    return FontSize.Lg;
                default:
                    return FontSize.Xs;
            }
        }
    }
}
