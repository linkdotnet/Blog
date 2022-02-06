using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence.Sql;

public class ProfileRepositoryTests : SqlDatabaseTestBase<ProfileInformationEntry>
{
    [Fact]
    public async Task ShouldSaveAndRetrieveAllEntries()
    {
        var item1 = new ProfileInformationEntryBuilder().WithContent("key1").Build();
        var item2 = new ProfileInformationEntryBuilder().WithContent("key2").Build();
        await Repository.StoreAsync(item1);
        await Repository.StoreAsync(item2);

        var items = await Repository.GetAllAsync();

        items[0].Content.Should().Be("key1");
        items[1].Content.Should().Be("key2");
    }

    [Fact]
    public async Task ShouldDelete()
    {
        var item1 = new ProfileInformationEntryBuilder().WithContent("key1").Build();
        var item2 = new ProfileInformationEntryBuilder().WithContent("key2").Build();
        await Repository.StoreAsync(item1);
        await Repository.StoreAsync(item2);

        await Repository.DeleteAsync(item1.Id);

        var items = await Repository.GetAllAsync();
        items.Should().HaveCount(1);
        items[0].Id.Should().Be(item2.Id);
    }

    [Fact]
    public async Task NoopOnDeleteWhenEntryNotFound()
    {
        var item = new ProfileInformationEntryBuilder().WithContent("key1").Build();
        await Repository.StoreAsync(item);

        await Repository.DeleteAsync("SomeIdWhichHopefullyDoesNotExist");

        (await Repository.GetAllAsync()).Should().HaveCount(1);
    }
}
