using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Botframework.AdaptiveCards.Converter.Facebook.Helpers
{
    public static class StringUtilities
    {
        private const string Ellipsis = "...";
        private static readonly char[] WhiteSpace = " \t\n".ToCharArray();
        /// <summary>
        /// Ellipsize the current string to the given maxLength.  Looks for last space, but has a potential bound to avoid too aggressive truncation.
        /// </summary>
        /// <remarks>
        /// This is a dumb version - there is no I18N smarts here whatsoever.
        /// </remarks>
        /// <param name="input">Input string (this).</param>
        /// <param name="maxLength">Maximum length of the returned string (including ellipsis).</param>
        /// <param name="maxDistanceFromEnd">Maximum distance of last space from the end of the string in order to be eligible as the truncation point 
        /// (e.g. if we have "Bob went onareallylongjourneywithnospacesinit", we might keep that last word and just ellipsize in the middle, instead of just putting "Bob went...")</param>
        /// <returns></returns>
        public static string Ellipsize(this string input, int maxLength, int maxDistanceFromEnd = int.MaxValue)
        {
            if (input.Length <= maxLength)
            {
                return input;
            }
            else
            {
                var adjustedMaxLength = maxLength - Ellipsis.Length;
                int lastSpace = input.Substring(0, adjustedMaxLength).LastIndexOfAny(WhiteSpace);
                var distanceOfSpaceFromEnd = (maxLength - lastSpace);
                if (lastSpace > 0 && distanceOfSpaceFromEnd <= maxDistanceFromEnd)
                {
                    return input.Substring(0, lastSpace) + Ellipsis;
                }
                else
                {
                    return input.Substring(0, adjustedMaxLength) + Ellipsis;
                }
            }
        }
    }
}
