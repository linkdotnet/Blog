namespace LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

public sealed record AuthInformation
{
    private string logoutUrl = string.Empty;

    public const string AuthInformationSection = "Authentication";

    public required string Provider { get; set; }

    public required string Domain { get; init; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public string LogoutUri
    {
        get => !string.IsNullOrEmpty(logoutUrl) ? logoutUrl : $"https://{Domain}/v2/logout?client_id={ClientId}";
        set => logoutUrl = value;
    }
}
