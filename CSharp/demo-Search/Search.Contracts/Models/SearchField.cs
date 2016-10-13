using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Search.Models
{
    public enum PreferredFilter { None, MinValue, MaxValue, RangeMin, RangeMax, Range };

    [Serializable]
    public class SearchField
    {
        public string Name;
        public Type Type;
        public bool IsFacetable;
        public bool IsFilterable;
        public bool IsKey;
        public bool IsRetrievable;
        public bool IsSearchable;
        public bool IsSortable;

        // Fields to control experince
        public PreferredFilter FilterPreference;
    }
}
