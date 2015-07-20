using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitemapGenerator.Infrastructure
{
    public class Page
    {
        public Page()
        {
            InternalLinks = new List<string>();
        }
        public string Url { get; set; }
        public string Text { get; set; }
        public List<string> InternalLinks { get; set; }
    }
}