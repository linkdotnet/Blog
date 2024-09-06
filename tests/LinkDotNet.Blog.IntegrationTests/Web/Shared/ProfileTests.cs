using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features.AboutMe.Components;
using LinkDotNet.Blog.Web.Features.Components;
using LinkDotNet.Blog.Web.Features.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkDotNet.Blog.IntegrationTests.Web.Shared;

public class ProfileTests : BunitContext
{
    [Fact]
    public void ShouldRenderAllItemsSortedByOrder()
    {
        var entry1 = new ProfileInformationEntryBuilder().WithContent("key 1").WithSortOrder(1).Build();
        var entry2 = new ProfileInformationEntryBuilder().WithContent("key 2").WithSortOrder(2).Build();
        var (repoMock, _) = RegisterServices();
        SetupGetAll(repoMock, entry1, entry2);
        var cut = RenderProfileWithEmptyInformation();

        var items = cut.FindAll(".profile-keypoints li");

        items.Count.ShouldBe(2);
        items[0].TextContent.ShouldContain("key 1");
        items[1].TextContent.ShouldContain("key 2");
    }

    [Fact]
    public void ShouldNotShowAdminActions()
    {
        RegisterServices();
        RenderProfileWithEmptyInformation().FindComponents<AddProfileShortItem>().ShouldBeEmpty();
    }

    [Fact]
    public void ShouldShowAdminActionsWhenLoggedIn()
    {
        RegisterServices();
        RenderProfileInAdmin().FindComponents<AddProfileShortItem>().ShouldHaveSingleItem();
    }

    [Fact]
    public void ShouldAddEntry()
    {
        var (repo, _) = RegisterServices();
        ProfileInformationEntry? entryToDb = null;
        repo.When(r => r.StoreAsync(Arg.Any<ProfileInformationEntry>()))
            .Do(call => entryToDb = call.Arg<ProfileInformationEntry>());
        var cut = RenderProfileInAdmin();
        var addShortItemComponent = cut.FindComponent<AddProfileShortItem>();
        addShortItemComponent.Find("input").Change("key");

        addShortItemComponent.Find("button").Click();

        entryToDb.ShouldNotBeNull();
        entryToDb.Content.ShouldBe("key");
        entryToDb.SortOrder.ShouldBe(1000);
    }

    [Fact]
    public void ShouldDeleteEntryWhenConfirmed()
    {
        var entryToDelete = new ProfileInformationEntryBuilder().WithContent("key 2").Build();
        entryToDelete.Id = "SomeId";
        var (repoMock, _) = RegisterServices();
        SetupGetAll(repoMock, entryToDelete);
        var cut = RenderProfileInAdmin();
        cut.Find(".profile-keypoints li button").Click();

        cut.FindComponent<ConfirmDialog>().Find("#ok").Click();

        repoMock.Received(1).DeleteAsync("SomeId");

    }

    [Fact]
    public void ShouldNotDeleteEntryWhenNotConfirmed()
    {
        var entryToDelete = new ProfileInformationEntryBuilder().WithContent("key 2").Build();
        entryToDelete.Id = "SomeId";
        var (repoMock, _) = RegisterServices();
        SetupGetAll(repoMock, entryToDelete);
        var cut = RenderProfileInAdmin();
        cut.Find(".profile-keypoints li button").Click();

        cut.FindComponent<ConfirmDialog>().Find("#cancel").Click();

        repoMock.Received(0).DeleteAsync("SomeId");

    }

    [Fact]
    public void ShouldAddEntryWithCorrectSortOrder()
    {
        var (repo, _) = RegisterServices();
        var entry = new ProfileInformationEntryBuilder().WithSortOrder(1).Build();
        SetupGetAll(repo, entry);
        ProfileInformationEntry? entryToDb = null;
        repo.When(r => r.StoreAsync(Arg.Any<ProfileInformationEntry>()))
            .Do(call => entryToDb = call.Arg<ProfileInformationEntry>());
        var cut = RenderProfileInAdmin();
        var addShortItemComponent = cut.FindComponent<AddProfileShortItem>();
        addShortItemComponent.Find("input").Change("key");

        addShortItemComponent.Find("button").Click();

        entryToDb.ShouldNotBeNull();
        entryToDb.Content.ShouldBe("key");
        entryToDb.SortOrder.ShouldBe(1001);
    }

    [Fact]
    public void ShouldSetNewOrderWhenItemDragAndDropped()
    {
        var target = new ProfileInformationEntryBuilder().WithSortOrder(100).Build();
        var source = new ProfileInformationEntryBuilder().WithSortOrder(200).Build();
        var (repo, calculator) = RegisterServices();
        SetupGetAll(repo, target, source);
        ProfileInformationEntry? entryToDb = null;
        repo.When(r => r.StoreAsync(Arg.Any<ProfileInformationEntry>()))
            .Do(call => entryToDb = call.Arg<ProfileInformationEntry>());
        var cut = RenderProfileInAdmin();
        calculator.GetSortOrder(target, Arg.Any<IEnumerable<ProfileInformationEntry>>()).Returns(150);

        cut.FindAll("li")[1].Drag();
        cut.FindAll("li")[0].Drop();

        source.SortOrder.ShouldBe(150);
        entryToDb.ShouldBe(source);
    }

    [Fact]
    public void ShouldNotChangeSortOrderWhenDroppedOnItself()
    {
        var source = new ProfileInformationEntryBuilder().WithSortOrder(200).Build();
        var (repo, _) = RegisterServices();
        SetupGetAll(repo, source);
        var cut = RenderProfileInAdmin();

        cut.FindAll("li")[0].Drag();
        cut.FindAll("li")[0].Drop();

        source.SortOrder.ShouldBe(200);
    }

    private static void SetupGetAll(
        IRepository<ProfileInformationEntry> repoMock,
        params ProfileInformationEntry[] entries)
    {
        repoMock.GetAllAsync(
                Arg.Any<Expression<Func<ProfileInformationEntry, bool>>>(),
                Arg.Any<Expression<Func<ProfileInformationEntry, object>>>(),
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Any<int>())
            .Returns(new PagedList<ProfileInformationEntry>(entries, entries.Length, 1, 100));
    }

    private (IRepository<ProfileInformationEntry> repoMock, ISortOrderCalculator calcMock) RegisterServices()
    {
        var repoMock = Substitute.For<IRepository<ProfileInformationEntry>>();
        var calcMock = Substitute.For<ISortOrderCalculator>();
        Services.AddScoped(_ => repoMock);
        Services.AddScoped(_ => calcMock);
        repoMock.GetAllAsync(
                Arg.Any<Expression<Func<ProfileInformationEntry, bool>>>(),
                Arg.Any<Expression<Func<ProfileInformationEntry, object>>>(),
                Arg.Any<bool>(),
                Arg.Any<int>(),
                Arg.Any<int>())
            .Returns(PagedList<ProfileInformationEntry>.Empty);
        return (repoMock, calcMock);
    }

    private RenderedComponent<Profile> RenderProfileWithEmptyInformation()
        => Render<Profile>(p => p.Add(s => s.ProfileInformation, new ProfileInformationBuilder().Build()));

    private RenderedComponent<Profile> RenderProfileInAdmin()
        => Render<Profile>(p => p
            .Add(s => s.ProfileInformation, new ProfileInformationBuilder().Build())
            .Add(s => s.ShowAdminActions, true));
}
