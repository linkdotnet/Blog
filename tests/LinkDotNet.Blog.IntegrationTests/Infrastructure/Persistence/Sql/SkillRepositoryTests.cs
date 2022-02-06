using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

public class SkillRepositoryTests : SqlDatabaseTestBase<Skill>
{
    [Fact]
    public async Task ShouldSaveAndRetrieveAllEntries()
    {
        var skill = new SkillBuilder()
            .WithSkillName("C#")
            .WithIconUrl("url")
            .WithCapability("Backend")
            .WithProficiencyLevel(ProficiencyLevel.Expert).Build();
        await Repository.StoreAsync(skill);

        var items = await Repository.GetAllAsync();

        items.Should().HaveCount(1);
        items[0].Name.Should().Be("C#");
        items[0].IconUrl.Should().Be("url");
        items[0].Capability.Should().Be("Backend");
        items[0].ProficiencyLevel.Should().Be(ProficiencyLevel.Expert);
    }

    [Fact]
    public async Task ShouldDelete()
    {
        var skill1 = new SkillBuilder().Build();
        var skill2 = new SkillBuilder().Build();
        await Repository.StoreAsync(skill1);
        await Repository.StoreAsync(skill2);

        await Repository.DeleteAsync(skill1.Id);

        var items = await Repository.GetAllAsync();
        items.Should().HaveCount(1);
        items[0].Id.Should().Be(skill2.Id);
    }

    [Fact]
    public async Task NoopOnDeleteWhenEntryNotFound()
    {
        var item = new SkillBuilder().Build();
        await Repository.StoreAsync(item);

        await Repository.DeleteAsync("SomeIdWhichHopefullyDoesNotExist");

        (await Repository.GetAllAsync()).Should().HaveCount(1);
    }
}
