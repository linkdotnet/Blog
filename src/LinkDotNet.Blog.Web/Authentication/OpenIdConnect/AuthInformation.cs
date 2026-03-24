namespace LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

public sealed record AuthInformation
{
    public const string AuthInformationSection = "Authentication";

    public required string Provider { get; set; }

    public required string Domain { get; init; }

    public required string ClientId { get; init; }

    public required string ClientSecret { get; init; }

    public string LogoutUri
    {
        get => !string.IsNullOrEmpty(field) ? field : $"https://{Domain}/v2/logout?client_id={ClientId}";
        set;
    } = string.Empty;
}
