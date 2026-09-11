using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Markdig;
using Markdig.Extensions.Alerts;
using Markdig.Helpers;
using Markdig.Renderers;

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

    private static void RenderTitle(HtmlRenderer renderer, StringSlice kind)
    {
        if (!Callouts.TryGetValue(kind.ToString(), out var callout))
        {
            return;
        }

        renderer.WriteLine($"""<p class="markdown-alert-title d-flex align-items-center gap-2 fw-semibold mb-2"><i class="bi {callout.Icon}" aria-hidden="true"></i>{callout.Title}</p>""");
    }
}
