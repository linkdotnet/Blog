using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web;
using LinkDotNet.Blog.Web.Features.AboutMe.Components;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared;

public class ProfileTests : TestContext
{
    [Fact]
    public void ShouldRenderAllItemsSortedByOrder()
    {
        var entry1 = new ProfileInformationEntryBuilder().WithContent("key 1").WithSortOrder(1).Build();
        var entry2 = new ProfileInformationEntryBuilder().WithContent("key 2").WithSortOrder(2).Build();
        var (repoMock, _) = RegisterServices();
        SetupGetAll(repoMock, entry1, entry2);
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
        RenderComponent<Profile>(p => p.Add(s => s.ShowAdminActions, true))
            .FindComponents<AddProfileShortItem>().Should().HaveCount(1);
    }

    [Fact]
    public void ShouldAddEntry()
    {
        var (repo, _) = RegisterServices();
        ProfileInformationEntry entryToDb = null;
        repo.Setup(p => p.StoreAsync(It.IsAny<ProfileInformationEntry>()))
            .Callback<ProfileInformationEntry>(p => entryToDb = p);
        var cut = RenderComponent<Profile>(p => p.Add(s => s.ShowAdminActions, true));
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
        SetupGetAll(repoMock, entryToDelete);
        var cut = RenderComponent<Profile>(p => p.Add(s => s.ShowAdminActions, true));
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
        SetupGetAll(repoMock, entryToDelete);
        var cut = RenderComponent<Profile>(p => p.Add(s => s.ShowAdminActions, true));
        cut.Find(".profile-keypoints li button").Click();

        cut.FindComponent<ConfirmDialog>().Find("#cancel").Click();

        repoMock.Verify(r => r.DeleteAsync("SomeId"), Times.Never);
    }

    [Fact]
    public void ShouldAddEntryWithCorrectSortOrder()
    {
        var (repo, _) = RegisterServices();
        var entry = new ProfileInformationEntryBuilder().WithSortOrder(1).Build();
        SetupGetAll(repo, entry);
        ProfileInformationEntry entryToDb = null;
        repo.Setup(p => p.StoreAsync(It.IsAny<ProfileInformationEntry>()))
            .Callback<ProfileInformationEntry>(p => entryToDb = p);
        var cut = RenderComponent<Profile>(p => p.Add(s => s.ShowAdminActions, true));
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
        var target = new ProfileInformationEntryBuilder().WithSortOrder(100).Build();
        var source = new ProfileInformationEntryBuilder().WithSortOrder(200).Build();
        var (repo, calculator) = RegisterServices();
        var profileInformationEntries = new List<ProfileInformationEntry> { target, source };
        SetupGetAll(repo, target, source);
        ProfileInformationEntry entryToDb = null;
        repo.Setup(p => p.StoreAsync(It.IsAny<ProfileInformationEntry>()))
            .Callback<ProfileInformationEntry>(p => entryToDb = p);
        var cut = RenderComponent<Profile>(p => p.Add(s => s.ShowAdminActions, true));
        calculator.Setup(s => s.GetSortOrder(target, profileInformationEntries)).Returns(150);

        cut.FindAll("li")[1].Drag();
        cut.FindAll("li")[0].Drop();

        source.SortOrder.Should().Be(150);
        entryToDb.Should().Be(source);
    }

    [Fact]
    public void ShouldNotChangeSortOrderWhenDroppedOnItself()
    {
        var source = new ProfileInformationEntryBuilder().WithSortOrder(200).Build();
        var (repo, _) = RegisterServices();
        SetupGetAll(repo, source);
        var cut = RenderComponent<Profile>(p => p.Add(s => s.ShowAdminActions, true));

        cut.FindAll("li")[0].Drag();
        cut.FindAll("li")[0].Drop();

        source.SortOrder.Should().Be(200);
    }

    private static AppConfiguration CreateEmptyConfiguration()
    {
        return new()
        {
            ProfileInformation = new ProfileInformation(),
        };
    }

    private static void SetupGetAll(
        Mock<IRepository<ProfileInformationEntry>> repoMock, params
            ProfileInformationEntry[] entries)
    {
        repoMock.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<ProfileInformationEntry, bool>>>(),
                It.IsAny<Expression<Func<ProfileInformationEntry, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .ReturnsAsync(new PaginatedList<ProfileInformationEntry>(entries, 1, 100));
    }

    private (Mock<IRepository<ProfileInformationEntry>> repoMock, Mock<ISortOrderCalculator> calcMock) RegisterServices()
    {
        var repoMock = new Mock<IRepository<ProfileInformationEntry>>();
        var calcMock = new Mock<ISortOrderCalculator>();
        Services.AddScoped(_ => CreateEmptyConfiguration());
        Services.AddScoped(_ => repoMock.Object);
        Services.AddScoped(_ => calcMock.Object);
        repoMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProfileInformationEntry, bool>>>(),
            It.IsAny<Expression<Func<ProfileInformationEntry, object>>>(),
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<int>())).ReturnsAsync(PaginatedList<ProfileInformationEntry>.Empty);
        return (repoMock, calcMock);
    }
}
