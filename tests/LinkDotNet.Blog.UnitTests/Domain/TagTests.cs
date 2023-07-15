using System;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class TagTests
{
    [Fact]
    public void ShouldCreateTag()
    {
        var tag = Tag.Create(" Test ");

        tag.Content.Should().Be("Test");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void ShouldThrowExceptionIfInvalid(string content)
    {
        Action act = () => Tag.Create(content);

        act.Should().Throw<ArgumentException>();
    }
}
