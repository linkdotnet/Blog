namespace LinkDotNet.Blog.UpgradeAssistant;

public static class MigrationDiscovery
{
    public static IReadOnlyList<IMigration> DiscoverAll()
    {
        return typeof(IMigration).Assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                     && typeof(IMigration).IsAssignableFrom(t))
            .Select(t => (IMigration)Activator.CreateInstance(t)!)
            .ToList();
    }
}
