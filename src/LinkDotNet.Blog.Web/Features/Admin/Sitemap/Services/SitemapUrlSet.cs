using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

[XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public sealed class SitemapUrlSet
{
    [XmlElement(ElementName = "url")]
    public List<SitemapUrl> Urls { get; set; } = new();
}
