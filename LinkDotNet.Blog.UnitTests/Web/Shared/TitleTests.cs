using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared
{
    public class TitleTests
    {
        [Fact]
        public void ShouldSetTitle()
        {
            using var ctx = new TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;

            ctx.RenderComponent<Title>(c => c.Add(p => p.Value, "New Title"));

            ctx.JSInterop.VerifyInvoke("setDocumentTitle").Arguments[0].Should().Be("New Title");
        }
    }
}