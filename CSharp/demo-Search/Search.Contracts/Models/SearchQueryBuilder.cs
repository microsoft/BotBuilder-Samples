namespace Search.Models
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class SearchQueryBuilder
    {
        private const int DefaultHitPerPage = 5;

        public SearchQueryBuilder()
        {
            this.Refinements = new Dictionary<string, IEnumerable<string>>();
        }

        public string SearchText { get; set; }

        public int PageNumber { get; set; }

        public int HitsPerPage { get; set; } = DefaultHitPerPage;

        public Dictionary<string, IEnumerable<string>> Refinements { get; private set; }

        public virtual void Reset()
        {
            this.SearchText = null;
            this.PageNumber = 0;
            this.Refinements.Clear();
        }
    }
}