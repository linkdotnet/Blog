namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence;

// CI excludes these by trait on runners that can't provide them (see .github/workflows/dotnet.yml).
internal static class TestTraits
{
    public const string Requires = "Requires";

    public const string Docker = "Docker";

    public const string X64 = "x64";
}
