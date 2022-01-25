using System;
using System.Linq;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Blog.Web.Shared.Admin;
using LinkDotNet.Blog.Web.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared.Admin;

public class CreateNewBlogPostTests : TestContext
{
    public CreateNewBlogPostTests()
    {
        ComponentFactories.AddStub<UploadFile>();
    }

    [Fact]
    public void ShouldCreateNewBlogPostWhenValidDataGiven()
    {
        Services.AddScoped(_ => Mock.Of<IMarkerService>());
        BlogPost blogPost = null;
        var cut = RenderComponent<CreateNewBlogPost>(
            p => p.Add(c => c.OnBlogPostCreated, bp => blogPost = bp));
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#published").Change(false);
        cut.Find("#tags").Change("Tag1,Tag2,Tag3");

        cut.Find("form").Submit();

        cut.WaitForState(() => cut.Find("#title").TextContent == string.Empty);
        blogPost.Should().NotBeNull();
        blogPost.Title.Should().Be("My Title");
        blogPost.ShortDescription.Should().Be("My short Description");
        blogPost.Content.Should().Be("My content");
        blogPost.PreviewImageUrl.Should().Be("My preview url");
        blogPost.IsPublished.Should().BeFalse();
        blogPost.Tags.Should().HaveCount(3);
        blogPost.Tags.Select(t => t.Content).Should().Contain(new[] { "Tag1", "Tag2", "Tag3" });
    }

    [Fact]
    public void ShouldFillGivenBlogPost()
    {
        var blogPost = new BlogPostBuilder()
            .WithTitle("Title")
            .WithShortDescription("Desc")
            .WithContent("Content")
            .WithTags("tag1", "tag2")
            .Build();
        BlogPost blogPostFromComponent = null;
        Services.AddScoped(_ => Mock.Of<IMarkerService>());
        var cut = RenderComponent<CreateNewBlogPost>(
            p =>
                p.Add(c => c.OnBlogPostCreated, bp => blogPostFromComponent = bp)
                 .Add(c => c.BlogPost, blogPost));
        cut.Find("#title").Input("My new Title");

        cut.Find("form").Submit();

        cut.WaitForState(() => cut.Find("#title").TextContent == string.Empty);
        blogPostFromComponent.Should().NotBeNull();
        blogPostFromComponent.Title.Should().Be("My new Title");
        blogPostFromComponent.ShortDescription.Should().Be("Desc");
        blogPostFromComponent.Content.Should().Be("Content");
        blogPostFromComponent.Tags.Select(t => t.Content).Should().Contain("tag1");
        blogPostFromComponent.Tags.Select(t => t.Content).Should().Contain("tag2");
    }

    [Fact]
    public void ShouldNotDeleteModelWhenSet()
    {
        BlogPost blogPost = null;
        Services.AddScoped(_ => Mock.Of<IMarkerService>());
        var cut = RenderComponent<CreateNewBlogPost>(
            p => p.Add(c => c.ClearAfterCreated, true)
                .Add(c => c.OnBlogPostCreated, post => blogPost = post));
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#tags").Change("Tag1,Tag2,Tag3");
        cut.Find("form").Submit();
        blogPost = null;

        cut.Find("form").Submit();

        blogPost.Should().BeNull();
    }

    [Fact]
    public void ShouldNotDeleteModelWhenNotSet()
    {
        BlogPost blogPost = null;
        Services.AddScoped(_ => Mock.Of<IMarkerService>());
        var cut = RenderComponent<CreateNewBlogPost>(
            p => p.Add(c => c.ClearAfterCreated, false)
                .Add(c => c.OnBlogPostCreated, post => blogPost = post));
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#tags").Change("Tag1,Tag2,Tag3");
        cut.Find("form").Submit();
        blogPost = null;

        cut.Find("form").Submit();

        blogPost.Should().NotBeNull();
    }

    [Fact]
    public void ShouldNotUpdateUpdatedDateWhenCheckboxSet()
    {
        Services.AddScoped(_ => Mock.Of<IMarkerService>());
        var somewhen = new DateTime(1991, 5, 17);
        var originalBlogPost = new BlogPostBuilder().WithUpdatedDate(somewhen).Build();
        BlogPost blogPostFromComponent = null;
        var cut = RenderComponent<CreateNewBlogPost>(
            p =>
                p.Add(c => c.OnBlogPostCreated, bp => blogPostFromComponent = bp)
                    .Add(c => c.BlogPost, originalBlogPost));

        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#tags").Change("Tag1,Tag2,Tag3");
        cut.Find("#updatedate").Change(false);
        cut.Find("form").Submit();

        blogPostFromComponent.UpdatedDate.Should().Be(somewhen);
    }

    [Fact]
    public void ShouldNotSetOptionToNotUpdateUpdatedDateOnInitialCreate()
    {
        Services.AddScoped(_ => Mock.Of<IMarkerService>());
        var cut = RenderComponent<CreateNewBlogPost>();

        var found = cut.FindAll("#updatedate");

        found.Should().HaveCount(0);
    }

    [Fact]
    public void ShouldAcceptInputWithoutLosingFocusOrEnter()
    {
        Services.AddScoped(_ => Mock.Of<IMarkerService>());
        BlogPost blogPost = null;
        var cut = RenderComponent<CreateNewBlogPost>(
            p => p.Add(c => c.OnBlogPostCreated, bp => blogPost = bp));
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#published").Change(false);
        cut.Find("#tags").Change("Tag1,Tag2,Tag3");

        cut.Find("form").Submit();

        cut.WaitForState(() => cut.Find("#title").TextContent == string.Empty);
        blogPost.Should().NotBeNull();
        blogPost.Title.Should().Be("My Title");
        blogPost.ShortDescription.Should().Be("My short Description");
        blogPost.Content.Should().Be("My content");
        blogPost.PreviewImageUrl.Should().Be("My preview url");
        blogPost.IsPublished.Should().BeFalse();
        blogPost.Tags.Should().HaveCount(3);
        blogPost.Tags.Select(t => t.Content).Should().Contain(new[] { "Tag1", "Tag2", "Tag3" });
    }

    [Theory]
    [InlineData("short", "b", true, "**", "**Test**")]
    [InlineData("short", "i", true, "*", "*Test*")]
    [InlineData("short", "b", false, "**", "Test")]
    [InlineData("short", "f", false, "**", "Test")]
    [InlineData("content", "b", true, "**", "**Test**")]
    [InlineData("content", "i", true, "*", "*Test*")]
    [InlineData("content", "b", false, "**", "Test")]
    [InlineData("content", "f", false, "**", "Test")]
    public void ShouldSetMarkerOnKeyUp(string id, string key, bool ctrlPressed, string fence, string expected)
    {
        var markerMock = new Mock<IMarkerService>();
        markerMock.Setup(m => m.GetNewMarkdownForElementAsync(id, "Test", fence, fence))
            .ReturnsAsync(expected);
        Services.AddScoped(_ => markerMock.Object);
        var cut = RenderComponent<CreateNewBlogPost>();
        cut.Find($"#{id}").Input("Test");
        cut.Find($"#{id}").KeyUp(key, ctrlKey: ctrlPressed);

        var content = cut.Find($"#{id}").TextContent;

        content.Should().Be(expected);
    }
}