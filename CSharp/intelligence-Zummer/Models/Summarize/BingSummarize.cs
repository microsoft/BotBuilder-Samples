using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zummer.Models.Summarize
{
    public class BingSummarize
    {
        public int StatusCode { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public Datum[] Data { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class Datum
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public float StaticScore { get; set; }
        public float RankingScore { get; set; }
        public float SummarizingScore { get; set; }
    }
}




