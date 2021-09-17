using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Shared.Services;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared.Services
{
    public class GiscusServiceTests : TestContext
    {
        [Fact]
        public async Task ShouldInitGiscus()
        {
            var giscus = new Giscus();
            JSInterop.Mode = JSRuntimeMode.Loose;
            var sut = new GiscusService(JSInterop.JSRuntime, new AppConfiguration { Giscus = giscus });

            await sut.EnableCommentSection("div");

            var init = JSInterop.Invocations.SingleOrDefault(i => i.Identifier == "initGiscus");
            init.Should().NotBeNull();
            init.Arguments.Should().Contain("giscus");
            init.Arguments.Should().Contain(giscus);
        }
    }
}