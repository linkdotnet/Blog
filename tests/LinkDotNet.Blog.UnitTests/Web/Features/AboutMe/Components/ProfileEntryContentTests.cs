using LinkDotNet.Blog.Web.Features.AboutMe.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe.Components;

public class ProfileEntryContentTests
{
    [Theory]
    [InlineData("Birthday: 17.05.1991", "Birthday", "17.05.1991")]
    [InlineData("Hobbies: Playing piano (calisthenics)", "Hobbies", "Playing piano (calisthenics)")]
    [InlineData("Blog: [link](https://example.com)", "Blog", "[link](https://example.com)")]
    public void ShouldSplitLabelAndValue(string content, string label, string body)
    {
        var result = ProfileEntryContent.Parse(content);

        result.Label.ShouldBe(label);
        result.Body.ShouldBe(body);
    }

    [Theory]
    [InlineData("Speaks German and English")]
    [InlineData("[Microsoft MVP](https://mvp.microsoft.com)")]
    [InlineData("https://example.com")]
    [InlineData("**Note**: bold label stays markdown")]
    [InlineData("<a href=\"x\" style=\"color: red\">Get in touch</a>")]
    [InlineData("A label that is way too long to be a label: value")]
    [InlineData("Trailing colon:")]
    public void ShouldKeepContentAsBody(string content)
    {
        var result = ProfileEntryContent.Parse(content);

        result.Label.ShouldBeNull();
        result.Body.ShouldBe(content);
    }
}
