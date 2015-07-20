using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace SitemapGenerator.Infrastructure
{
    public class Crawler
    {
        private SortedSet<string> crawledUrls = new SortedSet<string>();
        private Queue<Page> pagesToCrawl = new Queue<Page>();
        private int DepthLevel = 3;

        public SortedSet<string> InnerLinks { get { return crawledUrls; } }
        
        public async Task CrawlPage(string site)
        {
            Page fpage = new Page();
            LinkParser linkParser = new LinkParser();

            fpage.Url = site;
            fpage.Text = await GetPageText(fpage.Url);

            if (fpage.Text != string.Empty)
            {
                fpage.InternalLinks = linkParser.ParseLinks(fpage);
            }
            int lvl = DepthLevel;

            crawledUrls.Add(fpage.Url);
            pagesToCrawl.Enqueue(fpage);
            while (pagesToCrawl.Count != 0 && lvl >= 0)
            {

                Page p = pagesToCrawl.Dequeue();
                foreach (string link in p.InternalLinks)
                {
                    string url = link;
                    if (!crawledUrls.Contains(url))
                    {
                        Page spage = new Page();
                        spage.Url = url;
                        spage.Text = await GetPageText(url);
                        Console.WriteLine(url);
                        if (spage.Text != string.Empty)
                        {
                            spage.InternalLinks = linkParser.ParseLinks(spage);
                        }
                        pagesToCrawl.Enqueue(spage);
                        crawledUrls.Add(spage.Url);

                    }
                }
                lvl--;
            }
        }
        
        private async Task<string> GetPageText(string page)
        {
            string result = "";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(page);
                using (WebResponse response = await request.GetResponseAsync().ConfigureAwait(false))
                {

                    using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                    {
                        result = await stream.ReadToEndAsync().ConfigureAwait(false);
                        return result;
                    }
                }
            }
            catch (WebException)
            {
                return "";
            }
            catch (UriFormatException)
            {
                return "";
            }
        }

    }
}