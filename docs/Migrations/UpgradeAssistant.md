# Configuration Upgrade Assistant

The Blog Upgrade Assistant is an automated tool that helps you migrate your `appsettings.json` configuration files to the latest version when upgrading the blog to a new major version. This was introduced in version 12 of the blog. The update software tries to upgrade from version 11 to 12 automatically but not from earlier versions.

## Why Use the Upgrade Assistant?

When upgrading the blog to a new major version, the configuration schema may change:
- New mandatory settings may be added
- Settings may be moved between sections
- Default values may change

The Upgrade Assistant automates these changes, reducing errors and making upgrades easier.

## Quick Start

1. **Before upgrading**, backup your configuration files (the tool also does this automatically)
2. Navigate to your blog installation directory
3. Run the upgrade assistant:
   ```bash
   dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant
   ```
4. Review the changes and update any values as needed
5. Complete any additional manual steps from [MIGRATION.md](../../MIGRATION.md)

## Installation

The tool is included with the blog source code in the `tools/LinkDotNet.Blog.UpgradeAssistant` directory. No separate installation is needed.

To build the tool:
```bash
cd tools/LinkDotNet.Blog.UpgradeAssistant
dotnet build
```

## Usage Guide

### Basic Migration

The simplest way to use the tool is to run it from your blog directory:

```bash
# Navigate to your blog installation
cd /path/to/your/blog

# Run the upgrade assistant
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant
```

This will:
1. Find all `appsettings*.json` files in the current directory
2. Detect the current configuration version
3. Apply all necessary migrations
4. Save backups to `./backups` directory

### Preview Changes (Dry Run)

To see what changes will be made without actually modifying files:

```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --dry-run
```

The tool will display:
- Which migrations will be applied
- A preview of the modified configuration
- Warnings about new settings that require attention

### Migrate Specific Files

To migrate a specific configuration file:

```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --path /path/to/appsettings.Production.json
```

To migrate all files in a specific directory:

```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --path /path/to/config
```

### Custom Backup Location

By default, backups are saved to `./backups`. To use a custom location:

```bash
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --backup-dir /path/to/backup/location
```

## Understanding the Output

The tool uses color-coded output for clarity:

- 🟢 **Green (Success)**: Operation completed successfully
- 🔵 **Cyan (Info)**: Informational messages
- 🟡 **Yellow (Warning)**: Warnings about settings that need attention
- 🔴 **Red (Error)**: Errors that need to be resolved
- 🟣 **Magenta (Headers)**: Section headers

### Example Output

```
═══ Blog Upgrade Assistant ═══

ℹ Target: /path/to/blog
ℹ Backup directory: ./backups
ℹ Found 2 file(s) to process.

═══ Processing: appsettings.json ═══

ℹ Current version: Not set (pre-12.0)
  → Found 1 migration(s) to apply:
  →   • 11.0 → 12.0: Adds ShowBuildInformation setting to control build information display.
✓ Backup created: ./backups/appsettings_20251221_120000.json
  → Applying migration: 11.0 → 12.0
ℹ Added 'ShowBuildInformation' setting. Controls display of build information in the footer.
✓ Migration 11.0 → 12.0 applied successfully.
...
✓ File updated successfully: /path/to/blog/appsettings.json

═══ Migration Complete ═══

✓ All files processed successfully!
```

## How It Works

### Version Detection

The tool looks for a `ConfigVersion` field in your `appsettings.json`:

```json
{
  "ConfigVersion": "12.0",
  ...
}
```

If this field doesn't exist, the tool assumes you're running version 11.0 or earlier and will apply all necessary migrations.

### Migration Chain

The tool applies migrations sequentially:
1. Detects current version (e.g., 12.0)
2. Finds all migrations from current to latest (11.0→12.0)
3. Applies each migration in order
4. Updates the `ConfigVersion` field to the latest version

### Backup Process

