using System;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class ProfileInformationEntryTests
{
    [Fact]
    public void ShouldCreateObject()
    {
        var result = ProfileInformationEntry.Create("key", 12);

        result.Content.ShouldBe("key");
        result.SortOrder.ShouldBe(12);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void ShouldThrowExceptionWhenEmptyKeyOrValue(string? content)
    {
        Action act = () => ProfileInformationEntry.Create(content!, 0);

        act.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void ShouldTrim()
    {
        var result = ProfileInformationEntry.Create("   key ", 0);

        result.Content.ShouldBe("key");
    }
}
