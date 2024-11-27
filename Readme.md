﻿# LinkDotNet.Blog

[![.NET](https://github.com/linkdotnet/Blog/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/linkdotnet/Blog/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/linkdotnet/Blog/actions/workflows/codeql.yml/badge.svg)](https://github.com/linkdotnet/Blog/actions/workflows/codeql.yml)

This is a blog software completely written in C# / Blazor. The aim is to have it configurable as possible.

## How does it work

The basic idea is that the content creator writes their posts in markdown language (like this readme file).
The markdown will then be translated into HTML and displayed to the client. This gives an easy entry to writing posts with all the flexibility markdown has.
This also includes source code snippets. Highlighting is done via [highlight.js](https://highlightjs.org/) with the GitHub theme.

## In Action

![overview](assets/overview.gif)

## Documentation

-   [Authorization](./docs/Authorization/Readme.md)
-   [Comments](./docs/Comments/Readme.md)
-   [Storage Provider](./docs/Storage/Readme.md)
-   [Media Upload](./docs/Media/Readme.md)
-   [Search Engine Optimization (SEO)](./docs/SEO/Readme.md)
-   [Advanced Features](./docs/Features/AdvancedFeatures.md)

## Installation

-   [Installation Instructions](./docs/Setup/Readme.md)
-   Releases on [github.com](https://github.com/linkdotnet/Blog/releases)

## License

This project is released under the terms of the [MIT License](./LICENSE).

## Support & Contributing

Thanks to all [contributors](https://github.com/linkdotnet/Blog/graphs/contributors) and people that are creating bug-reports and valuable input:

<a href="https://github.com/linkdotnet/blog/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=linkdotnet/blog" alt="Supporters" />
</a>

## Try it out with **Codespaces**

This repository offers a [GitHub Codespace](https://github.com/features/codespaces) where you can easily run and modify the Blog completely in your browser, no IDE or local .net installation needed.

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://codespaces.new/linkdotnet/Blog)

## Resources
You want a visual walkthrough through the features and details? The awesome [@ncosentino / DevLeader](https://github.com/ncosentino/) has a YouTube video/series: 
 * [*"WordPress Is A DUMPSTER FIRE - Build A Blog In Blazor!"*](https://www.youtube.com/watch?v=RGq2s25xTPE).
 * [*"WordPress is HISTORY! Get Your Own Blazor Blog Running TODAY!"*](https://www.youtube.com/watch?v=A2vAO7jxFz4)