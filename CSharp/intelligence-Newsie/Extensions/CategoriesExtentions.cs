using NewsieBot.Utilities;

namespace NewsieBot.Extensions
{
    internal static class CategoriesExtentions
    {
        public static string GetDislaplyName(this NewsCategory @enum)
        {
            switch (@enum)
            {
                case NewsCategory.ScienceAndTechnology:
                    return "Science and Technology";
                default:
                    return @enum.ToString();
            }
        }

        public static string GetQueryName(this NewsCategory @enum)
        {
            switch (@enum)
            {
                case NewsCategory.ScienceAndTechnology:
                    return "Science Technology";
                default:
                    return @enum.ToString();
            }
        }
    }
}