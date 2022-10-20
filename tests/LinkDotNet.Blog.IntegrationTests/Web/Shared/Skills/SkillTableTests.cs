using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.AboutMe.Components.Skill;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared.Skills;

public class SkillTableTests : SqlDatabaseTestBase<Skill>
{
    [Fact]
    public async Task ShouldDeleteItem()
    {
        var skill = new SkillBuilder().WithSkillName("C#").Build();
        using var ctx = new TestContext();
        await Repository.StoreAsync(skill);
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());
        var cut = ctx.RenderComponent<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, true));
        cut.WaitForState(() => cut.HasComponent<SkillTag>());

        cut.FindComponent<SkillTag>().Find("button").Click();

        var items = await Repository.GetAllAsync();
        items.Should().HaveCount(0);
        cut.FindAll("td").Any(s => s.TextContent == "C#").Should().BeFalse();
    }

    [Fact]
    public async Task ShouldAddSkill()
    {
        using var ctx = new TestContext();
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());
        var cut = ctx.RenderComponent<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, true));
        cut.Find("button").Click();
        var dialog = cut.FindComponent<AddSkillDialog>();
        dialog.Find("#title").Change("C#");
        dialog.Find("#image").Change("Url");
        dialog.Find("#capability").Change("capability");
        dialog.Find("#proficiency").Change(ProficiencyLevel.Expert.Key);

        dialog.Find("form").Submit();

        cut.WaitForState(() => cut.HasComponent<SkillTag>());
        var skillTag = cut.FindComponent<SkillTag>();
        skillTag.Find("span").Text().Should().Contain("C#");
        var fromRepo = (await Repository.GetAllAsync())[0];
        fromRepo.Name.Should().Be("C#");
        fromRepo.IconUrl.Should().Be("Url");
        fromRepo.Capability.Should().Be("capability");
        fromRepo.ProficiencyLevel.Should().Be(ProficiencyLevel.Expert);
    }

    [Fact]
    public async Task ShouldNotAllowToEditSkillTagsWhenNotAdmin()
    {
        using var ctx = new TestContext();
        var skill = new SkillBuilder().Build();
        await Repository.StoreAsync(skill);
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());

        var cut = ctx.RenderComponent<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, false));

        cut.WaitForState(() => cut.FindComponents<SkillTag>().Any());
        cut.FindComponent<SkillTag>().Instance.ShowAdminActions.Should().BeFalse();
    }

    [Fact]
    public async Task ShouldUpdateProficiencyWhenSkillTagDragged()
    {
        using var ctx = new TestContext();
        var skill = new SkillBuilder().WithProficiencyLevel(ProficiencyLevel.Familiar).Build();
        await DbContext.AddAsync(skill);
        await DbContext.SaveChangesAsync();
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());
        var cut = ctx.RenderComponent<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, true));
        cut.WaitForState(() => cut.FindAll(".skill-tag").Any());

        cut.FindAll(".skill-tag")[0].Drag();
        cut.FindAll(".proficiency-level")[1].Drop();

        var skillFromDb = await Repository.GetByIdAsync(skill.Id);
        skillFromDb.ProficiencyLevel.Should().Be(ProficiencyLevel.Proficient);
    }

    [Fact]
    public async Task ShouldStayOnSameProficiencyWhenDroppedOnSameProficiencyLevel()
    {
        using var ctx = new TestContext();
        var skill = new SkillBuilder().WithProficiencyLevel(ProficiencyLevel.Familiar).Build();
        await DbContext.AddAsync(skill);
        await DbContext.SaveChangesAsync();
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());
        var cut = ctx.RenderComponent<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, true));
        cut.WaitForState(() => cut.FindAll(".skill-tag").Any());

        cut.FindAll(".skill-tag")[0].Drag();
        cut.FindAll(".proficiency-level")[0].Drop();

        var skillFromDb = await Repository.GetByIdAsync(skill.Id);
        skillFromDb.ProficiencyLevel.Should().Be(ProficiencyLevel.Familiar);
    }
}