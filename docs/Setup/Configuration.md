### Configurations

### appsettings.json

The appsettings.json file has a lot of options to customize the content of the blog. The following table shows which values are used when.

Browser spellchecking uses the `<html lang="en">` value in
`src/LinkDotNet.Blog.Web/App.razor`. Change `en` to your blog's language so the browser selects the correct dictionary.

```json
{
	"BlogName": "linkdotnet",
	"BlogBrandUrl": "http//some.url/image.png",
	"GithubAccountUrl": "",
	"Social": {
		"GithubAccountUrl": "",
		"LinkedInAccountUrl": "",
		"TwitterAccountUrl": "",
		"YoutubeAccountUrl": "",
	},
	"Introduction": {
		"Description": "Some nice text about yourself. Markup can be used [Github](https://github.com/someuser/somerepo)",
		"BackgroundUrl": "assets/profile-background.webp",
		"ProfilePictureUrl": "assets/profile-picture.webp"
	},
	"PersistenceProvider": "Sqlite",
	"ConnectionString": "Data Source=blog.db",
	"DatabaseName": "",
	"Authentication": {
		"Provider": "PROVIDER_NAME",
		"Domain": "",
		"ClientId": "",
		"ClientSecret": "",
		"LogoutUri": ""
	},
	"BlogPostsPerPage": 10,
	"FirstPageCacheDurationInMinutes": 10,
	"ProfileInformation": {
		"Name": "Steven Giesel",
		"Heading": "Software Engineer",
		"ProfilePictureUrl": "assets/profile-picture.webp"
	},
	"Giscus": {
		"Repository": "github/repo",
		"RepositoryId": "id",
		"Category": "general",
		"CategoryId": "id"
	},
	"Disqus": {
		"Shortname": "blog"
	},
	"ShowReadingIndicator": true,
	"SimlarBlogPosts": "true",
	"ShowBuildInformation": true,
	"SupportMe": {
		"KofiToken": "ABC123",
		"GithubSponsorName": "your-tag-here",
		"PatreonName": "your-tag-here",
		"ShowUnderBlogPost": true,
		"ShowUnderIntroduction": true,
		"ShowInFooter": true,
		"ShowSupportMePage": true,
		"SupportMePageDescription": "Buy my book here: [My Blazor Book](https://google.com) or please contribute to my open-source project here: [My Awesome Repo](https://github.com) . This can be **markdown**."
	},
	"ImageStorageProvider": "Azure",
	"ImageStorage" : {
		"AuthenticationMode": "Default",
		"ConnectionString": "",
		"ServiceUrl": "",
		"ContainerName": "",
		"CdnEndpoint": ""
	},
	"UseMultiAuthorMode": false,
	"EnableTagDiscoveryPanel": true,
	"EnableBrokenLinkChecker": true,
	"ShowCodeBlockLanguage": true,
	"EnableMarkdownForLlms": false
}
```

