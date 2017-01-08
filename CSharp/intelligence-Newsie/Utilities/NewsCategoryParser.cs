using System;
using System.Collections.Generic;

namespace Newsie.Utilities
{
    public enum NewsCategory
    {
        None,
        Business,
        Health,
        World,
        Sports,
        Entertainment,
        Politics,
        ScienceAndTechnology
    }

    internal sealed class NewsCategoryParser
    {
        private static readonly Dictionary<string, NewsCategory> CategorySynonyms =
            new Dictionary<string, NewsCategory>(StringComparer.OrdinalIgnoreCase)
            {
                { "Business", NewsCategory.Business },
                { "Health", NewsCategory.Health },
                { "Sport", NewsCategory.Sports },
                { "Entertain", NewsCategory.Entertainment },
                { "Entertaining", NewsCategory.Entertainment },
                { "Politic", NewsCategory.Politics },
                { "ScienceAndTechnology", NewsCategory.ScienceAndTechnology },
                { "Science and Technology", NewsCategory.ScienceAndTechnology },
                { "Science", NewsCategory.ScienceAndTechnology },
                { "Technology", NewsCategory.ScienceAndTechnology },
                { "Tech", NewsCategory.ScienceAndTechnology },
                { "Tech.", NewsCategory.ScienceAndTechnology },
                { "Sci", NewsCategory.ScienceAndTechnology }
            };

        public static bool TryParse(string entity, out NewsCategory newsCategory)
        {
            return Enum.TryParse(entity, true, out newsCategory) || CategorySynonyms.TryGetValue(entity, out newsCategory);
        }
    }
}