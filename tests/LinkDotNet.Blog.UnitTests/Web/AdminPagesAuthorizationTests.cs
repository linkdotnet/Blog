using System.Linq;
using System.Reflection;
using LinkDotNet.Blog.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace LinkDotNet.Blog.UnitTests.Web;

public class AdminPagesAuthorizationTests
{
    [Fact]
    public void AllAdminPagesShouldRequireAuthorization()
    {
        var unprotected = typeof(App).Assembly.GetTypes()
            .Where(t => t.Namespace?.StartsWith("LinkDotNet.Blog.Web.Features.Admin", System.StringComparison.Ordinal) == true)
            .Where(t => t.GetCustomAttributes<RouteAttribute>().Any())
            .Where(t => t.GetCustomAttribute<AuthorizeAttribute>() is null)
            .Select(t => t.FullName)
            .ToList();

        unprotected.ShouldBeEmpty();
    }
}
