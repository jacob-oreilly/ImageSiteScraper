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
        private static HashSet<string> urls = new HashSet<string>();
        private static List<FoundImageModel> foundImageList = new List<FoundImageModel>();
        private string rootUrl = "";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //Search first url
            //Once all links grabbed search next one on the list.
            //string url = "http://localhost:3000/";
            string url = "https://www.cyotek.com/";

            //urls.Add(url);
            HashSet<string>.Enumerator urlEnum = urls.GetEnumerator();

            Crawling(urls, url, 0, 20, url);

            //while (urlEnum.MoveNext()) {

            //}
            //var response = CallUrl(url).Result;
            //var allLinks = GetHrefTags(response, url);
            //var allImageUrls = GetImgTags(response);
            //var currentPageHashset = new HashSet<string>(urls);
            //foreach (var currentItem in currentPageHashset) {
            //    response = CallUrl(currentItem).Result;
            //    allLinks = GetHrefTags(response, url, url);
            //    allImageUrls = GetImgTags(response);
            //}

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
            var response = client.GetAsync(fullUrl);
            //var response = client.GetStringAsync(fullUrl);
            if (response.Result.IsSuccessStatusCode)
            {
                return await response.Result.Content.ReadAsStringAsync();
            }
            else {
                return "";
            }
        }
        public static IEnumerable<string> GetHrefTags(string htmlString, string currentUrl, string rootUrl)
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
                    if (!att.Value.Equals("/"))
                    {
                        if ((att.Value.Contains("https://") || att.Value.Contains("http://") || att.Value.Contains("www")) && !att.Value.Contains("rootUrl"))
                        {
                            return urls;
                        }
                        else if (!(att.Value.Contains("https://") || att.Value.Contains("http://") || att.Value.Contains("www")) && !att.Value.Contains(rootUrl))
                        {
                            if (!att.Value.StartsWith("/"))
                            {
                                urls.Add(rootUrl + "/" + att.Value);
                            }
                            else if (rootUrl.EndsWith("/")) {
                                urls.Add(rootUrl + att.Value.Remove(0, 1));
                            }
                            else
                            {
                                urls.Add(rootUrl + att.Value);
                            }
                        }
                    }
                    //else {
                    //    urls.Add(att.Value);
                    //}
                    hrefTags.Add(att.Value);
                }
            }

            return urls;
        }

        public static IEnumerable<string> GetImgTags(string htmlString)
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

        public static void Crawling(HashSet<string> siteUrls, string currentUrl, int currentCrawlDepth, int maxCrawlDepth, string rootUrl) {
            var response = CallUrl(currentUrl).Result;
            var allLinks = GetHrefTags(response, currentUrl, rootUrl);
            var allImageUrls = GetImgTags(response);
            //Add images and links associate with the images into the imagelistobject.
            var currentPageHashset = new HashSet<string>(siteUrls);
            if (currentCrawlDepth < maxCrawlDepth) {
                var count = 0;
                foreach (var url in currentPageHashset) {
                    if (currentCrawlDepth.Equals(count)) {
                        Crawling(siteUrls, url, currentCrawlDepth + 1, maxCrawlDepth, rootUrl);
                    }
                    count++;
                }
            }
        }
    }
}
