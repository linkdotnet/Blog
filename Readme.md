# LinkDotNet.Blog

[![.NET](https://github.com/linkdotnet/Blog/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/linkdotnet/Blog/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/linkdotnet/Blog/actions/workflows/codeql.yml/badge.svg)](https://github.com/linkdotnet/Blog/actions/workflows/codeql.yml)

This is a blog software completely written in C# / Blazor. The aim is to have it configurable as possible.

## How does it work

The basic idea is that the content creator writes his posts in markdown language (like this readme file).
The markdown will then be translated into HTML and displayed to the client. This gives an easy entry to writing posts with all the flexibility markdown has.
This also includes source code snippets. Highlighting is done via [highlight.js](https://highlightjs.org/) with the GitHub theme.

## In Action

![overview](assets/overview.gif)

## Components

-   [Authorization](./docs/Authorization/Readme.md)
-   [Comments](./docs/Comments/Readme.md)
-   [Storage Provider](./docs/Storage/Readme.md)
-   [Search Engine Optimization (SEO)](./docs/SEO/Readme.md)
-   [Setup](./docs/Setup/Readme.md)

                          |

## Storage Provider

Currently, there are 5 Storage-Provider:

-   InMemory - Basically a list holding your data (per request). If the User hits a hard reload, the data is gone.
-   RavenDb - As the name suggests for RavenDb. RavenDb automatically creates all the documents, if a database name is provided.
-   Sqlite - Based on EF Core, it can be easily adapted for other Sql Dialects. The tables are automatically created.
-   SqlServer - Based on EF Core, it can be easily adapted for other Sql Dialects. The tables are automatically created.
-   MySql - Based on EF Core - also supports MariaDB.

The default (when you clone the repository) is the `InMemory` option. That means every time you restart the service, all posts and related objects are gone.

## Donations

The blog software allows you to integrate via different micro-transaction services. The following chapter will show you how to set up donations.

### Ko-fi

You can use [Ko-fi](https://Ko-fi.com/) as a payment service to receive donations. To acquire the `KofiToken` as seen in the config above, head to [widgets page](https://Ko-fi.com/manage/widgets), click on "Ko-fi Button".
Now choose "Image" as the type. In the field below under `Copy & Paste Code` you see an `<a href='https://ko-fi.com/XYZ'` tag. Just take the `XYZ` part and put it into `KofiToken`.

### GitHub Sponsor

Enables the usage of [GitHub Sponsors](https://github.com/sponsors) as a payment service to receive donations. Only pass in your username. The button will use the following url: `https://github.com/sponsors/{your-user-name}`.

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

## RSS Feed

This blog also offers an RSS feed ([RSS 2.0 specification](https://validator.w3.org/feed/docs/rss2.html)), which can be consumed by your users or programs like Feedly. Just append `feed.rss` to your URL or click on the RSS feed icon in the navigation bar to get the feed. The RSS feed does not expose the whole content of a given blog post but its title and short description including some other tags like preview image, publishing date and so on.

Note the ConnectionString format of SQL Server needs to be consistent:

```
"ConnectionString": "Data Source=sql;Initial Catalog=master;User ID=sa;Password=<YOURPASSWORD>;TrustServerCertificate=True;MultiSubnetFailover=True"
```

For MySql use the following:

```
"PersistenceProvider": "MySql"
"ConnectionString": "Server=YOURSERVER;User ID=YOURUSERID;Password=YOURPASSWORD;Database=YOURDATABASE"
```
