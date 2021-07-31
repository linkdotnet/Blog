using Blazored.Toast.Services;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared.Skills;
using LinkDotNet.Domain;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared.Skills
{
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
                string.Empty));
        }
    }
}