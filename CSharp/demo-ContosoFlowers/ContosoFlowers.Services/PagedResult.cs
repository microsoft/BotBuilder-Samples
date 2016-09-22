namespace ContosoFlowers.Services
{
    using System.Collections.Generic;

    public class PagedResult<R>
    {
        public IEnumerable<R> Items { get; set; }

        public int TotalCount { get; set; }
    }
}