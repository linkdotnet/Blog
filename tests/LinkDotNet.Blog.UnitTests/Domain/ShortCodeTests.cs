using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class ShortCodeTests
{
    [Fact]
    public void ShouldExpandAllShortCodes()
    {
        var shortCodes = new[] { ShortCode.Create("a", "**A**"), ShortCode.Create("b", "_B_") };

        var result = ShortCode.Expand("[[a]] and [[b]] and [[a]]", shortCodes);

        result.ShouldBe("**A** and _B_ and **A**");
    }

    [Fact]
    public void ShouldLeaveUnknownTokensUntouched()
    {
        var result = ShortCode.Expand("[[unknown]]", [ShortCode.Create("a", "A")]);

        result.ShouldBe("[[unknown]]");
    }
}
