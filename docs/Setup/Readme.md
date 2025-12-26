## Setup

There are two main ways of using this blog:
* Latest development build: Just clone this repository and you are good to go (aka you are using the `master` branch).
* Latest release: Download the latest release from the [release page](https://github.com/linkdotnet/Blog/releases).

The development builds can be either unstable or use preview version of packages or the .net SDK itself. So only use it if you are willing to help with testing or development.
If you just want to use the blog, use the latest release.

There are some settings you can tweak. The following chapter will guide you
through the possibilities.

-   [Configurations](Configuration.md)
-   [Docker](Docker.md)

### Updates
Going from one version to another can introduce major breaking changes. The [release page](https://github.com/linkdotnet/Blog/releases) or [`MIGRATION.md`](../../MIGRATION.md) will show the steps to migrate. In newer version automatic is possible (Entity Framework for SQL-based databases) and a custom migration tool for `appsettings.json` is provided.

## Local Development
To spin up some mock data for local development, use the following method when registering services:
```csharp
if (builder.Environment.IsDevelopment())
{
    // This fakes the whole authentication process and logs in every user automatically.
    builder.Services.UseDummyAuthentication();

    // This seeds some dummy data into the database for local development.
    // It also overrides any real database configuration to use an in-memory database.
    // So on restart (or removing this line) all data will be lost.
    builder.Services.UseDummyData();
}
```

`UseDummyData` comes with a configuration:
```csharp
builder.Services.UseDummyData(options =>
{
    options.NumberOfBlogPosts = 15; // Default is 20
});
```