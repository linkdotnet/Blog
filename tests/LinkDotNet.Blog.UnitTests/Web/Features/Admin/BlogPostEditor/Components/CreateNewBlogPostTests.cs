using System;
using System.Linq;
using AngleSharp.Html.Dom;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.TestUtilities.Fakes;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NCronJob;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BlogPostEditor.Components;

public class CreateNewBlogPostTests : BunitContext
{
    private readonly CacheService cacheService = new CacheService();
    private readonly IOptions<ApplicationConfiguration> options;

    public CreateNewBlogPostTests()
    {
        var shortCodeRepository = Substitute.For<IRepository<ShortCode>>();
        shortCodeRepository.GetAllAsync().Returns(PagedList<ShortCode>.Empty);
        Services.AddScoped(_ => shortCodeRepository);
        JSInterop.SetupVoid("hljs.highlightAll");
        ComponentFactories.Add<MarkdownTextArea, MarkdownFake>();
        Services.AddScoped(_ => Substitute.For<IInstantJobRegistry>());
        Services.AddScoped<ICacheInvalidator>(_ => cacheService);
        Services.AddScoped(_ => Substitute.For<IToastService>());
        options = Substitute.For<IOptions<ApplicationConfiguration>>();

        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = true,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        Services.AddScoped(_ => options);

        var userRecordService = Substitute.For<IUserRecordService>();
        userRecordService.GetDisplayNameAsync().Returns("Test Author");
        Services.AddScoped(_ => userRecordService);
    }

