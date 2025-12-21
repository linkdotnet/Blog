# Blog Upgrade Assistant

A command-line tool to automatically migrate `appsettings.json` files to the latest configuration version for the LinkDotNet.Blog application.

## Features

- **Automatic migration** - Detects current configuration version and applies necessary migrations
- **Version tracking** - Adds `ConfigVersion` field to track configuration schema version
- **Safe backups** - Creates timestamped backups before making any changes
- **Colorful output** - Uses color-coded console messages for better visibility
- **Dry-run mode** - Preview changes without applying them
- **Multi-file support** - Can process multiple appsettings files in a directory

## Installation

Build the tool from source:

```bash
cd tools/LinkDotNet.Blog.UpgradeAssistant
dotnet build
```

## Usage

### Basic Usage

Navigate to your project directory containing `appsettings.json` and run:

```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant
```

### Specify Path

Migrate a specific file or directory:

```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --path /path/to/appsettings.json
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --path /path/to/config/directory
```

### Dry Run

Preview changes without applying them:

```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --dry-run
```

### Custom Backup Directory

Specify a custom backup location:

```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --backup-dir ./my-backups
```

## Command-Line Options

| Option | Short | Description |
|--------|-------|-------------|
| `--path <path>` | `-p` | Path to appsettings.json file or directory. Defaults to current directory. |
| `--dry-run` | `-d` | Preview changes without applying them. |
| `--backup-dir <path>` | `-b` | Custom backup directory path. Defaults to './backups'. |
| `--help` | `-h` | Display help message. |
| `--version` | `-v` | Display tool version. |

## Supported Migrations

The tool currently supports migrations from version 8.0 to 12.0:

### 8.0 → 9.0
- Moves donation settings (`KofiToken`, `GithubSponsorName`, `PatreonName`) to `SupportMe` section
- Adds `ShowSimilarPosts` setting

### 9.0 → 11.0
- Adds `UseMultiAuthorMode` setting

### 11.0 → 12.0
- Adds `ShowBuildInformation` setting

## Configuration Version

After migration, your `appsettings.json` will include a `ConfigVersion` field:

```json
{
  "ConfigVersion": "12.0",
  ...
}
```

This field is used to track the configuration schema version and determine which migrations need to be applied.

## Important Notes

- **Always backup** your configuration files before running migrations (the tool does this automatically)
- **Review changes** after migration to ensure all settings are correct
- **Database migrations** may be required separately - see `MIGRATION.md` for details
- The tool is idempotent - running it multiple times on the same file is safe

## Examples

### Migrate all appsettings files in current directory
```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant
```

### Preview changes before applying
```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --dry-run
```

### Migrate specific file with custom backup location
```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- \
  --path ./appsettings.Production.json \
  --backup-dir ./config-backups
```

## Troubleshooting

### "No appsettings.json files found"
Make sure you're in the correct directory or specify the path using `--path` option.

### "Invalid JSON"
Ensure your appsettings file is valid JSON before running the migration.

### Configuration already up to date
If you see "No migrations needed", your configuration is already at the latest version.

## See Also

- [MIGRATION.md](../../MIGRATION.md) - Manual migration guide
- [Configuration Documentation](../../docs/Setup/Configuration.md) - Configuration options
