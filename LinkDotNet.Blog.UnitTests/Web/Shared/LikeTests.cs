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
    }
}