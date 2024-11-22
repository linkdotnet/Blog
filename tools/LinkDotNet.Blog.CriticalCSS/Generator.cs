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
                async () => {
                    const styleSheets = Array.from(document.styleSheets);
                    const usedRules = new Set();
                    const processedUrls = new Set();

                    const viewportHeight = window.innerHeight;
                    const elements = document.querySelectorAll('*');
                    const aboveFold = Array.from(elements).filter(el => {
                        const rect = el.getBoundingClientRect();
                        return rect.top < viewportHeight;
                    });

                    async function fetchExternalStylesheet(url) {
                        if (processedUrls.has(url)) return;
                        processedUrls.add(url);
                        
                        try {
                            const response = await fetch(url);
                            const text = await response.text();
                            const blob = new Blob([text], { type: 'text/css' });
                            const styleSheet = new CSSStyleSheet();
                            await styleSheet.replace(text);
                            return styleSheet;
                        } catch (e) {
                            console.error('Failed to fetch:', url, e);
                            return null;
                        }
                    }

                    async function processStyleSheet(sheet) {
                        try {
                            if (sheet.href) {
                                const externalSheet = await fetchExternalStylesheet(sheet.href);
                                if (externalSheet) {
                                    Array.from(externalSheet.cssRules).forEach(processRule);
                                }
                            }
                            
                            Array.from(sheet.cssRules).forEach(processRule);
                        } catch (e) {
                            if (sheet.href) {
                                console.error('CORS issue with:', sheet.href);
                            }
                        }
                    }

                    function processRule(rule) {
                        switch (rule.type) {
                            case CSSRule.STYLE_RULE:
                                aboveFold.forEach(el => {
                                    try {
                                        if (el.matches(rule.selectorText)) {
                                            usedRules.add(rule.cssText);
                                        }
                                    } catch (e) {}
                                });
                                break;
                            case CSSRule.MEDIA_RULE:
                                if (window.matchMedia(rule.conditionText).matches) {
                                    Array.from(rule.cssRules).forEach(processRule);
                                }
                                break;
                            case CSSRule.IMPORT_RULE:
                                processStyleSheet(rule.styleSheet);
                                break;
                            case CSSRule.FONT_FACE_RULE:
                            case CSSRule.KEYFRAMES_RULE:
                                usedRules.add(rule.cssText);
                                break;
                        }
                    }

                    for (const sheet of styleSheets) {
                        await processStyleSheet(sheet);
                    }

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
