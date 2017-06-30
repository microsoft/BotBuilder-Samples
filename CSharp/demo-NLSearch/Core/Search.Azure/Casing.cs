using System.Text;

namespace Search.Utilities
{
    public class Casing
    {
        /// <summary>
        /// Break a string into words based on _ and case changes.
        /// </summary>
        /// <param name="original">Original string.</param>
        /// <returns>String with words on case change or _ boundaries.</returns>
        public static string CamelCase(string original)
        {
            var builder = new StringBuilder();
            var name = original.Trim();
            var previousUpper = char.IsUpper(name[0]);
            var previousLetter = char.IsLetter(name[0]);
            var first = true;
            for (var i = 0; i < name.Length; ++i)
            {
                var ch = name[i];
                if (!first && (ch == '_' || ch == ' '))
                {
                    // Non begin _ as space
                    builder.Append(' ');
                }
                else
                {
                    var isUpper = char.IsUpper(ch);
                    var isLetter = char.IsLetter(ch);
                    if ((!previousUpper && isUpper)
                        || (isLetter != previousLetter)
                        || (!first && isUpper && i + 1 < name.Length && char.IsLower(name[i + 1])))
                    {
                        // Break on lower to upper, number boundaries and Upper to lower
                        builder.Append(' ');
                    }
                    previousUpper = isUpper;
                    previousLetter = isLetter;
                    builder.Append(ch);
                    if (first)
                    {
                        first = false;
                    }
                }
            }
            return builder.ToString();
        }
    }
}