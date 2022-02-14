using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Bunit;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features.Archive;
using LinkDotNet.Blog.Web.Features.Components;
using Microsoft.Extensions.DependencyInjection;
using X.PagedList;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Archive;

public class ArchivePageTests : TestContext
{
    [Fact]
    public void ShouldShowLoading()
    {
        var repository = new Mock<IRepository<BlogPost>>();
        Services.AddScoped(_ => repository.Object);
        repository.Setup(r => r.GetAllAsync(
                It.IsAny<Expression<Func<BlogPost, bool>>>(),
                It.IsAny<Expression<Func<BlogPost, object>>>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>()))
            .Returns(async () =>
            {
                await Task.Delay(250);
                return new PagedList<BlogPost>(Array.Empty<BlogPost>(), 1, 1);
            });

        var cut = RenderComponent<ArchivePage>();

        cut.FindComponents<Loading>().Count.Should().Be(1);
    }

    [Fact]
    public void ShouldSetOgData()
    {
        var repository = new Mock<IRepository<BlogPost>>();
        Services.AddScoped(_ => repository.Object);
        repository.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<BlogPost, bool>>>(),
            It.IsAny<Expression<Func<BlogPost, object>>>(),
            It.IsAny<bool>(),
            It.IsAny<int>(),
            It.IsAny<int>()))
            .ReturnsAsync(new PagedList<BlogPost>(Array.Empty<BlogPost>(), 1, 1));

        var cut = RenderComponent<ArchivePage>();

        var ogData = cut.FindComponent<OgData>().Instance;
        ogData.Title.Should().Contain("Archive");
    }
}