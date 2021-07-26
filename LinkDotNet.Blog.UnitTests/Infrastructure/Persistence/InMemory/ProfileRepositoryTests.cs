using System.Threading.Tasks;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Infrastructure.Persistence.InMemory;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Infrastructure.Persistence.InMemory
{
    public class ProfileRepositoryTests
    {
        private readonly ProfileRepository profileRepository;

        public ProfileRepositoryTests()
        {
            profileRepository = new ProfileRepository();
        }

        [Fact]
        public async Task ShouldSaveAndRetrieveAllEntries()
        {
            var item1 = new ProfileInformationEntryBuilder().WithKey("key1").WithValue("value1").Build();
            var item2 = new ProfileInformationEntryBuilder().WithKey("key2").WithValue("value2").Build();
            await profileRepository.AddAsync(item1);
            await profileRepository.AddAsync(item2);

            var items = await profileRepository.GetAllAsync();

            items[0].Key.Should().Be("key1");
            items[0].Value.Should().Be("value1");
            items[1].Key.Should().Be("key2");
            items[1].Value.Should().Be("value2");
        }

        [Fact]
        public async Task ShouldDelete()
        {
            var item1 = new ProfileInformationEntryBuilder().WithKey("key1").WithValue("value1").Build();
            var item2 = new ProfileInformationEntryBuilder().WithKey("key2").WithValue("value2").Build();
            await profileRepository.AddAsync(item1);
            await profileRepository.AddAsync(item2);

            await profileRepository.DeleteAsync(item1.Id);

            var items = await profileRepository.GetAllAsync();
            items.Should().HaveCount(1);
            items[0].Id.Should().Be(item2.Id);
        }

        [Fact]
        public async Task NoopOnDeleteWhenEntryNotFound()
        {
            var item = new ProfileInformationEntryBuilder().WithKey("key1").WithValue("value1").Build();
            await profileRepository.AddAsync(item);

            await profileRepository.DeleteAsync("SomeIdWhichHopefullyDoesNotExist");

            (await profileRepository.GetAllAsync()).Should().HaveCount(1);
        }
    }
}