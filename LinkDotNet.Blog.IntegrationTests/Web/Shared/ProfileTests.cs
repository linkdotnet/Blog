using System.Collections.Generic;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Shared;
using LinkDotNet.Domain;
using LinkDotNet.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared
{
    public class ProfileTests : TestContext
    {
        [Fact]
        public void ShouldRenderAllItemsSortedByOrder()
        {
            var entry1 = new ProfileInformationEntryBuilder().WithContent("key 1").WithSortOrder(1).Build();
            var entry2 = new ProfileInformationEntryBuilder().WithContent("key 2").WithSortOrder(2).Build();
            var (repoMock, _) = RegisterServices();
            repoMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<ProfileInformationEntry> { entry1, entry2 });
            var cut = RenderComponent<Profile>();

            var items = cut.FindAll(".profile-keypoints li");

            items.Should().HaveCount(2);
            items[0].TextContent.Should().Contain("key 1");
            items[1].TextContent.Should().Contain("key 2");
        }

        [Fact]
        public void ShouldNotShowAdminActions()
        {
            RegisterServices();
            RenderComponent<Profile>().FindComponents<AddProfileShortItem>().Should().HaveCount(0);
        }

        [Fact]
        public void ShouldShowAdminActionsWhenLoggedIn()
        {
            RegisterServices();
            RenderComponent<Profile>(p => p.Add(s => s.IsAuthenticated, true))
                .FindComponents<AddProfileShortItem>().Should().HaveCount(1);
        }

        [Fact]
        public void ShouldAddEntry()
        {
            var (repo, _) = RegisterServices();
            ProfileInformationEntry entryToDb = null;
            repo.Setup(p => p.StoreAsync(It.IsAny<ProfileInformationEntry>()))
                .Callback<ProfileInformationEntry>(p => entryToDb = p);
            var cut = RenderComponent<Profile>(p => p.Add(s => s.IsAuthenticated, true));
            var addShortItemComponent = cut.FindComponent<AddProfileShortItem>();
            addShortItemComponent.Find("input").Change("key");

            addShortItemComponent.Find("button").Click();

            entryToDb.Should().NotBeNull();
            entryToDb.Content.Should().Be("key");
            entryToDb.SortOrder.Should().Be(1000);
        }

        [Fact]
        public void ShouldDeleteEntryWhenConfirmed()
        {
            var entryToDelete = new ProfileInformationEntryBuilder().WithContent("key 2").Build();
            entryToDelete.Id = "SomeId";
            var (repoMock, _) = RegisterServices();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { entryToDelete });
            var cut = RenderComponent<Profile>(p => p.Add(s => s.IsAuthenticated, true));
            cut.Find(".profile-keypoints li button").Click();

            cut.FindComponent<ConfirmDialog>().Find("#ok").Click();

            repoMock.Verify(r => r.DeleteAsync("SomeId"), Times.Once);
        }

        [Fact]
        public void ShouldNotDeleteEntryWhenNotConfirmed()
        {
            var entryToDelete = new ProfileInformationEntryBuilder().WithContent("key 2").Build();
            entryToDelete.Id = "SomeId";
            var (repoMock, _) = RegisterServices();
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new[] { entryToDelete });
            var cut = RenderComponent<Profile>(p => p.Add(s => s.IsAuthenticated, true));
            cut.Find(".profile-keypoints li button").Click();

            cut.FindComponent<ConfirmDialog>().Find("#cancel").Click();

            repoMock.Verify(r => r.DeleteAsync("SomeId"), Times.Never);
        }

        [Fact]
        public void ShouldAddEntryWithCorrectSortOrder()
        {
            var (repo, _) = RegisterServices();
            var entry = new ProfileInformationEntryBuilder().WithSortOrder(1).Build();
            repo.Setup(p => p.GetAllAsync()).ReturnsAsync(new[] { entry });
            ProfileInformationEntry entryToDb = null;
            repo.Setup(p => p.StoreAsync(It.IsAny<ProfileInformationEntry>()))
                .Callback<ProfileInformationEntry>(p => entryToDb = p);
            var cut = RenderComponent<Profile>(p => p.Add(s => s.IsAuthenticated, true));
            var addShortItemComponent = cut.FindComponent<AddProfileShortItem>();
            addShortItemComponent.Find("input").Change("key");

            addShortItemComponent.Find("button").Click();

            entryToDb.Should().NotBeNull();
            entryToDb.Content.Should().Be("key");
            entryToDb.SortOrder.Should().Be(1001);
        }

        [Fact]
        public void ShouldSetNewOrderWhenItemDragAndDropped()
        {

        }

        private static AppConfiguration CreateEmptyConfiguration()
        {
            return new()
            {
                ProfileInformation = new ProfileInformation(),
            };
        }

        private (Mock<IProfileRepository> repoMock, Mock<ISortOrderCalculator> calcMock) RegisterServices()
        {
            var repoMock = new Mock<IProfileRepository>();
            var calcMock = new Mock<ISortOrderCalculator>();
            Services.AddScoped(_ => CreateEmptyConfiguration());
            Services.AddScoped(_ => repoMock.Object);
            Services.AddScoped(_ => calcMock.Object);
            repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ProfileInformationEntry>());
            return (repoMock, calcMock);
        }
    }
}