using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Search.Models
{
#if !NETSTANDARD1_6
    [Serializable]
#else
    [DataContract]
#endif
    public class SearchSpec
    {
        private const int DefaultHitsPerPage = 5;
        private const int DefaultMaxFacets = 100;

        public int HitsPerPage { get; set; } = DefaultHitsPerPage;
        public int MaxFacets { get; set; } = DefaultMaxFacets;
        public List<string> Phrases { get; set; } = new List<string>();
        public FilterExpression Filter { get; set; }
        public List<SortKey> Sort { get; set; } = new List<SortKey>();
        public List<string> Selection { get; set; } = new List<string>();
        public int Skip { get; set; }
        public bool GetTotalCount { get; set; } = true;

        public int PageNumber
        {
            get { return Skip / HitsPerPage; }
            set { Skip = value * HitsPerPage;  }
        }

        public bool HasNoConstraints
        {
            get { return Filter == null && !Phrases.Any(); }
        }

        public void Remove(SearchField field)
        {
            if (Filter != null)
            {
                Filter = Filter.Remove(field.Name);
            }
            Sort.RemoveAll((s) => s.Field == field.Name);
        }

        public void Merge(SearchSpec other, FilterOperator filterCombine)
        {
            this.Phrases = this.Phrases.Union(other.Phrases).ToList();
            if (Filter == null)
            {
                this.Filter = other.Filter;
            }
            else if (other.Filter != null)
            {
                this.Filter = new FilterExpression(filterCombine, this.Filter, other.Filter);
            }
            this.Sort.AddRange(other.Sort);
            this.Selection.AddRange(other.Selection);
        }

        public SearchSpec DeepCopy()
        {
            return new SearchSpec
            {
                HitsPerPage = this.HitsPerPage,
                MaxFacets = this.MaxFacets,
                Filter = this.Filter?.DeepCopy(),
                Phrases = new List<string>(this.Phrases),
                Sort = new List<SortKey>(this.Sort),
                Selection = new List<string>(this.Selection),
                Skip = this.Skip,
            };
        }

        public override bool Equals(object obj)
        {
            var other = obj as SearchSpec;
            return other != null
                && Skip.Equals(other.Skip)
                && HitsPerPage.Equals(other.HitsPerPage)
                && (Filter == other.Filter || (Filter != null && Filter.Equals(other.Filter)))
                && Phrases.SequenceEqual(other.Phrases)
                && Sort.SequenceEqual(other.Sort)
                && Selection.SequenceEqual(other.Selection)
                ;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
