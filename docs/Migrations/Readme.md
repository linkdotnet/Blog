# Upgrading from an older version

The blog tries to follow “semantic versioning” as much as possible. This means that when a release comes out with a new “major” version, there are breaking changes. What is meant by “breaking changes”? This is either the database (i.e. the database schema has changed) or the “appsettings.json” file has breaking changes. The C# code is excluded. This can change completely at any time. Interfaces and classes that are used internally do not adhere to semantic versioning. Breaking changes also means that migration of some kind is mandatory. For example, because areas in appsettings.json have been completely restructured or because a new database table has been added.

This is contrasted by Minor changes. These are things where the user does not necessarily have to intervene. In appsettings.json, for example, these could be new features that are simply not automatically switched on or off. This means that these changes are optional.

Breaking changes are recorded in the [MIGRATION.md](../../MIGRATION.md). Since version 9 of the blog, “Entity Framework Migrations” has been introduced for all SQL providers. You can read more in the [documentation](../Storage/Readme.md). In a nutshell, this means that database migration can be carried out easily via the “ef migration” CLI tool. More on this in the documentation linked above.

Changes for the appsettings.json must currently still be made manually. The exact changes that need to be made here can be found in MIGRATION.md.

## Automated Configuration Migration (Version 12+)

Starting with **version 12**, the blog includes an **Automated Upgrade Assistant** that handles appsettings.json migrations automatically. This tool eliminates the need for manual configuration changes in most cases.

### What the Upgrade Assistant Does

- **Automatic Detection** - Detects your current configuration version
- **Sequential Migration** - Applies necessary migrations step-by-step
- **Safe Backups** - Creates timestamped backups before making any changes
- **Smart Filtering** - Skips version-controlled `appsettings.json`, only migrates environment-specific files
- **Colorful Output** - Provides clear, color-coded feedback about changes and warnings

### How to Use

Run the Upgrade Assistant from your blog directory:

```bash
# Preview changes without applying them
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --dry-run

# Migrate all environment-specific appsettings files
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant

# Migrate a specific file
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --path appsettings.Production.json
```

For detailed documentation on the Upgrade Assistant, see [UpgradeAssistant.md](./UpgradeAssistant.md).