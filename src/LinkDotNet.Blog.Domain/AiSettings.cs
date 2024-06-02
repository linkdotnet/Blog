namespace LinkDotNet.Blog.Domain;

public class AiSettings
{
    public const string AiSettingsSection = "AI";

    public string DeploymentName { get; set; }

    public string EndpointUrl { get; set; }

    public string ModelId { get; set; }

    public string ApiKey { get; set; }

    public bool IsEnabled => !string.IsNullOrEmpty(DeploymentName)
                             && !string.IsNullOrEmpty(EndpointUrl)
                             && !string.IsNullOrEmpty(ModelId)
                             && !string.IsNullOrEmpty(ApiKey);
}
