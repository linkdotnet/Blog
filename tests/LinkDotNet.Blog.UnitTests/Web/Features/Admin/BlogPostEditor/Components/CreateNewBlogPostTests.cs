using System;
using System.Linq;
using Blazored.Toast.Services;
using Bunit;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BlogPostEditor.Components;

public class CreateNewBlogPostTests : TestContext
{
    public CreateNewBlogPostTests()
    {
        ComponentFactories.AddStub<UploadFile>();
        Services.AddScoped(_ => Mock.Of<IToastService>());
    }

    [Fact]
    public void ShouldCreateNewBlogPostWhenValidDataGiven()
    {
        BlogPost blogPost = null;
        var cut = RenderComponent<CreateNewBlogPost>(
            p => p.Add(c => c.OnBlogPostCreated, bp => blogPost = bp));
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#fallback-preview").Change("My fallback preview url");
        cut.Find("#published").Change(false);
        cut.Find("#tags").Change("Tag1,Tag2,Tag3");

        cut.Find("form").Submit();

        cut.WaitForState(() => cut.Find("#title").TextContent == string.Empty);
        blogPost.Should().NotBeNull();
        blogPost.Title.Should().Be("My Title");
        blogPost.ShortDescription.Should().Be("My short Description");
        blogPost.Content.Should().Be("My content");
        blogPost.PreviewImageUrl.Should().Be("My preview url");
        blogPost.PreviewImageUrlFallback.Should().Be("My fallback preview url");
        blogPost.IsPublished.Should().BeFalse();
        blogPost.UpdatedDate.Should().NotBe(default);
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
        var cut = RenderComponent<CreateNewBlogPost>(
            p =>
                p.Add(c => c.OnBlogPostCreated, bp => blogPostFromComponent = bp)
                 .Add(c => c.BlogPost, blogPost));

        cut.Find("form").Submit();

        cut.WaitForState(() => cut.Find("#title").TextContent == string.Empty);
        blogPostFromComponent.Should().NotBeNull();
        blogPostFromComponent.Title.Should().Be("Title");
        blogPostFromComponent.ShortDescription.Should().Be("Desc");
        blogPostFromComponent.Content.Should().Be("Content");
        blogPostFromComponent.Tags.Select(t => t.Content).Should().Contain("tag1");
        blogPostFromComponent.Tags.Select(t => t.Content).Should().Contain("tag2");
    }

    [Fact]
    public void ShouldNotDeleteModelWhenSet()
    {
        BlogPost blogPost = null;
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
        var cut = RenderComponent<CreateNewBlogPost>();

        var found = cut.FindAll("#updatedate");

        found.Should().HaveCount(0);
    }

    [Fact]
    public void ShouldAcceptInputWithoutLosingFocusOrEnter()
    {
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

    [Fact(Skip = "Need bUnit > 1.9.8")]
    public void ShouldStopExternalNavigationWhenDirty()
    {
        var cut = RenderComponent<CreateNewBlogPost>();

        cut.Find("#title").Change("Hey");

        cut.FindComponent<NavigationLock>().Instance.ConfirmExternalNavigation.Should().BeTrue();
    }

    [Fact(Skip = "Need bUnit > 1.9.8")]
    public void ShouldStopInternalNavigationWhenDirty()
    {
        var cut = RenderComponent<CreateNewBlogPost>();
        cut.Find("#tags").Change("Hey");
        var fakeNavigationManager = Services.GetRequiredService<FakeNavigationManager>();

        fakeNavigationManager.NavigateTo("/internal");

        fakeNavigationManager.History.Count.Should().Be(1);
    }

    [Fact(Skip = "Need bUnit > 1.9.8")]
    public void ShouldNotPreventWhenToastIsClicked()
    {
        var toastMock = new Mock<IToastService>();
        Services.AddScoped(_ => toastMock.Object);
        toastMock.Setup(t => t.ShowWarning(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action>()))
            .Callback<string, string, Action>((_, _, onClick) => onClick());
        var cut = RenderComponent<CreateNewBlogPost>();
        cut.Find("#tags").Change("Hey");
        var fakeNavigationManager = Services.GetRequiredService<FakeNavigationManager>();
        fakeNavigationManager.NavigateTo("/internal");

        fakeNavigationManager.NavigateTo("/internal");

        fakeNavigationManager.History.Count.Should().Be(2);
    }
}