| Property                                      | Type           | Description                                                                                                                                                                      |
| --------------------------------------------- | -------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| BlogName                                      | string         | Name of your blog. Is used in the navbar and is used as the title of the page. Will not be shown when `BlogBrandUrl` is set                                                      |
| BlogBrandUrl                                  | string         | The url to an image which is used as a brand image in the navigation bar. If not set or `null` the `BlogName` will be shown. See [Styling.md](../Features/Styling.md#brand-image-vs-blog-name) for a visual example                                                      |
| Social                                        | node           | Represents all possible linked social accounts                                                                                                                                   |
| GithubAccountUrl                              | string         | Url to your github account. If not set it is not shown in the introduction card                                                                                                  |
| LinkedInAccountUrl                            | string         | Url to your LinkedIn account. If not set it is not shown in the introduction card                                                                                                |
| TwitterAccountUrl                             | string         | Url to your Twitter account. If not set it is not shown in the introduction card                                                                                                 |
| YoutubeAccountUrl                             | string         | Url to your Youtube account. If not set it is not shown in the introduction card                                                                                                 |
| Introduction                                  |                | Is used for the introduction part of the blog                                                                                                                                    |
| Description                                   | MarkdownString | Small introduction text for yourself. This is also used for `<meta name="description">` tag. For this the markup will be converted to plain text                                 |
| BackgroundUrl                                 | string         | Url or path to the background image. (Optional). See [Styling.md](../Features/Styling.md#introduction-background-image) for a visual example                                    |
| ProfilePictureUrl                             | string         | Url or path to your profile picture                                                                                                                                              |
| [PersistenceProvider](./../Storage/Readme.md) | string         | Declares the type of the storage provider (one of the following: `SqlServer`, `Sqlite`, `RavenDb`, `MongoDB`, `MySql`, `PostgreSql`). More in-depth explanation [here](./../Storage/Readme.md) |
| ConnectionString                              | string         | Is used for connection to a database.                                                                                                                                            |
| DatabaseName                                  | string         | Name of the database. Only used with `RavenDbStorageProvider`                                                                                                                    |
| [AuthProvider](./../Authorization/Readme.md)  | string         |                                                                                                                                                                                  |
| [PROVIDER_NAME](./../Authorization/Readme.md) | string         |                                                                                                                                                                                  |
| Domain                                        | string         |                                                                                                                                                                                  |
| ClientId                                      | string         |                                                                                                                                                                                  |
| ClientSecret                                  | string         |                                                                                                                                                                                  |
| LogoutUri                                     | string         |                                                                                                                                                                                  |
| BlogPostsPerPage                              | int            | Gives the amount of blog posts loaded and display per page. For more the user has to use the navigation                                                                          |
| FirstPageCacheDurationInMinutes               | int            | The duration in minutes the first page is cached.                                                                                                                                |
| AboutMeProfileInformation                     | node           | Sets information for the About Me Page. If omitted the page is disabled completely                                                                                               |
| Name                                          | string         | Name, which is displayed on top of the profile card                                                                                                                              |
| Heading                                       | string         | Displayed under the name. For example job title                                                                                                                                  |
| ProfilePictureUrl                             | string         | Displayed profile picture                                                                                                                                                        |
| [Giscus](./../Comments/Giscus.md)             | node           | Enables the comment section via giscus. If left empty the comment section will not be shown.                                                                                     |
| [Disqus](./../Comments/Disqus.md)             | node           | Enables the comment section via disqus. If left empty the comment section will not be shown.                                                                                     |
| ShowReadingIndicator                          | boolean        | If set to `true` (default) a circle indicates the progress when a user reads a blog post (without comments).                                                                     |
| SimilarBlogPosts                              | boolean        | If set to `true` (default) similar blog posts are shown at the end of a blog post.                                                                                               |
| ShowBuildInformation                          | boolean        | If set to `true` (default) the build information (target framework and build timestamp) is shown in the footer.                                                                  |
| [SupportMe](./../Donations/Readme.md)         | node           | Donation sections configuration. If left empty no donation sections will not be shown.                                                                                           |
| [ImageStorageProvider](./../Media/Readme.md)  | string         | Declares the type of the image storage provider (currently only `Azure`).                                                                                                        |
| [ImageStorage](./../Media/Readme.md)          | node           | Configuration for the image storage provider.                                                                                                                                    |
| AuthenticationMode                            | string         | The authentication mode for the image storage provider. Either `Default` or `ConnectionString`                                                                                   |
| ConnectionString                              | string         | The connection string for the image storage provider. Only used if `AuthenticationMode` is set to `ConnectionString`                                                             |
| ServiceUrl                                    | string         | The host url of the Azure blob storage. Only used if `AuthenticationMode` is set to `Default`                                                                                    |
| ContainerName                                 | string         | The container name for the image storage provider                                                                                                                                |
| CdnEndpoint                                   | string         | Optional CDN endpoint to use for uploaded images. If set, the blog will return this URL instead of the storage account URL for uploaded assets.                                  |
| UseMultiAuthorMode                            | boolean        | The default value is `false`. If set to `true` then author name will be associated with blog posts at the time of creation. This author name will be fetched from the identity provider's `name` or `nickname` or `preferred_username` claim property. |
| EnableTagDiscoveryPanel                       | boolean        | The default value is `true`. Enables the Tag Discovery Panel, which helps users discover topics by browsing popular tags.                                                        |
| EnableBrokenLinkChecker                       | boolean        | The default value is `true`. Checks all external links of published blog posts once a day and lists broken ones on the "Broken links" admin page.                               |
| ShowCodeBlockLanguage                         | boolean        | The default value is `true`. Shows the language of a fenced code block (e.g. `csharp`) in the header of the code block.                                                        |
| EnableMarkdownForLlms                         | boolean        | The default value is `false`. Serves published blog posts as markdown to clients (like LLMs) that ask for `text/markdown` or append `.md` to the url, and serves `/llms.txt`. See [Markdown for LLMs](#markdown-for-llms).        |

### Markdown for LLMs

If `EnableMarkdownForLlms` is set to `true`, a blog post page (`/blogPost/{id}` or `/blogPost/{id}/{slug}`) can be requested as markdown instead of HTML. LLMs and agents then don't have to parse the HTML page. Only blog post pages support this; every other page always returns HTML.

```bash
curl -H "Accept: text/markdown" https://example.com/blogPost/38a0d0f4-e9ff-4947-a05c-527af68b6cd1
```

The response (`Content-Type: text/markdown; charset=utf-8`) contains YAML front matter, the title and the raw markdown content with all short codes expanded:

```markdown
---
title: "My blog post"
description: "The short description"
updated: 2026-09-01
tags: ["dotnet", "csharp"]
canonical: "https://example.com/blogPost/38a0d0f4-e9ff-4947-a05c-527af68b6cd1/my-blog-post"
---

# My blog post

The content...
```

- `author` is added when `UseMultiAuthorMode` is enabled and the blog post has an author; `tags` is omitted when the blog post has none.
- Markdown is only served if `text/markdown` is weighted at least as high as HTML in the `Accept` header (`text/html`, `text/*` or `*/*`), so `text/markdown, text/html;q=0.9` returns markdown, while `text/html, text/markdown;q=0.9` and a regular browser request return HTML.
- Only published blog posts are served. Unpublished, scheduled and unknown blog posts return `404` with a markdown body.
- Blog post responses carry `Vary: Accept` so caches and CDNs keep the HTML and markdown variants apart.
- Appending `.md` to a blog post url (`/blogPost/{id}.md` or `/blogPost/{id}/{slug}.md`) returns markdown regardless of the `Accept` header, for agents that can't set headers.
- HTML blog post pages advertise the markdown version via a `Link: </blogPost/{id}/{slug}.md>; rel="alternate"; type="text/markdown"` header and a `<link rel="alternate" type="text/markdown">` tag.
- `/llms.txt` (see [llmstxt.org](https://llmstxt.org)) lists the blog name, introduction and all published blog posts with links to their markdown version.
- If the setting is `false` or missing, the `Accept` header is ignored and HTML is always returned, `.md` urls render the HTML page, no `Link` header or `<link>` tag is emitted and `/llms.txt` returns `404`.
