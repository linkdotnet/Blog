using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared
{
    public class TitleTests : TestContext
    {
        [Fact]
        public void ShouldSetTitle()
        {
            JSInterop.Mode = JSRuntimeMode.Loose;

            RenderComponent<Title>(c => c.Add(p => p.Value, "New Title"));

            JSInterop.VerifyInvoke("setDocumentTitle").Arguments[0].Should().Be("New Title");
        }
    }
}