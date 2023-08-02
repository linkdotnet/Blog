# LinkDotNet.Blog

[![.NET](https://github.com/linkdotnet/Blog/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/linkdotnet/Blog/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/linkdotnet/Blog/actions/workflows/codeql.yml/badge.svg)](https://github.com/linkdotnet/Blog/actions/workflows/codeql.yml)

This is a blog software completely written in C# / Blazor. The aim is to have it configurable as possible.

## How does it work

The basic idea is that the content creator writes his posts in markdown language (like this readme file).
The markdown will then be translated into HTML and displayed to the client. This gives an easy entry to writing posts with all the flexibility markdown has.
This also includes source code snippets. Highlighting is done via [highlight.js](https://highlightjs.org/) with the GitHub theme.

## In Action

![overview](assets/overview.gif)

## Components

-   [Authorization](./docs/Authorization/Readme.md)
-   [Comments](./docs/Comments/Readme.md)
-   [Storage Provider](./docs/Storage/Readme.md)
-   [Search Engine Optimization (SEO)](./docs/SEO/Readme.md)
-   [Setup](./docs/Setup/Readme.md)
