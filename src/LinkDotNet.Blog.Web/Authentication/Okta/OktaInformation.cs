namespace LinkDotNet.Blog.Web.Authentication.Okta;

public record OktaInformation
{
    public string Domain { get; init; }

    public string ClientId { get; init; }

    public string ClientSecret { get; init; }

    public string AuthorizationServerId { get; init; } = "default";
}