using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Markdig;
using Markdig.Extensions.Alerts;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace LinkDotNet.Blog.Web.Features;

internal sealed class CalloutExtension : IMarkdownExtension
{
    private static readonly FrozenDictionary<string, (string Icon, string Title)> Callouts =
        new Dictionary<string, (string Icon, string Title)>
        {
            ["NOTE"] = ("bi-info-circle", "Note"),
            ["TIP"] = ("bi-lightbulb", "Tip"),
            ["IMPORTANT"] = ("bi-exclamation-square", "Important"),
            ["WARNING"] = ("bi-exclamation-triangle", "Warning"),
            ["CAUTION"] = ("bi-exclamation-octagon", "Caution"),
        }.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        ArgumentNullException.ThrowIfNull(pipeline);
        pipeline.DocumentProcessed += UseThemeClasses;
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        if (renderer is not HtmlRenderer htmlRenderer)
        {
            return;
        }

        // UseBootstrap blanks the alert title, so this has to run after the Bootstrap extension
        var alertBlockRenderer = htmlRenderer.ObjectRenderers.OfType<AlertBlockRenderer>().FirstOrDefault();
        alertBlockRenderer?.RenderKind = RenderTitle;
    }

    // UseBootstrap emits Bootstrap 5 "alert-{color}" classes; Bootstrap 6 colors alerts via "theme-{color}" and lays them out as a row
    private static void UseThemeClasses(MarkdownDocument document)
    {
        foreach (var alert in document.Descendants<AlertBlock>())
        {
            var classes = alert.TryGetAttributes()?.Classes;
            if (classes is null)
            {
                continue;
            }

            for (var i = 0; i < classes.Count; i++)
            {
                if (classes[i].StartsWith("alert-", StringComparison.Ordinal))
                {
                    classes[i] = "theme-" + classes[i]["alert-".Length..];
                }
            }

            classes.Add("flex-column");
        }
    }

    private static void RenderTitle(HtmlRenderer renderer, StringSlice kind)
    {
        if (!Callouts.TryGetValue(kind.ToString(), out var callout))
        {
            return;
        }

        renderer.WriteLine($"""<p class="markdown-alert-title d-flex align-items-center gap-3 fw-semibold mb-3"><i class="bi {callout.Icon}" aria-hidden="true"></i>{callout.Title}</p>""");
    }
}
