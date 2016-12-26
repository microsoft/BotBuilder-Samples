using System;
using System.Collections.Generic;

namespace Newsie.Utilities
{
    public enum Categories
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

    internal sealed class LuisCategoryParser
    {
        private static readonly Dictionary<string, Categories> CategorySynonyms =
            new Dictionary<string, Categories>(StringComparer.OrdinalIgnoreCase)
            {
                { "Business", Categories.Business },
                { "Health", Categories.Health },
                { "Sport", Categories.Sports },
                { "Entertain", Categories.Entertainment },
                { "Entertaining", Categories.Entertainment },
                { "Politic", Categories.Politics },
                { "ScienceAndTechnology", Categories.ScienceAndTechnology },
                { "Science and Technology", Categories.ScienceAndTechnology },
                { "Science", Categories.ScienceAndTechnology },
                { "Technology", Categories.ScienceAndTechnology },
                { "Tech", Categories.ScienceAndTechnology },
                { "Tech.", Categories.ScienceAndTechnology },
                { "Sci", Categories.ScienceAndTechnology }
            };

        public static bool TryParse(string entity, out Categories category)
        {
            return Enum.TryParse(entity, true, out category) || CategorySynonyms.TryGetValue(entity, out category);
        }
    }
}