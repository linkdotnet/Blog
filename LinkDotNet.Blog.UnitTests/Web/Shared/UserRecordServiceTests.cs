using System;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Bunit;
using Bunit.TestDoubles;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared
{
    public class UserRecordServiceTests : TestContext
    {
        private readonly Mock<IRepository<UserRecord>> repositoryMock;
        private readonly FakeNavigationManager fakeNavigationManager;
        private readonly FakeAuthenticationStateProvider fakeAuthenticationStateProvider;
        private readonly Mock<ILocalStorageService> localStorageService;
        private readonly UserRecordService sut;

        public UserRecordServiceTests()
        {
            repositoryMock = new Mock<IRepository<UserRecord>>();
            fakeNavigationManager = new FakeNavigationManager(this.Renderer);
            fakeAuthenticationStateProvider = new FakeAuthenticationStateProvider();
            localStorageService = new Mock<ILocalStorageService>();
            sut = new UserRecordService(
                repositoryMock.Object,
                fakeNavigationManager,
                fakeAuthenticationStateProvider,
                localStorageService.Object,
                new Mock<ILogger>().Object);
        }

        [Fact]
        public async Task ShouldStoreInformation()
        {
            const string Url = "http://localhost/subpart";
            fakeNavigationManager.NavigateTo(Url);
            UserRecord recordToDb = null;
            repositoryMock.Setup(r => r.StoreAsync(It.IsAny<UserRecord>()))
                .Callback<UserRecord>(u => recordToDb = u);

            await sut.StoreUserRecordAsync();

            recordToDb.Should().NotBeNull();
            recordToDb.UrlClicked.Should().Be("subpart");
            recordToDb.UserIdentifierHash.Should().NotBe(0);
            recordToDb.DateTimeUtcClicked.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task ShouldGetUserHashAgain()
        {
            UserRecord recordToDb = null;
            var guidForUser = Guid.NewGuid();
            localStorageService.Setup(l => l.ContainKeyAsync("user", default))
                .ReturnsAsync(true);
            localStorageService.Setup(l => l.GetItemAsync<Guid>("user", default))
                .ReturnsAsync(guidForUser);
            repositoryMock.Setup(r => r.StoreAsync(It.IsAny<UserRecord>()))
                .Callback<UserRecord>(u => recordToDb = u);

            await sut.StoreUserRecordAsync();

            recordToDb.Should().NotBeNull();
            var hashCode = guidForUser.GetHashCode();
            recordToDb.UserIdentifierHash.Should().Be(hashCode);
        }

        [Fact]
        public async Task ShouldNotStoreForAdmin()
        {
            fakeAuthenticationStateProvider.TriggerAuthenticationStateChanged("Steven");

            await sut.StoreUserRecordAsync();

            repositoryMock.Verify(r => r.StoreAsync(It.IsAny<UserRecord>()), Times.Never);
        }

        [Fact]
        public async Task ShouldNotThrowExceptionToOutsideWorld()
        {
            localStorageService.Setup(l => l.ContainKeyAsync("user", default)).Throws<Exception>();

            Func<Task> act = () => sut.StoreUserRecordAsync();

            await act.Should().NotThrowAsync<Exception>();
        }
    }
}