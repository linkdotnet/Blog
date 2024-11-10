# Critical CSS Generator

Critical CSS is the minimal set of CSS required to render the initial view of a webpage. Extracting critical CSS improves page load performance by:

* Reducing render-blocking CSS
* Improving First Contentful Paint (FCP)
* Reducing the initial page load size

## Usage
You can run the tool from the command line or directly via `dotnet run`

```bash
dotnet run -- --install-playwright --output console
```

This will install a `Chromium` driver for `Playwright` and output the critical CSS to the console.

For help, run:

```bash
dotnet run -- --help
```