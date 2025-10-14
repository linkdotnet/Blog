using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Blazored.Toast.Services;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.AboutMe.Components.Skill;
using Microsoft.Extensions.DependencyInjection;
using TestContext = Xunit.TestContext;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared.Skills;

public class SkillTableTests : SqlDatabaseTestBase<Skill>
{
    [Fact]
    public async Task ShouldDeleteItem()
    {
        var skill = new SkillBuilder().WithSkillName("C#").Build();
        using var ctx = new BunitContext();
        await Repository.StoreAsync(skill);
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        var cut = ctx.Render<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, true));

        await cut.WaitForComponent<SkillTag>().Find("button").ClickAsync();

        var items = await Repository.GetAllAsync();
        items.ShouldBeEmpty();
        cut.FindAll("td").Any(s => s.TextContent == "C#").ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldAddSkill()
    {
        using var ctx = new BunitContext();
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        var cut = ctx.Render<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, true));
        await cut.Find("button").ClickAsync();
        var dialog = cut.FindComponent<AddSkillDialog>();
        dialog.Find("#title").Change("C#");
        dialog.Find("#image").Change("Url");
        dialog.Find("#capability").Change("capability");
        dialog.Find("#proficiency").Change(ProficiencyLevel.Expert.Key);

        await dialog.Find("form").SubmitAsync();

        var skillTag = cut.WaitForComponent<SkillTag>();
        skillTag.Find("span").Text().ShouldContain("C#");
        var fromRepo = (await Repository.GetAllAsync())[0];
        fromRepo.Name.ShouldBe("C#");
        fromRepo.IconUrl.ShouldBe("Url");
        fromRepo.Capability.ShouldBe("capability");
        fromRepo.ProficiencyLevel.ShouldBe(ProficiencyLevel.Expert);
    }

    [Fact]
    public async Task ShouldNotAllowToEditSkillTagsWhenNotAdmin()
    {
        using var ctx = new BunitContext();
        var skill = new SkillBuilder().Build();
        await Repository.StoreAsync(skill);
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());

        var cut = ctx.Render<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, false));

        cut.WaitForComponent<SkillTag>().Instance.ShowAdminActions.ShouldBeFalse();
    }

    [Fact]
    public async Task ShouldUpdateProficiencyWhenSkillTagDragged()
    {
        using var ctx = new BunitContext();
        var skill = new SkillBuilder().WithProficiencyLevel(ProficiencyLevel.Familiar).Build();
        await DbContext.AddAsync(skill, TestContext.Current.CancellationToken);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        var cut = ctx.Render<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, true));
        cut.WaitForElement(".skill-tag");

        await cut.FindAll(".skill-tag")[0].DragAsync();
        await cut.FindAll(".proficiency-level")[1].DropAsync();

        var skillFromDb = await Repository.GetByIdAsync(skill.Id);
        skillFromDb.ShouldNotBeNull();
        skillFromDb.ProficiencyLevel.ShouldBe(ProficiencyLevel.Proficient);
    }

    [Fact]
    public async Task ShouldStayOnSameProficiencyWhenDroppedOnSameProficiencyLevel()
    {
        using var ctx = new BunitContext();
        var skill = new SkillBuilder().WithProficiencyLevel(ProficiencyLevel.Familiar).Build();
        await DbContext.AddAsync(skill, TestContext.Current.CancellationToken);
        await DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        ctx.Services.AddScoped(_ => Repository);
        ctx.Services.AddScoped(_ => Substitute.For<IToastService>());
        var cut = ctx.Render<SkillTable>(p =>
            p.Add(s => s.ShowAdminActions, true));
        cut.WaitForElement(".skill-tag");

        await cut.FindAll(".skill-tag")[0].DragAsync();
        await cut.FindAll(".proficiency-level")[0].DropAsync();

        var skillFromDb = await Repository.GetByIdAsync(skill.Id);
        skillFromDb.ShouldNotBeNull();
        skillFromDb.ProficiencyLevel.ShouldBe(ProficiencyLevel.Familiar);
    }
}