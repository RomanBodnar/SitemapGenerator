using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace SitemapGenerator.Infrastructure
{
    public class LinkParser
    {
        private List<string> all_links = new List<string>();
        private List<string> relativeLinks = new List<string>();
        private HtmlDocument html = new HtmlDocument();

        public List<string> ParseLinks(Page page)
        {
            all_links.Clear();
            relativeLinks.Clear();
            List<string> result_links = new List<string>();

            html.LoadHtml(page.Text);
            var root_node = html.DocumentNode.SelectNodes("//a[@href]");
            if (root_node != null)
            {
                foreach (var link in root_node)
                {
                    all_links.Add(link.Attributes["href"].Value);
                }
            }
            relativeLinks = (from link in all_links
                             where (IsAWebPage(link)) && !link.StartsWith("mailto:")
                             && (link.StartsWith(SiteName(page.Url)) || !link.Contains("http"))
                             select link).ToList<string>();
            foreach (string link in relativeLinks)
            {
                result_links.Add(FixPath(link, page.Url));
            }
            return result_links;
        }

        private bool IsAWebPage(string foundHref)
        {
            if (foundHref.IndexOf("javascript:") == 0)
                return false;

            string extension = foundHref.Substring(foundHref.LastIndexOf(".") + 1,
                                                   foundHref.Length - foundHref.LastIndexOf(".") - 1);
            switch (extension)
            {
                case "jpg":
                case "css":
                    return false;
                default:
                    return true;
            }

        }
        private string SiteName(string url)
        {
            int first_entry = url.IndexOf("/");
            if (first_entry != -1)
                first_entry += 2;
            int second_entry = url.IndexOf("/", first_entry);
            if (second_entry == -1)
            {
                return url;
            }
            try
            {
                string result_string = url.Substring(0, second_entry);
                return result_string;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.Source);
            }
            return "";

        }
        private string FixPath(string link, string originUrl)
        {
            if (link.StartsWith(SiteName(originUrl)))
            {
                return link;
            }
            if (link.StartsWith("/"))
            {
                return string.Format(SiteName(originUrl) + link);
            }
            if (link.StartsWith("../"))
            {
                return string.Format(SiteName(originUrl) + "/" + link.Substring(3, link.Length - 3));
            }
            else
            {
                return string.Format(SiteName(originUrl) + "/" + link);
            }
        }
    }
}