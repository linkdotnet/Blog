using Blazored.LocalStorage;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared
{
    public class LikeTests : TestContext
    {
        [Theory]
        [InlineData(0, "0 Likes")]
        [InlineData(1, "1 Like")]
        [InlineData(2, "2 Likes")]
        public void ShouldDisplayLikes(int likes, string expectedText)
        {
            Services.AddScoped(_ => new Mock<ILocalStorageService>().Object);
            var blogPost = new BlogPostBuilder().WithLikes(likes).Build();
            var cut = RenderComponent<Like>(
                p => p.Add(l => l.BlogPost, blogPost));

            var label = cut.Find("small").TextContent;

            label.Should().Be(expectedText);
        }

        [Fact]
        public void ShouldInvokeEventWhenButtonClicked()
        {
            Services.AddScoped(_ => new Mock<ILocalStorageService>().Object);
            var blogPost = new BlogPostBuilder().Build();
            var wasClicked = false;
            var wasLike = false;
            var cut = RenderComponent<Like>(
                p => p.Add(l => l.BlogPost, blogPost)
                    .Add(l => l.OnBlogPostLiked, b =>
                    {
                        wasClicked = true;
                        wasLike = b;
                    }));

            cut.Find("button").Click();

            wasClicked.Should().BeTrue();
            wasLike.Should().BeTrue();
        }

        [Fact]
        public void ShouldSetLocalStorageVariableOnClick()
        {
            var localStorage = new Mock<ILocalStorageService>();
            Services.AddScoped(_ => localStorage.Object);
            var blogPost = new BlogPostBuilder().Build();
            var cut = RenderComponent<Like>(
                p => p.Add(l => l.BlogPost, blogPost));

            cut.Find("button").Click();

            localStorage.Verify(l => l.SetItemAsync("hasLiked", true, default), Times.Once);
        }

        [Fact]
        public void ShouldCheckLocalStorageOnInit()
        {
            var localStorage = new Mock<ILocalStorageService>();
            localStorage.Setup(l => l.ContainKeyAsync("hasLiked", default)).ReturnsAsync(true);
            localStorage.Setup(l => l.GetItemAsync<bool>("hasLiked", default)).ReturnsAsync(true);
            Services.AddScoped(_ => localStorage.Object);
            var blogPost = new BlogPostBuilder().Build();
            var wasLike = true;
            var cut = RenderComponent<Like>(
                p => p.Add(l => l.BlogPost, blogPost)
                    .Add(l => l.OnBlogPostLiked, b => wasLike = b));

            cut.Find("button").Click();

            wasLike.Should().BeFalse();
        }

        [Fact]
        public void ShouldCheckStorageOnClickAgainAndDoNothingOnMismatch()
        {
            var localStorage = new Mock<ILocalStorageService>();
            Services.AddScoped(_ => localStorage.Object);
            var blogPost = new BlogPostBuilder().Build();
            var wasClicked = false;
            var cut = RenderComponent<Like>(
                p => p.Add(l => l.BlogPost, blogPost)
                    .Add(l => l.OnBlogPostLiked, _ => wasClicked = true));
            localStorage.Setup(l => l.ContainKeyAsync("hasLiked", default)).ReturnsAsync(true);
            localStorage.Setup(l => l.GetItemAsync<bool>("hasLiked", default)).ReturnsAsync(true);

            cut.Find("button").Click();

            wasClicked.Should().BeFalse();
        }
    }
}