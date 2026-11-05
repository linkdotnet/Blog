using System.Collections.Generic;

namespace LinkDotNet.Blog.Web.Features.Admin.ShortCodes;

public sealed record ShortCodeTemplate(string Title, string Description, string Icon, string SuggestedName, string MarkdownContent)
{
    public static IReadOnlyList<ShortCodeTemplate> All { get; } =
    [
        new(
            "Shoutout",
            "Thank a sponsor or highlight a person or project.",
            "bi-megaphone",
            "shoutout",
            "> [!NOTE]\n> **Shoutout!** This post is made possible by [Your Sponsor](https://example.com)."),
        new(
            "Advertisement",
            "Promote a product, course or book.",
            "bi-bag-heart",
            "promo",
            "### Your product name\n\nOne sentence on why readers will love it.\n\n[Get it now](https://example.com)"),
        new(
            "Newsletter",
            "Invite readers to subscribe.",
            "bi-envelope-paper",
            "newsletter",
            "> [!TIP]\n> **Enjoyed this post?** Get one practical article every week. [Subscribe](https://example.com/newsletter)."),
        new(
            "Disclosure",
            "Affiliate or sponsorship disclaimer.",
            "bi-info-circle",
            "disclosure",
            "*Disclosure: This article contains affiliate links. If you buy through them I earn a small commission at no extra cost to you.*"),
        new(
            "Event",
            "Announce a talk, stream or release.",
            "bi-calendar-event",
            "event",
            "**Come see me live!** I'm speaking at *Your Event* on the 1st of January. [Register](https://example.com/event)"),
    ];
}
