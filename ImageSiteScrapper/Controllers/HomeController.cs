using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ImageSiteScraper.Models;
using System.Net.Http;
using System.Net;
using HtmlAgilityPack;

namespace ImageSiteScraper.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private HashSet<string> urls = new HashSet<string>();
        private List<FoundImageModel> foundImageList;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //Search first url
            //Once all links grabbed search next one on the list.
            //string url = "http://localhost:3000/";
            string url = "https://reactjs.org/";
            urls.Add(url);
            HashSet<string>.Enumerator urlEnum = urls.GetEnumerator();
            while (urlEnum.MoveNext()) {
                var response = CallUrl(urlEnum.Current).Result;
                var allLinks = GetHrefTags(response);
                var allImageUrls = GetImgTags(response);
            }
           
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13;
            client.DefaultRequestHeaders.Accept.Clear();
            var response = client.GetStringAsync(fullUrl);
            return await response;
        }
        public IEnumerable<string> GetHrefTags(string htmlString)
        {
            HtmlDocument htmlSnippet = new HtmlDocument();
            htmlSnippet.LoadHtml(htmlString);

            List<string> hrefTags = new List<string>();
            var aHrefList = htmlSnippet.DocumentNode.SelectNodes("//a[@href]");
            if (aHrefList != null)
            {
                foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes("//a[@href]"))
                {
                    HtmlAttribute att = link.Attributes["href"];
                    urls.Add(att.Value);
                    hrefTags.Add(att.Value);
                }
            }

            return urls;
        }

        public IEnumerable<string> GetImgTags(string htmlString)
        {
            HtmlDocument htmlSnippet = new HtmlDocument();
            htmlSnippet.LoadHtml(htmlString);

            List<string> hrefTags = new List<string>();
            var imageTagList = htmlSnippet.DocumentNode.SelectNodes("//img[@src]");
            if (imageTagList != null)
            {
                foreach (HtmlNode link in imageTagList)
                {
                    HtmlAttribute att = link.Attributes["src"];
                    hrefTags.Add(att.Value);
                }
            }
            return hrefTags;
        }
    }
}
