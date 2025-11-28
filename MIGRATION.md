# Migration Guide
This document describes the changes that need to be made to migrate from one version of the blog to another.

## 11.0 to 12.0

A new config has been added `ShowReadPostIndicator` in `appsettings.json`. The default value is `false`. If set to `true`, a subtle indicator will show which blog posts the user has already read. The read state is stored in the browser's localStorage.

```json
{
  "ShowReadPostIndicator": true
}
```

## 9.0 to 11.0

A new config has been added `UseMultiAuthorMode` in `appsettings.json`. The default value of this config is `false`. If set to `true` then author name will be associated with blog posts at the time of creation.

## 8.0 to 9.0

### SQL - Entity Framework Migrations

Starting with `v9.0` the blog uses Entity Framework Migrations for all SQL providers. If you are already having a database you need to run the following script that creates the history table and the initial entry:
```bash
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241128180004_Initial', N'8.0.11');
GO
```

Read more in the [documentation](docs/Storage/Readme.md).

### Support / Donation section
If you used the sponsor/donation mechanism in the `appsettings.json` like this:
```json
{
  ...
  "KofiToken": "TokenHere",
  "GithubSponsorName": "namehere",
  "PatreonName": "namehere",
}
```

These moved to their own respective subsection:
```json
{
	"SupportMe": {
	  "KofiToken": "TokenHere",
      "GithubSponsorName": "namehere",
      "PatreonName": "namehere",
	  "ShowUnderBlogPost": true,
	}
}
```

The `ShowUnderBlogPost` is needed to indicate that the part will be shown under each blog post. We also added more possibilities to customize this:

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

Use `true` or `false` to choose where you want the donation buttons to appear and also a support me page can optionally be added to the nav menu. Checkout the [Donation section in the documentation](docs/Donations/Readme.md).

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
