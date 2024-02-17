namespace LinkDotNet.Blog.Web.Features.Components;

public readonly record struct SelectionRange
{
    public int Start { get; init; }

    public int End { get; init; }
}
