using System.Linq;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class ReadingTimeCalculatorTests
{
    [Fact]
    public void ShouldCountWordsAndImages()
    {
        var content = string.Join(' ', Enumerable.Repeat("word", 750)) + string.Join(' ', Enumerable.Repeat("![image](url)", 4));

        var result = ReadingTimeCalculator.CalculateReadingTime(content);

        result.Should().Be(5);
    }
    [Fact]
    public void ShouldCountShortContent()
    {
        const string content = "```csharp\nvar a = 1;\n```";

        var result = ReadingTimeCalculator.CalculateReadingTime(content);

        result.Should().Be(1);
    }
}
