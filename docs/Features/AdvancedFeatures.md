## Advanced Features

- [Advanced Features](#advanced-features)
- [Shortcodes](#shortcodes)
  - [Creating a shortcode](#creating-a-shortcode)
  - [Using a shortcode](#using-a-shortcode)
  - [Limitations](#limitations)
- [Critical CSS Generator](#critical-css-generator)
  - [How it works](#how-it-works)
  - [Options](#options)
- [Output Modes](#output-modes)
  - [Console Mode](#console-mode)
    - [File Mode](#file-mode)
    - [Layout Mode](#layout-mode)
  - [Examples](#examples)
  - [Notes](#notes)

This page lists some of the more advanced or less-used features of the blog software.

## Shortcodes
Shortcodes are markdown content that can be shown inside blog posts (like templates that can be referenced).
The idea is to reuse certain shortcodes across various blog posts.
If you update the shortcode, it will be updated across all those blog posts as well.

For example if you have a running promotion you can add a shortcode and link it in various blog posts. Updating the shortcode (for example that it is almost sold out) will update all blog posts that reference this shortcode.

### Creating a shortcode

To create a shortcode, click on "Shortcodes" in the Admin tab of the navigation bar. You can create a shortcode by adding a name in the top row and the markdown content in the editor. Clicking on an already existing shortcode will allow you to either edit the shortcode or delete it.

Currently, deleting a shortcode will leave the shortcode name inside the blogpost. Therefore only delete shortcodes if you are sure that they are not used anymore.

### Using a shortcode
There are two ways:
 1. If you know the shortcode name, just type in `[[SHORTCODENAME]]` where `SHORTCODENAME` is the name you gave the shortcode.
 2. Click on the more button in the editor and select "Shortcodes". This will open a dialog where you can select the shortcode you want to insert and puts it into the clipboard.

### Limitations
Shortcodes
 * are not recursive. This means that you cannot use a shortcode inside a shortcode.
 * are not part of the table of contents even though they might have headers.
 * are not part of the reading time calculation.
 * are only available in the content section of a blog post and not the description.
 * are currently only copied to the clipboard and not inserted directly into the editor at the cursor position.

## Critical CSS Generator

The Critical CSS Generator is a tool that extracts the minimal CSS required for rendering the above-the-fold content of the blog. This optimization improves the initial page load performance by reducing render-blocking CSS.

### How it works

The generator:

1. Starts a test instance of the blog
2. Visits the homepage and a sample blog post
3. Extracts the critical CSS using Playwright
4. Outputs the CSS based on the chosen output

The generator is under `tools/LinkDotNet.Blog.CriticalCSS`. You can run it from the command line or directly via `dotnet run`. Here an example

```bash
dotnet run -- --install-playwright -o file -p "critical.css"
```

The output of the "critical.css" should be copied into the head of the [`_Layout.cshtml`](../../src/LinkDotNet.Blog.Web/Pages/_Layout.cshtml) file.

### Options

| Option | Long Form | Description | Required | Example |
|--------|-----------|-------------|----------|---------|
| `-i` | `--install-playwright` | Installs required Playwright dependencies | No | `--install-playwright` |
| `-o` | `--output` | Output mode: `console`, `file`, or `layout` | Yes | `--output console` |
| `-p` | `--path` | File path for `file` or `layout` output modes | Yes* | `--path styles.css` |
| `-h` | `--help` | Shows help information | No | `--help` |

*Required when using `file` or `layout` output modes

## Output Modes

### Console Mode
Outputs the critical CSS directly to the console:

```sh
dotnet run -- --output console
```

#### File Mode
Saves the critical CSS to a new file:

```sh
dotnet run -- --output file --path critical.css
```

#### Layout Mode
Injects or updates the critical CSS in your layout file:

```sh
dotnet run -- --output layout --path ./Pages/Shared/_Layout.cshtml
```

### Examples

1. Install Playwright and output to console:
```sh
dotnet run -- --install-playwright --output console
```

2. Save critical CSS to a file:
```sh
dotnet run -- --output file --path styles.css
```

3. Update layout file with critical CSS:
```sh
dotnet run -- --output layout --path _Layout.cshtml
```

4. Show help information:
```sh
dotnet run -- --help
```

### Notes

- The tool requires an internet connection for Playwright installation
- The generated CSS is minified for optimal performance
- When using layout mode, existing `<style>` tags will be replaced
- If no `<style>` tag exists in layout mode, it will be inserted before `</head>`