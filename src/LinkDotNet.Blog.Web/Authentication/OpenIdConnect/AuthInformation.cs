namespace LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

public sealed record AuthInformation
{
    public string Domain { get; init; }

    public string ClientId { get; init; }

    public string ClientSecret { get; init; }

    public string LogoutUri { get; set; }
}