Before making any changes, the tool:
1. Creates a `backups` directory (or uses the specified backup location)
2. Copies each file with a timestamp: `appsettings_20241221_120000.json`
3. Preserves the original file structure and formatting

### Idempotency

The tool is **idempotent** - running it multiple times on the same file is safe:
- If no migrations are needed, no changes are made
- Already migrated files show "No migrations needed"
- Version tracking ensures migrations aren't applied twice

## Migration Details


**Changes:**
- Adds `UseMultiAuthorMode` setting (default: `false`)

**After:**
```json
{
  "UseMultiAuthorMode": false
}
```

**Manual Steps Required:**
- Set to `true` if you want multi-author support
- Configure author information per blog post

### Version 11.0 → 12.0

**Changes:**
- Adds `ShowBuildInformation` setting (default: `true`)

**After:**
```json
{
  "ShowBuildInformation": true
}
```

**Manual Steps Required:**
- None (setting is optional)

## Command-Line Reference

```
Usage: upgrade-assistant [options]

Options:
  -p, --path <path>         Path to appsettings.json file or directory
                            Defaults to current directory
  -d, --dry-run             Preview changes without applying them
  -b, --backup-dir <path>   Custom backup directory path
                            Defaults to './backups'
  -h, --help                Display help message
  -v, --version             Display tool version
```

## Troubleshooting

### "No appsettings.json files found"

**Cause:** Tool can't find configuration files in the specified location.

**Solution:**
```bash
# Specify the correct path
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --path /correct/path
```

### "Invalid JSON in appsettings.json"

**Cause:** Your configuration file has JSON syntax errors.

**Solution:**
1. Open the file in a JSON validator or editor
2. Fix syntax errors (missing commas, brackets, etc.)
3. Run the tool again

### "Configuration is already up to date"

**Cause:** Your configuration is already at the latest version.

**Solution:** No action needed! Your configuration is current.

### Migration Applied but Application Fails

**Possible Causes:**
- Database migrations not applied
- Custom configuration values need adjustment
- Environment-specific settings need updating

**Solution:**
1. Check [MIGRATION.md](../../MIGRATION.md) for additional manual steps
2. Review application logs for specific errors
3. Verify all required database tables exist
4. Check environment-specific configuration files

### Need to Revert Changes

**Solution:**
1. Navigate to the backup directory (default: `./backups`)
2. Find the backup file with the timestamp before migration
3. Copy it back to your configuration location
4. Restart the application

Example:
```bash
cp backups/appsettings_20251221_120000.json appsettings.json
```

## Advanced Usage

### Batch Processing

Process multiple configuration directories:

```bash
for dir in /path/to/blog1 /path/to/blog2 /path/to/blog3; do
  dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --path $dir
done
```

### Automated CI/CD Integration

Include in your deployment pipeline:

```yaml
# Example GitHub Actions workflow
- name: Upgrade Configuration
  run: |
    dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant \
      --path ./deployment/config \
      --backup-dir ./backups
```

### Scripted Migration with Verification

```bash
#!/bin/bash
# Upgrade with verification

# Run dry-run first
dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --dry-run --path ./config

# Ask for confirmation
read -p "Proceed with migration? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
  # Run actual migration
  dotnet run --project tools/LinkDotNet.Blog.UpgradeAssistant -- --path ./config
  echo "Migration complete. Please review the changes."
fi
```

## Additional Resources

- [MIGRATION.md](../../MIGRATION.md) - Complete migration guide with manual steps
- [Configuration Documentation](../Setup/Configuration.md) - All configuration options
- [Tool README](../../tools/LinkDotNet.Blog.UpgradeAssistant/README.md) - Developer documentation

## Getting Help

If you encounter issues:

1. Check this documentation and [MIGRATION.md](../../MIGRATION.md)
2. Review the tool's output messages - they often contain helpful information
3. Open an issue on GitHub with:
   - Tool output (with sensitive data removed)
   - Your configuration version
   - Error messages or unexpected behavior
