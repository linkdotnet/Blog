using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.AboutMe.Components.Skill;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe.Components;

public class AddSkillDialogTests : BunitContext
{
    [Fact]
    public void ShouldCreateSkill()
    {
        Skill addedSkill = null;
        var toastServiceMock = Substitute.For<IToastService>();
        Services.AddScoped(_ => toastServiceMock);
        var cut = Render<AddSkillDialog>(p =>
            p.Add(s => s.SkillAdded, t => addedSkill = t));
        cut.Find("#title").Change("C#");
        cut.Find("#image").Change("Url");
        cut.Find("#capability").Change("capability");
        cut.Find("#proficiency").Change(ProficiencyLevel.Expert.Key);

        cut.Find("form").Submit();

        addedSkill.Should().NotBeNull();
        addedSkill.Name.Should().Be("C#");
        addedSkill.IconUrl.Should().Be("Url");
        addedSkill.Capability.Should().Be("capability");
        addedSkill.ProficiencyLevel.Should().Be(ProficiencyLevel.Expert);
        toastServiceMock.Received(1).ShowSuccess(
            "Created Skill C# in capability capability with level Expert");
    }
}