namespace LinkDotNet.Blog.Web
{
    public record AppConfiguration
    {
        public string LinkedinAccountUrl { get; init; }
        public bool HasLinkedinAccount => !string.IsNullOrEmpty(LinkedinAccountUrl);
        
        public string GithubAccountUrl { get; init; }
        public bool HasGithubAccount => !string.IsNullOrEmpty(LinkedinAccountUrl);
    }
}