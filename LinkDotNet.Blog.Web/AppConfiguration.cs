using LinkDotNet.Blog.Web.Shared.Services;
using LinkDotNet.Domain;

namespace LinkDotNet.Blog.Web
{
    public record AppConfiguration
    {
        public string BlogName { get; init; }

        public string LinkedinAccountUrl { get; init; }

        public bool HasLinkedinAccount => !string.IsNullOrEmpty(LinkedinAccountUrl);

        public string GithubAccountUrl { get; init; }

        public bool HasGithubAccount => !string.IsNullOrEmpty(GithubAccountUrl);

        public Introduction Introduction { get; init; }

        public string ConnectionString { get; init; }

        public string DatabaseName { get; init; }

        public int BlogPostsPerPage { get; init; }

        public bool IsAboutMeEnabled => ProfileInformation != null;

        public ProfileInformation ProfileInformation { get; init; }

        public Giscus Giscus { get; init; }

        public bool IsGiscusEnabled => Giscus != null;
    }
}