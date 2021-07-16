# LinkDotNet.Blog
This is a blog software completely written in C# / Blazor. The aim is to have it configurable as possible. 

## How does it work
The basic idea is that the content creator writes his posts in markdown language (like this readme file). 
The markdown will then by translated to HTML and displayed to the client. This gives an easy entry to writing posts with all the flexibility markdown has.
This also includes source code snippets. Right now only C# is highlighted properly but other languages can be extended easily as the highlighting is done via highlight.js.

## In Action
[!overview](overview.gif)

## Setup
Just clone this repository and you are good to go. There are some settings you can tweak. The following chapter will guide you 
through the possibilities.

### appsettings.json
The appsettings.json file has a lot of options to customize the content of the blog. The following table shows which values are used when.

```json
{
  ...
  "BlogName": "linkdotnet",
  "GithubAccountUrl": "",
  "LinkedInAccountUrl": "",
  "Introduction": {
    "Description": "Some nice text about yourself. Markup can be used [Github](https://github.com/someuser/somerepo)",
    "BackgroundUrl": "assets/profile-background.webp",
    "ProfilePictureUrl": "assets/profile-picture.webp"
  },
  "ConnectionString": "",
  "DatabaseName": "",
  "Auth0": {
    "Domain": "",
    "ClientId": "",
    "ClientSecret": ""
  },
  "BlogPostsPerPage": 10,
}

```

| Property           | Type           | Description                                                                                                                                       |
| ------------------ | -------------- | ------------------------------------------------------------------------------------------------------------------------------------------------- |
| BlogName           | string         | Name of your blog. Is used in the navbar and is used as the title of the page.                                                                    |
| GithubAccountUrl   | string         | Url to your github account. If not set the navigation link is not shown                                                                           |
| LinkedInAccountUrl | string         | Url to your LinkedIn account. If not set the navigation link is not shown                                                                         |
| Introduction       |                | Is used for the introduction part of the blog                                                                                                     |
| Description        | MarkdownString | Small introduction text for yourself. This is also used for `<meta name="description">` tag. For this the markup will be converted to plain text. |
| BackgroundUrl      | string         | Url or path to the background image                                                                                                               |
| ProfilePictureUrl  | string         | Url or path to your profile picture                                                                                                               |
| ConnectionString   | string         | Is used for connection to a database. Not used when `InMemoryStorageProvider` is used                                                             |
| DatabaseName       | string         | Name of the database. Only used with `RavenDbStorageProvider`                                                                                     |
| Auth0              |                | Configuration for setting up Auth0                                                                                                                |
| Domain             | string         | See more details here: https://manage.auth0.com/dashboard/                                                                                        |
| ClientId           | string         | See more details here: https://manage.auth0.com/dashboard/                                                                                        |
| ClientSecret       | string         | See more details here: https://manage.auth0.com/dashboard/                                                                                        |
| BlogPostsPerPage   | int            | Gives the amount of blog posts loaded and display per page. For more the user has to use the navigation                                           |

The usage might shift directly into the extension methods, where they are used.

## Storage Provider
Currently there are 4 Storage-Provider:
 * InMemory - Basically a list holding your data (per request)
 * RavenDb - As the name suggests for RavenDb
 * Sqlite - Based on EF Core, so it can be easily adapted for other Sql Dialects
 * SqlServer - Based on EF Core, so it can be easily adapted for other Sql Dialects

### Using
To use one of those just use the extension method in the Startup.cs in `ConfigureServices`:
```csharp
services.UseSqlAsStorageProvider();
```

It is only one storage provider at a time allowed. Registering multiple will result in an exception.

## Authorization
There is only one mechanism enabled via Auth0. For more information go to: https://auth0.com/docs/applications

The main advantage of Auth0 is the easy configurable dashboard on their website. 

## Search Engine Optimization (SEO)
The blog includes some of the most important tags to get indexed by a crawler. Furthermore some aspects of the Open Graph specification are implemented.

### Robots.txt
In the wwwroot/ you can find a default robots.txt. It allows that the site gets completely indexed. If you want to tweak that behavior feel free.
Also you can provide a sitemap.xml to get a better ranking. 

### Open Graph Tags
To get better results when for example shared via LinkedIn some of the `<meta property="og:tag">` tags are implemented.

The following tags are set depending on the page:

| Open Graph Tag | Index                                                     | Display Blog Post                            |
| -------------- | --------------------------------------------------------- | -------------------------------------------- |
| og:title       | Title of the blog (defined in Introduction)               | Title of the Blog Post                       |
| og:url         | Url to the index page                                     | Url of the page                              |
| og:image       | Background image (defined in Introduction)                | Yes                                          |
| og:type        | article                                                   | article                                      |
| og:description | Short description in plain text (defined in Introduction) | Short Description of Blog Post in plain text |

Furthermore the following tags are set:

| Tag                                      | Index                                | Display Blog Post             |
| ---------------------------------------- | ------------------------------------ | ----------------------------- |
| Title of the web page                    | Defined in AppConfiguration.BlogName | Title of the blogpost         |
| &lt;meta name="keyword" content="" /&gt; | not set                              | Tags defined in the Blog Post |