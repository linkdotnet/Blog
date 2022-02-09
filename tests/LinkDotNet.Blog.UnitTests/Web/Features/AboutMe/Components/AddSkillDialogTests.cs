using Blazored.Toast.Services;
using Bunit;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Web.Features.AboutMe.Components;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.UnitTests.Web.Features.AboutMe.Components;

public class AddSkillDialogTests : TestContext
{
    [Fact]
    public void ShouldCreateSkill()
    {
        Skill addedSkill = null;
        var toastServiceMock = new Mock<IToastService>();
        Services.AddScoped(_ => toastServiceMock.Object);
        var cut = RenderComponent<AddSkillDialog>(p =>
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
        toastServiceMock.Verify(t => t.ShowSuccess(
            "Created Skill C# in capability capability with level Expert",
            string.Empty,
            null));
    }
}