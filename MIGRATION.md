# Migration Guide
This document describes the changes that need to be made to migrate from one version of the blog to another.

## Automated Upgrade Assistant

Starting with version 12.0, we provide an **Upgrade Assistant** tool that automates most configuration migrations. This tool:
- Automatically detects your current configuration version
- Applies necessary transformations to `appsettings.json` files
- Creates backups before making changes
- Provides colorful console output with clear warnings and instructions

**Usage:**
```bash
# From your blog directory
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant

# Preview changes without applying
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --dry-run

# See all options
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --help
```

For detailed documentation, see [docs/Migrations/UpgradeAssistant.md](docs/Migrations/UpgradeAssistant.md).

**Note:** While the Upgrade Assistant handles most configuration changes automatically, some migrations still require manual steps (especially database schema changes). These are noted below.

---

## 16.0 to 17.0

### Blog post versioning on MongoDB and RavenDB
Versioning now works on every storage provider (before, the editor failed to open on MongoDB and RavenDB). No database changes are needed.

- Saving a post stores the snapshot and the updated post as one unit of work. On a standalone MongoDB server (no replica set) these are two ordered writes instead of a transaction; the worst case after a crash is one extra snapshot of the unchanged post.
- New versions get the id `{blogPostId}-v{versionNumber}`. Existing versions keep their ids.
- Deleting a blog post now also deletes its versions.
- Likes are updated atomically and no longer overwrite concurrent edits.

### Blog post lists on RavenDB and MongoDB
The search, archive, bookmarks, RSS feed, broken link checker and visit counter now also work on RavenDB (and the archive on MongoDB). Lists no longer load the content of every blog post. The search now ignores casing of the title on every storage provider (before it depended on the database).

### Similar blog posts
Similar blog posts are now stored under the id `{blogPostId}-similar`, so they no longer collide with blog post ids on RavenDB (similar posts never showed up there before). Existing entries keep working and are replaced the next time the similar blog post job runs (after creating, publishing or deleting a blog post).

## 15.0 to 16.0

### Blog post record index

The index on `BlogPostRecords` was replaced. Both queries that read this table (the dashboard visit counter and `TransformBlogPostRecordsJob`) filter on `DateClicked` only, but the old index led with `BlogPostId` and could therefore never be seeked. The replacement leads with `DateClicked` and carries `BlogPostId` and `Clicks`, which makes the dashboard's `GROUP BY BlogPostId, SUM(Clicks)` index-only.

For SQL providers, run the `ChangeBlogPostRecordIndex` Entity Framework migration, or execute `scripts/2026-09-13-BlogPostRecordIndex.sql` (SQL Server, includes an optional `__EFMigrationsHistory` baseline). The portable equivalent is:

```sql
DROP INDEX IX_BlogPostRecords_BlogPostId_DateClicked ON BlogPostRecords;
CREATE INDEX IX_BlogPostRecords_DateClicked_BlogPostId_Clicks
    ON BlogPostRecords (DateClicked, BlogPostId, Clicks);
```

> **The `DROP` may fail because the old index does not exist.** That is expected on most installations, and safe to ignore. `BlogDbContext` bootstraps its schema with `Database.EnsureCreated()`, which only acts on an empty database and never writes `__EFMigrationsHistory`. On a database that already had tables when it was first started, no Entity Framework migration has ever been applied — including `AddBlogPostRecordIndex`, which introduced the old index. Check what you actually have with `EXEC sp_helpindex 'BlogPostRecords'` before running anything.

### Visit counts are no longer double-counted

`ShowBlogPostPage` recorded a visit on every render rather than only the first, so each page view was counted at least twice. This is now fixed. Expect the numbers on the dashboard to drop by roughly half from the upgrade date onward — that is the correction, not a drop in traffic. Historical data is left untouched and is not comparable with data recorded after the upgrade.

### Code block language
Code blocks now have a header that contains the copy button and, optionally, the language of the code block. The `ShowCodeBlockLanguage` setting was added on the root level of the `appsettings.json` file (handled by the Upgrade Assistant). The default is `true`, set it to `false` to hide the language.

```json
{
  ...
  "ShowCodeBlockLanguage": true
}
```

## 14.0 to 15.0

### Broken link checker
A new `BrokenLinks` table is introduced to store the results of the broken link checker. For SQL providers, run the `AddBrokenLinks` Entity Framework migration or execute the following script:

```sql
CREATE TABLE BrokenLinks
(
	Id [VARCHAR](900) NOT NULL,
	BlogPostId [NVARCHAR](256) NOT NULL,
	BlogPostTitle [NVARCHAR](256) NOT NULL,
	Url NVARCHAR(MAX) NOT NULL,
	Reason [NVARCHAR](1024) NOT NULL,
	CheckedDate DATETIME2 NOT NULL,
)

ALTER TABLE BrokenLinks
ADD CONSTRAINT PK_BrokenLinks PRIMARY KEY (Id)
```

The `EnableBrokenLinkChecker` setting was added on the root level of the `appsettings.json` file (handled by the Upgrade Assistant). The default is `true`, set it to `false` to turn the checker off.

```json
{
  ...
  "EnableBrokenLinkChecker": true
}
```

