using System.Linq;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.AboutMe.Components.Skill;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe.Components;

public class SkillTagTests : BunitContext
{
    [Fact]
    public void ShouldRenderImageAndText()
    {
        var skill = new SkillBuilder().WithSkillName("C#").WithIconUrl("test").Build();

        var cut = Render<SkillTag>(p => p.Add(
            s => s.Skill, skill));

        cut.Find("span").TextContent.ShouldContain("C#");
        cut.Find("img").Attributes.Single(a => a.Name == "src").Value.ShouldBe("test");
        cut.FindAll("button").ShouldBeEmpty();
    }

    [Fact]
    public void ShouldNotRenderImageWhenNotAvailable()
    {
        var skill = new SkillBuilder().WithIconUrl(null).Build();

        var cut = Render<SkillTag>(p => p.Add(
            s => s.Skill, skill));

        cut.FindAll("img").ShouldBeEmpty();
    }

    [Fact]
    public void ShouldInvokeDeleteEvent()
    {
        var skill = new SkillBuilder().Build();
        var wasInvoked = false;
        var cut = Render<SkillTag>(p => p
            .Add(
            s => s.Skill, skill)
            .Add(s => s.ShowAdminActions, true)
            .Add(s => s.DeleteSkill, () => wasInvoked = true));

        cut.Find("button").Click();

        wasInvoked.ShouldBeTrue();
    }
}