    [Fact]
    public void ShouldCreateNewBlogPostWhenMultiAuthorModeIsEnabled()
    {
        BlogPost? blogPost = null;
        var cut = Render<CreateNewBlogPost>(
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
        blogPost.ShouldNotBeNull();
        blogPost.Title.ShouldBe("My Title");
        blogPost.ShortDescription.ShouldBe("My short Description");
        blogPost.Content.ShouldBe("My content");
        blogPost.PreviewImageUrl.ShouldBe("My preview url");
        blogPost.PreviewImageUrlFallback.ShouldBe("My fallback preview url");
        blogPost.IsPublished.ShouldBeFalse();
        blogPost.UpdatedDate.ShouldNotBe(default);
        blogPost.AuthorName.ShouldBe("Test Author");
        blogPost.Tags.Count.ShouldBe(3);
        blogPost.Tags.ShouldContain("Tag1");
        blogPost.Tags.ShouldContain("Tag2");
        blogPost.Tags.ShouldContain("Tag3");
    }

    [Fact]
    public void ShouldAuthorNameIsNullWhenMultiAuthorModeIsDisable()
    {
        options.Value.Returns(new ApplicationConfiguration()
        {
            UseMultiAuthorMode = false,
            BlogName = "Test",
            ConnectionString = "Test",
            DatabaseName = "Test"
        });

        BlogPost? blogPost = null;
        var cut = Render<CreateNewBlogPost>(
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
        blogPost.ShouldNotBeNull();
        blogPost.AuthorName.ShouldBeNull();
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
        BlogPost? blogPostFromComponent = null;
        var cut = Render<CreateNewBlogPost>(
            p =>
                p.Add(c => c.OnBlogPostCreated, bp => blogPostFromComponent = bp)
                 .Add(c => c.BlogPost, blogPost));

        cut.Find("form").Submit();

        cut.WaitForState(() => cut.Find("#title").TextContent == string.Empty);
        blogPostFromComponent.ShouldNotBeNull();
        blogPostFromComponent.Title.ShouldBe("Title");
        blogPostFromComponent.ShortDescription.ShouldBe("Desc");
        blogPostFromComponent.Content.ShouldBe("Content");
        blogPostFromComponent.Tags.ShouldContain("tag1");
        blogPostFromComponent.Tags.ShouldContain("tag2");
    }

    [Fact]
    public void ShouldNotDeleteModelWhenSet()
    {
        BlogPost? blogPost = null;
        var cut = Render<CreateNewBlogPost>(
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

        blogPost.ShouldBeNull();
    }

    [Fact]
    public void ShouldNotDeleteModelWhenNotSet()
    {
        BlogPost? blogPost = null;
        var cut = Render<CreateNewBlogPost>(
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

        blogPost.ShouldNotBeNull();
    }

    [Fact]
    public void ShouldNotUpdateUpdatedDateWhenCheckboxSet()
    {
        var someWhen = new DateTime(1991, 5, 17);
        var originalBlogPost = new BlogPostBuilder().WithUpdatedDate(someWhen).Build();
        BlogPost? blogPostFromComponent = null;
        var cut = Render<CreateNewBlogPost>(
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

        blogPostFromComponent.ShouldNotBeNull();
        blogPostFromComponent.UpdatedDate.ShouldBe(someWhen);
    }

    [Fact]
    public void ShouldNotSetOptionToNotUpdateUpdatedDateOnInitialCreate()
    {
        var cut = Render<CreateNewBlogPost>();

        var found = cut.FindAll("#updatedate");

        found.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldAcceptInputWithoutLosingFocusOrEnter()
    {
        BlogPost? blogPost = null;
        var cut = Render<CreateNewBlogPost>(
            p => p.Add(c => c.OnBlogPostCreated, bp => blogPost = bp));
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#published").Change(false);
        cut.Find("#tags").Change("Tag1,Tag2,Tag3");

        cut.Find("form").Submit();

        cut.WaitForState(() => cut.Find("#title").TextContent == string.Empty);
        blogPost.ShouldNotBeNull();
        blogPost.Title.ShouldBe("My Title");
        blogPost.ShortDescription.ShouldBe("My short Description");
        blogPost.Content.ShouldBe("My content");
        blogPost.PreviewImageUrl.ShouldBe("My preview url");
        blogPost.IsPublished.ShouldBeFalse();
        blogPost.Tags.Count.ShouldBe(3);
        blogPost.Tags.ShouldContain("Tag1");
        blogPost.Tags.ShouldContain("Tag2");
        blogPost.Tags.ShouldContain("Tag3");
    }

    [Fact]
    public void ShouldStopExternalNavigationWhenDirty()
    {
        var cut = Render<CreateNewBlogPost>();

        cut.Find("#title").Input("Hey");

        cut.FindComponent<NavigationLock>().Instance.ConfirmExternalNavigation.ShouldBeTrue();
    }

    [Fact]
    public void ShouldStopInternalNavigationWhenDirty()
    {
        JSInterop.Setup<bool>("confirm", "You have unsaved changes. Are you sure you want to continue?")
            .SetResult(false);
        var cut = Render<CreateNewBlogPost>();
        cut.Find("#tags").Change("Hey");
        var fakeNavigationManager = Services.GetRequiredService<BunitNavigationManager>();

        fakeNavigationManager.NavigateTo("/internal");

        fakeNavigationManager.History.Count.ShouldBe(1);
        fakeNavigationManager.History.Single().State.ShouldBe(NavigationState.Prevented);
    }

    [Fact]
    public void ShouldNotBlogNavigationOnInitialLoad()
    {
        var blogPost = new BlogPostBuilder().Build();
        Render<CreateNewBlogPost>(
            p => p.Add(s => s.BlogPost, blogPost));
        var fakeNavigationManager = Services.GetRequiredService<BunitNavigationManager>();

        fakeNavigationManager.NavigateTo("/internal");

        fakeNavigationManager.History.Count.ShouldBe(1);
        fakeNavigationManager.History.Single().State.ShouldBe(NavigationState.Succeeded);
    }

    [Fact]
    public void GivenBlogPostWithSchedule_ShouldSetSchedule()
    {
        BlogPost? blogPost = null;
        var cut = Render<CreateNewBlogPost>(
            p => p.Add(c => c.OnBlogPostCreated, bp => blogPost = bp));
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#published").Change(false);
        cut.Find("#scheduled").Change("01/01/2099 00:00");

        cut.Find("form").Submit();

        blogPost.ShouldNotBeNull();
        blogPost.ScheduledPublishDate.ShouldBe(new DateTime(2099, 01, 01));
    }

    [Fact]
    public void GivenBlogPost_WhenEnteringScheduledDate_ThenIsPublishedSetToFalse()
    {
        BlogPost? blogPost = null;
        var cut = Render<CreateNewBlogPost>(
            p => p.Add(c => c.OnBlogPostCreated, bp => blogPost = bp));
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#published").Change(true);

        cut.Find("#scheduled").Change("01/01/2099 00:00");

        var element = cut.Find("#published") as IHtmlInputElement;
        element.ShouldNotBeNull();
        element.IsChecked.ShouldBeFalse();
    }
    
    [Fact]
    public void GivenBlogPost_WhenCacheInvalidatedOptionIsSet_CacheIsInvalidated()
    {
        var cut = Render<CreateNewBlogPost>();
        var token = cacheService.Token;
        cut.Find("#title").Input("My Title");
        cut.Find("#short").Input("My short Description");
        cut.Find("#content").Input("My content");
        cut.Find("#preview").Change("My preview url");
        cut.Find("#published").Change(false);
        cut.Find("#invalidate-cache").Change(true);

        cut.Find("form").Submit();

        token.IsCancellationRequested.ShouldBeTrue();
    }

    [Fact]
    public void ShouldTransformHtmlToMarkdown()
    {
        var cut = Render<CreateNewBlogPost>();
        var content = cut.Find("#content");
        content.Input("<h3>My Content</h3>");
        var btnConvert = cut.Find("#convert");
        btnConvert.Click();
        content.TextContent.ShouldBeEquivalentTo("### My Content");
        btnConvert.TextContent.Trim().ShouldBeEquivalentTo("Restore");
    }

    [Fact]
    public void ShouldRestoreMarkdownToHtml()
    {
        var cut = Render<CreateNewBlogPost>();
        string htmlContent = "<h3>My Content</h3>";
        string markdownContent = "### My Content";
        var content = cut.Find("#content");

        content.Input(htmlContent);
        var btnConvert = cut.Find("#convert");
        btnConvert.Click();
        content.TextContent.ShouldBeEquivalentTo(markdownContent);
        btnConvert.TextContent.Trim().ShouldBeEquivalentTo("Restore");

        btnConvert.Click();
        content.TextContent.ShouldBeEquivalentTo(htmlContent);
        btnConvert.TextContent.Trim().ShouldBeEquivalentTo("Convert to markdown");
    }
}