## 13.0 to 14.0

### Blog post versioning
A new `BlogPostVersions` table stores the version history of blog posts. `Database.EnsureCreated()` never adds tables to an existing database, so for SQL providers run the `AddBlogPostVersioning` Entity Framework migration or execute the following script (SQL Server):

```sql
CREATE TABLE [BlogPostVersions] (
    [Id] varchar(900) NOT NULL,
    [BlogPostId] varchar(256) NOT NULL,
    [VersionNumber] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [Title] nvarchar(256) NOT NULL,
    [ShortDescription] nvarchar(max) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [PreviewImageUrl] nvarchar(1024) NOT NULL,
    [PreviewImageUrlFallback] nvarchar(1024) NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [Tags] nvarchar(2048) NOT NULL,
    [IsPublished] bit NOT NULL,
    [ReadingTimeInMinutes] int NOT NULL,
    [AuthorName] nvarchar(256) NULL,
    CONSTRAINT [PK_BlogPostVersions] PRIMARY KEY ([Id])
)
GO
CREATE UNIQUE INDEX [IX_BlogPostVersions_BlogPostId_VersionNumber] ON [BlogPostVersions] ([BlogPostId], [VersionNumber])
GO
```

<details>
<summary>SQLite</summary>

```sql
CREATE TABLE "BlogPostVersions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BlogPostVersions" PRIMARY KEY,
    "BlogPostId" TEXT NOT NULL,
    "VersionNumber" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "Title" TEXT NOT NULL,
    "ShortDescription" TEXT NOT NULL,
    "Content" TEXT NOT NULL,
    "PreviewImageUrl" TEXT NOT NULL,
    "PreviewImageUrlFallback" TEXT NULL,
    "UpdatedDate" TEXT NOT NULL,
    "Tags" TEXT NOT NULL,
    "IsPublished" INTEGER NOT NULL,
    "ReadingTimeInMinutes" INTEGER NOT NULL,
    "AuthorName" TEXT NULL
);
CREATE UNIQUE INDEX "IX_BlogPostVersions_BlogPostId_VersionNumber" ON "BlogPostVersions" ("BlogPostId", "VersionNumber");
```
</details>

<details>
<summary>MySQL</summary>

```sql
CREATE TABLE `BlogPostVersions` (
    `Id` varchar(255) CHARACTER SET utf8mb4 NOT NULL,
    `BlogPostId` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    `VersionNumber` int NOT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `Title` varchar(256) CHARACTER SET utf8mb4 NOT NULL,
    `ShortDescription` longtext CHARACTER SET utf8mb4 NOT NULL,
    `Content` longtext CHARACTER SET utf8mb4 NOT NULL,
    `PreviewImageUrl` varchar(1024) CHARACTER SET utf8mb4 NOT NULL,
    `PreviewImageUrlFallback` varchar(1024) CHARACTER SET utf8mb4 NULL,
    `UpdatedDate` datetime(6) NOT NULL,
    `Tags` varchar(2048) CHARACTER SET utf8mb4 NOT NULL,
    `IsPublished` tinyint(1) NOT NULL,
    `ReadingTimeInMinutes` int NOT NULL,
    `AuthorName` varchar(256) CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_BlogPostVersions` PRIMARY KEY (`Id`)
) CHARACTER SET=utf8mb4;
CREATE UNIQUE INDEX `IX_BlogPostVersions_BlogPostId_VersionNumber` ON `BlogPostVersions` (`BlogPostId`, `VersionNumber`);
```
</details>

<details>
<summary>PostgreSQL</summary>

```sql
CREATE TABLE "BlogPostVersions" (
    "Id" text NOT NULL,
    "BlogPostId" character varying(256) NOT NULL,
    "VersionNumber" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "Title" character varying(256) NOT NULL,
    "ShortDescription" text NOT NULL,
    "Content" text NOT NULL,
    "PreviewImageUrl" character varying(1024) NOT NULL,
    "PreviewImageUrlFallback" character varying(1024),
    "UpdatedDate" timestamp with time zone NOT NULL,
    "Tags" text[] NOT NULL,
    "IsPublished" boolean NOT NULL,
    "ReadingTimeInMinutes" integer NOT NULL,
    "AuthorName" character varying(256),
    CONSTRAINT "PK_BlogPostVersions" PRIMARY KEY ("Id")
);
CREATE UNIQUE INDEX "IX_BlogPostVersions_BlogPostId_VersionNumber" ON "BlogPostVersions" ("BlogPostId", "VersionNumber");
```
</details>

If you created the table by hand and want to use Entity Framework migrations later, mark the migration as applied:

```sql
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260424073229_AddBlogPostVersioning', N'9.0.14');
```

## 12.0 to 13.0
`EnableTagDiscoveryPanel` setting was added on the root level of the `appsettings.json` file. This setting controls whether the Tag Discovery panel is shown in the navigation bar.

```json
{
  ...
  "EnableTagDiscoveryPanel": true
}
```

## 11.0 to 12.0
`ShowBuildInformation` setting was added on the root level of the `appsettings.json` file. This setting controls whether build information (like build date) is shown in the `Footer` component.

```json
{
  ...
  "ShowBuildInformation": true
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
