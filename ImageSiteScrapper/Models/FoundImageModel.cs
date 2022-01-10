using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSiteScraper.Models
{
    public class FoundImageModel
    {
        public string imageUrl { get; set; }
        public List<string> existsOnUrls { get; set; }
    }
}
