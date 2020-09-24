using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catering
{
    public class CosmosResult<T>
    {
        public string ContinuationToken { get; set; }
        public IList<T> Items { get; set; }
    }
}
