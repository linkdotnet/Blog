using LinkDotNet.Blog.Web.Features.ShowBlogPost;

namespace LinkDotNet.Blog.UnitTests.Web.Features.ShowBlogPost;

public class LikeLedgerTests
{
    [Fact]
    public void ShouldAllowOnlyOneNetLikePerBlogPost()
    {
        var sut = new LikeLedger();

        sut.TryRecord("1", true).ShouldBeTrue();
        sut.TryRecord("1", true).ShouldBeFalse();
        sut.TryRecord("1", false).ShouldBeTrue();
        sut.TryRecord("1", true).ShouldBeTrue();
        sut.TryRecord("2", true).ShouldBeTrue();
    }

    [Fact]
    public void ShouldAllowOnlyOneNetUnlikePerBlogPost()
    {
        var sut = new LikeLedger();

        sut.TryRecord("1", false).ShouldBeTrue();
        sut.TryRecord("1", false).ShouldBeFalse();
    }
}
