using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly AuthenticationStateProvider authenticationStateProvider;

    public CurrentUserService(AuthenticationStateProvider authenticationStateProvider)
        => this.authenticationStateProvider = authenticationStateProvider;

    public async ValueTask<string?> GetDisplayNameAsync()
    {
        var user = (await authenticationStateProvider.GetAuthenticationStateAsync()).User;
        if (user?.Identity is not { IsAuthenticated: true })
        {
            return null;
        }

        var name = user.FindFirst("Name")?.Value
                   ?? user.FindFirst("preferred_username")?.Value
                   ?? user.FindFirst("nickname")?.Value;

        return string.IsNullOrWhiteSpace(name) ? null : name;
    }
}
