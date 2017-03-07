using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsieBot.Models.News
{
    public class BingNews
    {
        public string _type { get; set; }
        public Instrumentation instrumentation { get; set; }
        public string readLink { get; set; }
        public int totalEstimatedMatches { get; set; }
        public Value[] value { get; set; }
    }

    public class Instrumentation
    {
        public string pingUrlBase { get; set; }
        public string pageLoadPingUrl { get; set; }
    }

    public class Value
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public string description { get; set; }
        public About[] about { get; set; }
        public Provider[] provider { get; set; }
        public DateTime datePublished { get; set; }
        public string category { get; set; }
        public Image image { get; set; }
        public Clusteredarticle[] clusteredArticles { get; set; }
        public Mention1[] mentions { get; set; }
        public Video video { get; set; }
    }

    public class Image
    {
        public Thumbnail thumbnail { get; set; }
    }

    public class Thumbnail
    {
        public string contentUrl { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Video
    {
        public string name { get; set; }
        public string thumbnailUrl { get; set; }
        public string embedHtml { get; set; }
        public bool allowHttpsEmbed { get; set; }
        public Thumbnail1 thumbnail { get; set; }
    }

    public class Thumbnail1
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class About
    {
        public string readLink { get; set; }
        public string name { get; set; }
    }

    public class Provider
    {
        public string _type { get; set; }
        public string name { get; set; }
    }

    public class Clusteredarticle
    {
        public string name { get; set; }
        public string url { get; set; }
        public string urlPingSuffix { get; set; }
        public string description { get; set; }
        public About1[] about { get; set; }
        public Mention[] mentions { get; set; }
        public Provider1[] provider { get; set; }
        public DateTime datePublished { get; set; }
        public string category { get; set; }
    }

    public class About1
    {
        public string readLink { get; set; }
        public string name { get; set; }
    }

    public class Mention
    {
        public string name { get; set; }
    }

    public class Provider1
    {
        public string _type { get; set; }
        public string name { get; set; }
    }

    public class Mention1
    {
        public string name { get; set; }
    }
}