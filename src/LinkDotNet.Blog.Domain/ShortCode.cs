using System;
using System.Collections.Generic;
using System.Text;

namespace LinkDotNet.Blog.Domain;

public class ShortCode : Entity
{
    // RavenDB's serializer only uses parameterless constructors.
    private ShortCode()
    {
        Name = default!;
        MarkdownContent = default!;
    }

    private ShortCode(string name, string markdownContent)
    {
        Name = name;
        MarkdownContent = markdownContent;
    }

    public string MarkdownContent { get; private set; }

    public string Name { get; set; }

    public string Token => $"[[{Name}]]";

    public void Update(string name, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        MarkdownContent = content;
        Name = name;
    }

    public static ShortCode Create(string name, string content)
    {
        return new ShortCode(name, content);
    }

    public static string Expand(string markdown, IEnumerable<ShortCode> shortCodes)
    {
        ArgumentNullException.ThrowIfNull(shortCodes);

        var sb = new StringBuilder(markdown);
        foreach (var shortCode in shortCodes)
        {
            sb.Replace(shortCode.Token, shortCode.MarkdownContent);
        }

        return sb.ToString();
    }
}
