### Configurations

### appsettings.json

The appsettings.json file has a lot of options to customize the content of the blog. The following table shows which values are used when.

```json
{
	"BlogName": "linkdotnet",
	"BlogBrandUrl": "http//some.url/image.png",
	"GithubAccountUrl": "",
	"Social": {
		"GithubAccountUrl": "",
		"LinkedInAccountUrl": "",
		"TwitterAccountUrl": ""
	},
	"Introduction": {
		"Description": "Some nice text about yourself. Markup can be used [Github](https://github.com/someuser/somerepo)",
		"BackgroundUrl": "assets/profile-background.webp",
		"ProfilePictureUrl": "assets/profile-picture.webp"
	},
	"PersistenceProvider": "InMemory",
	"ConnectionString": "",
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
	"KofiToken": "ABC123",
	"GithubSponsorName": "your-tag-here",
	"ShowReadingIndicator": true,
	"PatreonName": "your-tag-here",
	"SimlarBlogPosts": "true"
}
```

| Property                                      | Type           | Description                                                                                                                                                                       |
| --------------------------------------------- | -------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| BlogName                                      | string         | Name of your blog. Is used in the navbar and is used as the title of the page. Will not be shown when `BlogBrandUrl` is set                                                       |
| BlogBrandUrl                                  | string         | The url to an image which is used as a brand image in the navigation bar. If not set or `null` the `BlogName` will be shown                                                       |
| Social                                        | node           | Represents all possible linked social accounts                                                                                                                                    |
| GithubAccountUrl                              | string         | Url to your github account. If not set it is not shown in the introduction card                                                                                                   |
| LinkedInAccountUrl                            | string         | Url to your LinkedIn account. If not set it is not shown in the introduction card                                                                                                 |
| TwitterAccountUrl                             | string         | Url to your Twitter account. If not set it is not shown in the introduction card                                                                                                  |
| Introduction                                  |                | Is used for the introduction part of the blog                                                                                                                                     |
| Description                                   | MarkdownString | Small introduction text for yourself. This is also used for `<meta name="description">` tag. For this the markup will be converted to plain text                                  |
| BackgroundUrl                                 | string         | Url or path to the background image. (Optional)                                                                                                                                   |
| ProfilePictureUrl                             | string         | Url or path to your profile picture                                                                                                                                               |
| PersistenceProvider                           | string         | Declares the type of the storage provider (one of the following: `SqlServer`, `Sqlite`, `RavenDb`, `InMemory`, `MySql`). More in-depth explanation [here](./../Storage/Readme.md) |
| ConnectionString                              | string         | Is used for connection to a database. Not used when `InMemoryStorageProvider` is used                                                                                             |
| DatabaseName                                  | string         | Name of the database. Only used with `RavenDbStorageProvider`                                                                                                                     |
| [AuthProvider](./../Authorization/Readme.md)  | string         |                                                                                                                                                                                   |
| [PROVIDER_NAME](./../Authorization/Readme.md) | string         |                                                                                                                                                                                   |
| Domain                                        | string         |                                                                                                                                                                                   |
| ClientId                                      | string         |                                                                                                                                                                                   |
| ClientSecret                                  | string         |                                                                                                                                                                                   |
| LogoutUri                                     | string         |                                                                                                                                                                                   |
| BlogPostsPerPage                              | int            | Gives the amount of blog posts loaded and display per page. For more the user has to use the navigation                                                                           |
| FirstPageCacheDurationInMinutes               | int            | The duration in minutes the first page is cached.                                                                                                                                 |
| AboutMeProfileInformation                     | node           | Sets information for the About Me Page. If omitted the page is disabled completely                                                                                                |
| Name                                          | string         | Name, which is displayed on top of the profile card                                                                                                                               |
| Heading                                       | string         | Displayed under the name. For example job title                                                                                                                                   |
| ProfilePictureUrl                             | string         | Displayed profile picture                                                                                                                                                         |
| [Giscus](./../Comments/Giscus.md)             | node           | Enables the comment section via giscus. If left empty the comment secion will not be shown.                                                                                       |
| [Disqus](./../Comments/Disqus.md)             | node           | Enables the comment section via disqus. If left empty the comment secion will not be shown.                                                                                       |
| KofiToken                                     | string         | Enables the "Buy me a Coffee" button of Kofi. To aquire the token head down to the "Kofi" section                                                                                 |
| GithubSponsorName                             | string         | Enables the "Github Sponsor" button which redirects to GitHub. Only pass in the user name instead of the url.                                                                     |
| ShowReadingIndicator                          | boolean        | If set to `true` (default) a circle indicates the progress when a user reads a blog post (without comments).                                                                      |
| PatreonName                                   | string         | Enables the "Become a patreon" button that redirects to patreon.com. Only pass the user name (public profile) as user name.                                                       |
| SimilarBlogPosts                              | boolean        | If set to `true` (default) similar blog posts are shown at the end of a blog post.                                                                                                |