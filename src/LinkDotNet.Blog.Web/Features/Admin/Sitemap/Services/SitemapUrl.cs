using System.Xml.Serialization;

namespace LinkDotNet.Blog.Web.Features.Admin.Sitemap.Services;

[XmlRoot(ElementName = "url")]
public class SitemapUrl
{
    [XmlElement(ElementName = "loc")]
    public string Location { get; set; }

    [XmlElement(ElementName = "lastmod")]
    public string LastModified { get; set; }
}
