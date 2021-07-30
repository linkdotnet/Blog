using System.Linq;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared.Skills;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared.Skills
{
    public class SkillTagTests : TestContext
    {
        [Fact]
        public void ShouldRenderImageAndText()
        {
            var skill = new SkillBuilder().WithSkillName("C#").WithIconUrl("test").Build();

            var cut = RenderComponent<SkillTag>(p => p.Add(
                s => s.Skill, skill));

            cut.Find("span").TextContent.Should().Contain("C#");
            cut.Find("img").Attributes.Single(a => a.Name == "src").Value.Should().Be("test");
        }

        [Fact]
        public void ShouldNotRenderImageWhenNotAvailable()
        {
            var skill = new SkillBuilder().WithIconUrl(null).Build();

            var cut = RenderComponent<SkillTag>(p => p.Add(
                s => s.Skill, skill));

            cut.FindAll("img").Should().HaveCount(0);
        }
    }
}