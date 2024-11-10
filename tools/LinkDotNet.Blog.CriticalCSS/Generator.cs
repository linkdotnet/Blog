using Microsoft.Playwright;

namespace LinkDotNet.Blog.CriticalCSS;

internal static class CriticalCssGenerator
{
    public static async Task<string> GenerateAsync(IReadOnlyCollection<string>urls)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync();
        var criticalCss = new HashSet<string>();

        var viewports = new[]
        {
            new ViewportSize { Width = 1920, Height = 1080 },
            new ViewportSize { Width = 375, Height = 667 }
        };

        foreach (var viewport in viewports)
        {
            foreach (var url in urls)
            {
                var page = await browser.NewPageAsync();
                await page.GotoAsync(url);
                await page.SetViewportSizeAsync(viewport.Width, viewport.Height);

                var usedCss = await page.EvaluateAsync<string[]>(
                    """
                    () => {
                            const styleSheets = Array.from(document.styleSheets);
                            const usedRules = new Set();

                            const viewportHeight = window.innerHeight;
                            const elements = document.querySelectorAll('*');
                            const aboveFold = Array.from(elements).filter(el => {
                                const rect = el.getBoundingClientRect();
                                return rect.top < viewportHeight;
                            });

                            styleSheets.forEach(sheet => {
                                try {
                                    Array.from(sheet.cssRules).forEach(rule => {
                                        if (rule.type === 1) {
                                            aboveFold.forEach(el => {
                                                if (el.matches(rule.selectorText)) {
                                                    usedRules.add(rule.cssText);
                                                }
                                            });
                                        }
                                    });
                                } catch (e) {
                                }
                            });

                            return Array.from(usedRules);
                        }
                    """);

                foreach (var css in usedCss)
                {
                    criticalCss.Add(css);
                }
            }
        }

        var criticalCssContent = string.Join(string.Empty, criticalCss).Replace("@", "@@", StringComparison.OrdinalIgnoreCase);
        var styleTag = $"<style>{criticalCssContent}</style>";

        return styleTag;
    }
}
