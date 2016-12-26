using Newsie.Utilities;

namespace Newsie.Extensions
{
    internal static class CategoriesExtentions
    {
        public static string GetDislaplyName(this Categories @enum)
        {
            switch (@enum)
            {
                case Categories.ScienceAndTechnology:
                    return "Science and Technology";
                default:
                    return @enum.ToString();
            }
        }
    }
}