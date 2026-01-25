# Markdown Import

The Markdown Import feature allows you to automatically import blog posts from external sources (such as GitHub repositories) by periodically scanning for markdown files. This enables you to author and version control your blog posts externally while having them automatically synchronized to your blog.

## Overview

The Markdown Import job runs every 15 minutes (when enabled) and:
1. Fetches markdown files from the configured source URL
2. Parses metadata from each file's header section
3. Creates new blog posts or updates existing ones based on the `ExternalId`
4. Clears the cache to reflect changes

## Configuration

Add the following section to your `appsettings.json` file:

```json
{
  "MarkdownImport": {
    "Enabled": true,
    "SourceType": "FlatDirectory",
    "Url": "https://raw.githubusercontent.com/yourusername/blog-posts/main/posts/"
  }
}
```

### Configuration Properties

| Property | Type | Description | Default |
|----------|------|-------------|---------|
| `Enabled` | boolean | Enable or disable the markdown import feature | `false` |
| `SourceType` | string | Type of source provider (currently only `FlatDirectory` is supported) | `"FlatDirectory"` |
| `Url` | string | Base URL where markdown files are located | `""` |

## Markdown File Format

Each markdown file must follow this three-section format, with sections separated by `----------`:

```markdown
----------
id: unique-blog-post-id
title: Your Blog Post Title
tags: tag1, tag2, tag3
image: https://example.com/preview-image.webp
fallbackimage: https://example.com/fallback-image.jpg
published: true
updatedDate: 2026-01-25T20:30:00Z
authorName: John Doe
----------
This is the **short description** of your blog post.
It can contain *markdown* formatting and will be displayed in blog post previews.
----------
This is the main content of your blog post.

## You can use headings

- Bullet points
- Code blocks
- Images
- All markdown features supported by the blog
```

### Metadata Fields

#### Required Fields

- **id**: Unique identifier for the blog post (used to track and update posts). Must be unique across all markdown files. Example: `my-first-post`
- **title**: The title of the blog post
- **image**: URL to the preview image (used in blog post cards and social media)
- **published**: Boolean value (`true` or `false`) indicating whether the post should be published

#### Optional Fields

- **tags**: Comma-separated list of tags
- **fallbackimage**: URL to a fallback image (used if the primary image fails to load)
- **updatedDate**: ISO 8601 formatted date (e.g., `2026-01-25T20:30:00Z`). If not provided, current time is used
- **authorName**: Name of the author. Useful when `UseMultiAuthorMode` is enabled

### Content Sections

After the metadata header, the file must contain two content sections:

1. **Short Description** (first section after header): A brief summary shown in blog post listings
2. **Main Content** (second section after header): The full blog post content

Both sections support full markdown syntax.

## How It Works

### Import Process

1. The job fetches all `.md` files from the configured URL
2. Files are processed in alphabetical order
3. For each file:
   - The markdown is parsed into metadata, short description, and content
   - The system checks if a blog post with the same `ExternalId` exists
   - If it exists, the post is updated with new content
   - If it doesn't exist, a new blog post is created
4. After successful imports, the cache is cleared

### Manual Import Trigger

In addition to the automatic 15-minute schedule, you can manually trigger an import from the **Settings** page in the admin area:

1. Log in to your blog
2. Navigate to **Settings** (when logged in)
3. Click the **"Run Import"** button in the Markdown Import row
4. The import job will start immediately

This is useful when:
- You've just pushed new markdown files and want them imported right away
- You're testing the import configuration
- You need to re-import files after making corrections

### Update Behavior

When a markdown file is re-imported (same `id` as an existing post):
- All content is updated from the markdown file
- The `ExternalId` remains unchanged
- **⚠️ Manual edits made through the blog UI will be overwritten**

**Critical Warning**: If you edit an imported blog post through the blog's UI (using the built-in editor), those changes will be **permanently lost** the next time the import job runs (either automatically every 15 minutes or when manually triggered). 

**Best Practice**: Always treat your external markdown repository as the **single source of truth** for imported posts. Make all edits to imported posts in your external repository, not in the blog UI.

If you need to stop auto-importing a specific post while retaining your manual edits:
1. Remove the markdown file from the external source, OR
2. Change the `id` field in the markdown file (this will create a new post on next import)
3. The original imported post (with your manual edits) will remain unchanged in the blog

### Error Handling

The import job is designed to be resilient:
- If a file fails to parse, an error is logged and the job continues with other files
- If the source URL is unreachable, the error is logged and the job completes without changes
- Invalid field values are logged as warnings but won't crash the job

## Example Workflows

### GitHub Repository Setup

1. Create a repository for your blog posts (e.g., `blog-posts`)
2. Create a `posts/` directory
3. Add markdown files following the format above
4. Configure your blog's `appsettings.json` to point to the raw GitHub URL:

```json
{
  "MarkdownImport": {
    "Enabled": true,
    "SourceType": "FlatDirectory",
    "Url": "https://raw.githubusercontent.com/yourusername/blog-posts/main/posts/"
  }
}
```

### Example Markdown File

File: `2026-01-my-first-imported-post.md`

```markdown
----------
id: 2026-01-my-first-imported-post
title: Getting Started with Markdown Import
tags: tutorial, markdown, automation
image: https://images.unsplash.com/photo-1499750310107-5fef28a66643
fallbackimage: https://via.placeholder.com/800x400
published: true
updatedDate: 2026-01-25T10:00:00Z
authorName: Jane Developer
----------
Learn how to use the markdown import feature to manage your blog posts in a Git repository.
This short description appears in blog listings.
----------
# Introduction

This is the full blog post content. You can use any markdown syntax here.

## Why Use Markdown Import?

- Version control your blog posts with Git
- Write in your favorite editor
- Collaborate with others using pull requests
- Automate your blogging workflow

## Code Example

```csharp
public class BlogPost
{
    public string Title { get; set; }
    public string Content { get; set; }
}
```

That's all there is to it!

## Troubleshooting

### Posts Not Importing

1. Check that `Enabled` is set to `true` in configuration
2. Verify the `Url` is accessible and returns a directory listing with `.md` files
3. Check application logs for error messages
4. Ensure markdown files follow the correct format

### Parsing Errors

Common issues:
- Missing required fields (`id`, `title`, `image`, `published`)
- Malformed header section (missing `----------` delimiters)
- Invalid date format in `updatedDate` field
- Empty content sections

Check the application logs for specific error messages indicating which file and what field caused the issue.

### Updates Not Reflecting

- The job runs every 15 minutes, so changes may take time to appear
- Check that the `id` field in your markdown matches the `ExternalId` of the existing post
- Clear the blog cache manually if needed

## Limitations

- **Flat Directory Only**: Currently only supports flat directory structures (all files in one directory)
- **Public URLs**: The URL must be publicly accessible (no authentication support yet)
- **No Conflict Resolution**: External source is always the source of truth; manual edits are overwritten
