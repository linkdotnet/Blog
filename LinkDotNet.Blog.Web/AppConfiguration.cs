using LinkDotNet.Domain;

namespace LinkDotNet.Blog.Web
{
    public record AppConfiguration
    {
        public string BlogName { get; init; }

        public string LinkedinAccountUrl { get; init; }

        public bool HasLinkedinAccount => !string.IsNullOrEmpty(LinkedinAccountUrl);

        public string GithubAccountUrl { get; init; }

        public bool HasGithubAccount => !string.IsNullOrEmpty(LinkedinAccountUrl);

        public Introduction Introduction { get; init; }

        public string ConnectionString { get; init; }

        public string DatabaseName { get; init; }
    }
}