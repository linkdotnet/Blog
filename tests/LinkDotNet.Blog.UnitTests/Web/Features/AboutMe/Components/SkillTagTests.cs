using System.Linq;
using Bunit;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.AboutMe.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe.Components;

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
        cut.FindAll("button").Should().HaveCount(0);
    }

    [Fact]
    public void ShouldNotRenderImageWhenNotAvailable()
    {
        var skill = new SkillBuilder().WithIconUrl(null).Build();

        var cut = RenderComponent<SkillTag>(p => p.Add(
            s => s.Skill, skill));

        cut.FindAll("img").Should().HaveCount(0);
    }

    [Fact]
    public void ShouldInvokeDeleteEvent()
    {
        var skill = new SkillBuilder().Build();
        var wasInvoked = false;
        var cut = RenderComponent<SkillTag>(p => p
            .Add(
            s => s.Skill, skill)
            .Add(s => s.IsAuthenticated, true)
            .Add(s => s.DeleteSkill, () => wasInvoked = true));

        cut.Find("button").Click();

        wasInvoked.Should().BeTrue();
    }
}