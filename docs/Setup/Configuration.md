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
	"SupportMe": {
		"KofiToken": "ABC123",
		"GithubSponsorName": "your-tag-here",
		"PatreonName": "your-tag-here",
		"ShowUnderBlogPost": true,
		"ShowUnderIntroduction": true,
		"ShowInFooter": true,
		"ShowSupportMePage": true,
		"SupportMePageDescription": "Buy my book here: [My Blazor Book](https://google.com) or please contribute to my open-source project here: [My Awesome Repo](https://github.com) . This can be **markdown**."
	}
}
```

| Property                                      | Type           | Description                                                                                                                                                                      |
| --------------------------------------------- | -------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| BlogName                                      | string         | Name of your blog. Is used in the navbar and is used as the title of the page. Will not be shown when `BlogBrandUrl` is set                                                      |
| BlogBrandUrl                                  | string         | The url to an image which is used as a brand image in the navigation bar. If not set or `null` the `BlogName` will be shown                                                      |
| Social                                        | node           | Represents all possible linked social accounts                                                                                                                                   |
| GithubAccountUrl                              | string         | Url to your github account. If not set it is not shown in the introduction card                                                                                                  |
| LinkedInAccountUrl                            | string         | Url to your LinkedIn account. If not set it is not shown in the introduction card                                                                                                |
| TwitterAccountUrl                             | string         | Url to your Twitter account. If not set it is not shown in the introduction card                                                                                                 |
| YoutubeAccountUrl                             | string         | Url to your Youtube account. If not set it is not shown in the introduction card                                                                                                 |
| Introduction                                  |                | Is used for the introduction part of the blog                                                                                                                                    |
| Description                                   | MarkdownString | Small introduction text for yourself. This is also used for `<meta name="description">` tag. For this the markup will be converted to plain text                                 |
| BackgroundUrl                                 | string         | Url or path to the background image. (Optional)                                                                                                                                  |
| ProfilePictureUrl                             | string         | Url or path to your profile picture                                                                                                                                              |
| [PersistenceProvider](./../Storage/Readme.md) | string         | Declares the type of the storage provider (one of the following: `SqlServer`, `Sqlite`, `RavenDb`, `MongoDB`, `MySql`). More in-depth explanation [here](./../Storage/Readme.md) |
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
| [SupportMe](./../Donations/Readme.md)         | node           | Donation sections configuration. If left empty no donation sections will not be shown.                                                                                           |
