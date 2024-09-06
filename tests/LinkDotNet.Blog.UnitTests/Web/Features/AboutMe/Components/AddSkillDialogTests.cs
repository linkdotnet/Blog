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

        addedSkill.ShouldNotBeNull();
        addedSkill.Name.ShouldBe("C#");
        addedSkill.IconUrl.ShouldBe("Url");
        addedSkill.Capability.ShouldBe("capability");
        addedSkill.ProficiencyLevel.ShouldBe(ProficiencyLevel.Expert);
        toastServiceMock.Received(1).ShowSuccess(
            "Created Skill C# in capability capability with level Expert");
    }
}