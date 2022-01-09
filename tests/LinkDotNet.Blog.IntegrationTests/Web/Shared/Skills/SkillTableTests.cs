using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using Blazored.Toast.Services;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared.Skills;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared.Skills;

public class SkillTableTests : SqlDatabaseTestBase<Skill>
{
    [Fact]
    public async Task ShouldDeleteItem()
    {
        var skill = new SkillBuilder().WithSkillName("C#").Build();
        using var ctx = new TestContext();
        await Repository.StoreAsync(skill);
        ctx.Services.AddScoped<IRepository<Skill>>(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());
        var cut = ctx.RenderComponent<SkillTable>(p =>
            p.Add(s => s.IsAuthenticated, true));
        cut.WaitForState(() => cut.HasComponent<SkillTag>());

        cut.FindComponent<SkillTag>().Find("button").Click();

        var items = (await Repository.GetAllAsync()).ToList();
        items.Should().HaveCount(0);
        cut.FindAll("td").Any(s => s.TextContent == "C#").Should().BeFalse();
    }

    [Fact]
    public async Task ShouldAddSkill()
    {
        using var ctx = new TestContext();
        ctx.Services.AddScoped<IRepository<Skill>>(_ => Repository);
        ctx.Services.AddScoped(_ => Mock.Of<IToastService>());
        var cut = ctx.RenderComponent<SkillTable>(p =>
            p.Add(s => s.IsAuthenticated, true));
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
}