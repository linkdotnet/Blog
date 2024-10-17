## Search Engine Optimization (SEO)

The blog includes some of the most important tags to get indexed by a crawler. Furthermore, some aspects of the Open Graph specification are implemented.

### Robots.txt

In the wwwroot/ you can find a default robots.txt. It allows the site gets completely indexed. If you want to tweak that behavior - feel free.
Also, you can provide a sitemap.xml to get a better ranking. The blog can create a sitemap.xml on its own. For that log in and click on the `Admin` button in the navigation bar and afterward on `Sitemap`. There you can let the blog create a new one for you. This is especially helpful after you created a new blog post to make it easier for indexers like Google.

### Open Graph Tags

To get better results when for example shared via LinkedIn some of the `<meta property="og:tag">` tags are implemented.

The following tags are set depending on the page:

| Open Graph Tag | Index                                                     | Display Blog Post                                                           |
| -------------- | --------------------------------------------------------- | --------------------------------------------------------------------------- |
| og:title       | Title of the blog (defined in Introduction)               | Title of the Blog Post                                                      |
| og:url         | Url to the index page                                     | Url of the page                                                             |
| og:image       | Profile image (defined in Introduction)                   | Uses the preview image. If a fallback is defined this will be used instead. |
| og:type        | article                                                   | article                                                                     |
| og:description | Short description in plain text (defined in Introduction) | Short Description of Blog Post in plain text                                |

Furthermore, the following tags are set:

| Tag                                      | Index                                | Display Blog Post             |
| ---------------------------------------- | ------------------------------------ | ----------------------------- |
| Title of the web page                    | Defined in AppConfiguration.BlogName | Title of the blogpost         |
| &lt;meta name="keyword" content="" /&gt; | not set                              | Tags defined in the Blog Post |

### RSS Feed

This blog also offers an RSS feed ([RSS 2.0 specification](https://validator.w3.org/feed/docs/rss2.html)), which can be consumed by your users or programs like Feedly. Just append `feed.rss` to your URL or click on the RSS feed icon in the navigation bar to get the feed. The RSS feed does not expose the whole content of a given blog post but its title and short description including some other tags like preview image, publishing date and so on.

### Sitemap

This blog offers to generate a [sitemap](https://developers.google.com/search/docs/crawling-indexing/sitemaps/build-sitemap) that lists all blog posts, the archive and pages of the blog. A sitemap can be generated in the Admin tab of the navigation bar under "Sitemap". This allows, especially new sites that don't have many inbound links, to be indexed easier by search engines.