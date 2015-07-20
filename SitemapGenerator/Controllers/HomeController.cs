using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Xml.Linq;
using SitemapGenerator.Infrastructure;
using System.Net;

namespace SitemapGenerator.Controllers
{
    public class HomeController : Controller
    {
        private Crawler crawler = new Crawler();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> GetXml(string link)
        {
            if (RemoteUrlExists(link))
            {
                await crawler.CrawlPage(link);
                GenerataXmlSitemap(crawler.InnerLinks);
                return View();
            }
            else
            {
                TempData["Message"] = "Сталась помилка під час обробки запиту. " +
                                      "Будь ласка, перевірте посилання на коректність (має містити http:// або https://).";
                return View("Index");
            }
            
        }
        public FileResult Download()
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + "Store/sitemap.xml";
            string fileType = "text/xml";
            string fileName = "sitemap.xml";
            return File(filePath, fileType, fileName);
        }
        private void GenerataXmlSitemap(IEnumerable<string> links)
        {
            XDocument xdoc = new XDocument();

            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            XElement urlset = new XElement(ns + "urlset");
            foreach (string link in links)
            {
                urlset.Add(new XElement(ns + "url", new XElement(ns + "loc", System.Security.SecurityElement.Escape(link))));
            }
            xdoc.Add(urlset);
            xdoc.Save(AppDomain.CurrentDomain.BaseDirectory + "Store/sitemap.xml");
        }
        private bool RemoteUrlExists(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //Returns TRUE if the Status code == 200
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                //Any exception will returns false.
                return false;
            }
        }
    }
}