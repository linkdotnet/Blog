using System;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class TalkTests
{
    [Fact]
    public void CreateTalkWithTrimmedWhiteSpaces()
    {
        var talk = Talk.Create(" title ", " place ", " desc ", new DateTime(2022, 10, 2));

        talk.PresentationTitle.Should().Be("title");
        talk.Place.Should().Be("place");
        talk.Description.Should().Be("desc");
        talk.PublishedDate.Should().Be(new DateTime(2022, 10, 2));
    }

    [Theory]
    [InlineData(null, "place", "desc")]
    [InlineData("", "place", "desc")]
    [InlineData(" ", "place", "desc")]
    [InlineData("title", null, "desc")]
    [InlineData("title", "", "desc")]
    [InlineData("title", " ", "desc")]
    [InlineData("title", "place", null)]
    [InlineData("title", "place", "")]
    [InlineData("title", "place", " ")]
    public void TalkWithInvalidInvariantShouldNotBeCreated(string title, string place, string desc)
    {
        Action act = () => Talk.Create(title, place, desc, DateTime.MinValue);

        act.Should().Throw<Exception>();
    }
}