using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LinkDotNet.Blog.UnitTests.Web;

public partial class BootstrapIconsSubsetTests
{
    private static readonly string[] SourceExtensions = [".razor", ".cs", ".js", ".css", ".html"];

    [Fact]
    public void SubsetShouldContainEveryIconUsedInSource()
    {
        var webProject = Path.GetFullPath(Path.Combine(ThisDirectory(), "../../../src/LinkDotNet.Blog.Web"));
        var subsetCss = Path.Combine(webProject, "wwwroot/css/bootstrap-icons.min.css");
        var available = File.ReadAllText(subsetCss);

        var missing = Directory.EnumerateFiles(webProject, "*", SearchOption.AllDirectories)
            .Where(f => SourceExtensions.Contains(Path.GetExtension(f)) && f != subsetCss)
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}")
                        && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}"))
            .SelectMany(f => IconRegex().Matches(File.ReadAllText(f)).Select(m => m.Value))
            .Distinct()
            .Where(icon => !available.Contains($".{icon}::before", System.StringComparison.Ordinal))
            .Order()
            .ToList();

        missing.ShouldBeEmpty(
            "bootstrap-icons font and CSS are subset to the icons in use. Regenerate both from the full " +
            "Bootstrap Icons release with `pyftsubset bootstrap-icons.woff2 --unicodes=<codepoints> --flavor=woff2` " +
            "and add the matching `.bi-*::before` rules.");
    }

    private static string ThisDirectory([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;

    [GeneratedRegex(@"\bbi-[a-z0-9]+(-[a-z0-9]+)*")]
    private static partial Regex IconRegex();
}
