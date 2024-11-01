# Migration Guide
This document describes the changes that need to be made to migrate from one version of the blog to another.

## 8.0 to 9.0

### Shortcodes
Shortcodes, a form a templating that can be adjusted dynamically, are introduced in this version. The following table has to be added to the database:

```sql
CREATE TABLE Shortcodes
(
	Id [NVARCHAR](450) NOT NULL,
	Name [NVARCHAR](512) NOT NULL,
	MarkdownContent NVARCHAR(MAX) NOT NULL,
)

ALTER TABLE Shortcodes
ADD CONSTRAINT PK_Shortcodes PRIMARY KEY (Id)
```

### Similiar blog posts

A new `SimilarBlogPost` table is introduced to store similar blog posts.

```sql
CREATE TABLE SimilarBlogPosts
(
	Id [NVARCHAR](450) NOT NULL,
	SimilarBlogPostIds NVARCHAR(1350) NOT NULL,
)

ALTER TABLE SimilarBlogPosts
ADD CONSTRAINT PK_SimilarBlogPosts PRIMARY KEY (Id)
```

Add the following to the `appsettings.json`:

```json
{
	"SimilarBlogPosts": true
}
```

Or `false` if you don't want to use this feature.

### Donation configuration
Existing donation settings are now moved to a "SupportMe" node. Copy your previously set values for `KofiToken`, `GithubSponsorName`, and `PatreonName` and follow the new format below. Use `true` or `false` to choose where you want the donation buttons to appear and also a support me page can optionally be added to the nav menu.

```json
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
```

