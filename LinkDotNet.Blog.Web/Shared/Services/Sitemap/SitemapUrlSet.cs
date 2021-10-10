using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkDotNet.Blog.Web.Shared.Services.Sitemap
{
    [XmlRoot(ElementName="urlset")]
    public class SitemapUrlSet
    {
        [XmlElement(ElementName = "url")]
        public List<SitemapUrl> Urls { get; set; } = new();

        [XmlAttribute(AttributeName="xmlns")]
        public string Namespace { get; set; }
    }
}