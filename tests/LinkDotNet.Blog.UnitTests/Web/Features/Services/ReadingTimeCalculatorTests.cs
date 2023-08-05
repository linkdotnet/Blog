using LinkDotNet.Blog.Web.Features.Services;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Services;

public class ReadingTimeCalculatorTests
{
    [Fact]
    public void ShouldCountShortContent()
    {
        const string content = "```csharp\nvar a = 1;\n```";

        var result = ReadingTimeCalculator.CalculateReadingTime(content);

        result.Should().Be(1);
    }
}
