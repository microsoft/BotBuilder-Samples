using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Search.Models
{
#if !NETSTANDARD1_6
    [Serializable]
#else
    [DataContract]
#endif
    public class Canonicalizer
    {
        private readonly Dictionary<string, Synonyms> map = new Dictionary<string, Synonyms>();

        private string Normalize(string source)
        {
            return source.Trim().ToLower();
        }

        public Canonicalizer(params Synonyms[] synonyms)
        {
            foreach (var synonym in synonyms)
            {
                Add(synonym);
            }
        }

        public void Add(Synonyms synonyms)
        {
            foreach (var alt in synonyms.Alternatives)
            {
                var key = Normalize(alt);
                // TODO: allow multiple synonyms and generate a disjunction for query
                if (!map.ContainsKey(key))
                {
                    map.Add(Normalize(alt), synonyms);
                }
            }
        }

        public void Remove(Synonyms synonyms)
        {
            map.Remove(Normalize(synonyms.Canonical));
            foreach (var alt in synonyms.Alternatives)
            {
                map.Remove(Normalize(alt));
            }
        }

        public string Canonicalize(string source)
        {
            string canonical = null;
            Synonyms synonyms;
            if (source != null && map.TryGetValue(Normalize(source), out synonyms))
            {
                canonical = synonyms.Canonical;
            }
            return canonical;
        }

        public string CanonicalDescription(string source)
        {
            string description = null;
            Synonyms synonyms;
            if (source != null && map.TryGetValue(Normalize(source), out synonyms))
            {
                description = synonyms.Alternatives.FirstOrDefault();
            }
            if (description == null)
            {
                description = source;
            }
            return description;
        }
    }
}