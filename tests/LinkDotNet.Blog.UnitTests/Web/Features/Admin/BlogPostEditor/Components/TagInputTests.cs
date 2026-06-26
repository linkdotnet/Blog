using System.Collections.Generic;
using System.Linq;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BlogPostEditor.Components;

public class TagInputTests : BunitContext
{
    [Fact]
    public void RendersExistingCommaSeparatedTagsAsPills()
    {
        var cut = Render<TagInput>(parameters => parameters
            .Add(component => component.Value, "dotnet,blazor"));

        cut.Markup.ShouldContain("dotnet");
        cut.Markup.ShouldContain("blazor");
        cut.FindAll(".badge").Count.ShouldBe(2);
    }

    [Fact]
    public void SelectingSuggestionUpdatesCommaSeparatedValue()
    {
        var value = "dotnet";
        var cut = Render<TagInput>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ValueChanged, updated => value = updated)
            .Add(component => component.Suggestions, new List<string> { "blazor", "csharp" }));

        cut.Find("#tags").Input("bla");
        cut.FindAll(".list-group-item").Single().MouseDown();

        value.ShouldBe("dotnet,blazor");
    }

    [Fact]
    public void FreeFormCommaEntryCreatesPills()
    {
        var value = string.Empty;
        var cut = Render<TagInput>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ValueChanged, updated => value = updated));

        cut.Find("#tags").Input("dotnet,blazor");

        value.ShouldBe("dotnet,blazor");
        cut.FindAll(".badge").Count.ShouldBe(2);
    }

    [Fact]
    public void DuplicateTagsAreIgnoredCaseInsensitively()
    {
        var value = "DotNet";
        var cut = Render<TagInput>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ValueChanged, updated => value = updated));

        cut.Find("#tags").Input("dotnet,Blazor");

        value.ShouldBe("DotNet,Blazor");
        cut.FindAll(".badge").Count.ShouldBe(2);
    }

    [Fact]
    public void RemovingPillUpdatesValue()
    {
        var value = "dotnet,blazor";
        var cut = Render<TagInput>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ValueChanged, updated => value = updated));

        cut.Find("button[aria-label='Remove dotnet']").Click();

        value.ShouldBe("blazor");
        cut.Markup.ShouldNotContain("dotnet");
    }

    [Fact]
    public void PastedCommaSeparatedTagsAreNormalized()
    {
        var value = string.Empty;
        var cut = Render<TagInput>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ValueChanged, updated => value = updated));

        cut.Find("#tags").Input(" alpha, beta , , Gamma ");

        value.ShouldBe("alpha,beta,Gamma");
    }

    [Fact]
    public void KeyboardCanSelectSuggestions()
    {
        var value = string.Empty;
        var cut = Render<TagInput>(parameters => parameters
            .Add(component => component.Value, value)
            .Add(component => component.ValueChanged, updated => value = updated)
            .Add(component => component.Suggestions, new List<string> { "blazor" }));

        var input = cut.Find("#tags");
        input.Input("bla");
        input.KeyDown(new KeyboardEventArgs { Key = "Enter" });

        value.ShouldBe("blazor");
    }
}
