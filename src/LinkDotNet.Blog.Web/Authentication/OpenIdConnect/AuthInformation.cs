namespace LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

public sealed record AuthInformation
{
    private string logoutUrl = string.Empty;

    public const string AuthInformationSection = "Authentication";

    public string Provider { get; set; }

    public string Domain { get; init; }

    public string ClientId { get; init; }

    public string ClientSecret { get; init; }

    public string LogoutUri
    {
        get => !string.IsNullOrEmpty(logoutUrl) ? logoutUrl : $"https://{Domain}/v2/logout?client_id={ClientId}";
        set => logoutUrl = value;
    }
}
