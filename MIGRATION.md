# Migration Guide
This document describes the changes that need to be made to migrate from one version of the blog to another.

## 8.0 to 9.0

### Shortcodes
Shortcodes, a form a templating that can be adjusted dynamically, are introduced in this version. The following table has to be added to the database:

```sql
CREATE TABLE Shortcodes
(
	Id [NVARCHAR](450)] NOT NULL,
	Name [NVARCHAR(512)] NOT NULL,
	MarkdownContent NVARCHAR(MAX) NOT NULL,
)
```

### Similiar blog posts

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
