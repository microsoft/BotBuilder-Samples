using System.Runtime.Serialization;

namespace Search.Models
{
    using System;
    using System.Collections.Generic;

#if !NETSTANDARD1_6
    [Serializable]
#else
    [DataContract]
#endif
    public class SearchHit
    {
        public SearchHit()
        {
            this.PropertyBag = new Dictionary<string, object>();
        }

        public string Key { get; set; }

        public string Title { get; set; }

        public string PictureUrl { get; set; }

        public string Description { get; set; }

        public IDictionary<string, object> PropertyBag { get; set; }
    }
}