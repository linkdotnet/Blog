using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace LinkDotNet.Blog.IntegrationTests.Infrastructure.Persistence;

// One container per database for the whole test run; Testcontainers' resource reaper removes it when the process exits.
internal sealed class TestContainer
{
    private readonly Lazy<Task<string?>> connectionString;
    private readonly string name;

    private TestContainer(string name, Func<Task<string>> start)
    {
        this.name = name;
        connectionString = new Lazy<Task<string?>>(() => TryStartAsync(start));
    }

    public static TestContainer Create<TContainer>(string name, Func<TContainer> build, Func<TContainer, string> getConnectionString)
        where TContainer : IContainer =>
        new(name, async () =>
        {
            var container = build();
            await container.StartAsync();
            return getConnectionString(container);
        });

    public async Task<string> GetConnectionStringAsync()
    {
        var value = await connectionString.Value;
        if (value is null)
        {
            Assert.Skip($"Docker is not available to run {name}.");
        }

        return value;
    }

    private static async Task<string?> TryStartAsync(Func<Task<string>> start)
    {
        try
        {
            return await start();
        }
        catch (DockerUnavailableException)
        {
            return null;
        }
    }
}
