using LinkDotNet.Blog.UpgradeAssistant.Migrations;

namespace LinkDotNet.Blog.UpgradeAssistant;

public static class MigrationDiscovery
{
    public static IReadOnlyList<IMigration> DiscoverAll()
    {
        return new IMigration[]
            {
                new Migration11To12()
            }
            .OrderBy(m => Version.TryParse(m.FromVersion, out var v) ? v : new Version(0, 0))
            .ThenBy(m => Version.TryParse(m.ToVersion, out var v) ? v : new Version(0, 0))
            .ToList();
    }
}
