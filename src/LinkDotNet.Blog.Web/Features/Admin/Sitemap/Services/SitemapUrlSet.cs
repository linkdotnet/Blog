using System.Collections.Generic;
using System.Xml.Serialization;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

[XmlRoot(ElementName = "urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
public sealed class SitemapUrlSet
{
    [XmlElement(ElementName = "url")]
#pragma warning disable CA1002
    public List<SitemapUrl> Urls { get; init; } = new();
#pragma warning restore CA1002
}
