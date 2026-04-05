using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace LinkDotNet.Blog.Generators.Tests;

public sealed class EnumerationGeneratorTests
{
    [Fact]
    public void EmitsEnumerationAttribute()
    {
        var result = RunGenerator(string.Empty);

        var attributeFile = result.GeneratedTrees.SingleOrDefault(t => t.FilePath.EndsWith("EnumerationAttribute.g.cs"));
        attributeFile.ShouldNotBeNull();

        var text = attributeFile.GetText(Xunit.TestContext.Current.CancellationToken).ToString();
        text.ShouldContain("internal sealed class EnumerationAttribute");
        text.ShouldContain("params string[] values");
    }

    [Fact]
    public void GeneratesStaticFieldsForEachValue()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            [Enumeration("One", "Two", "Three")]
            public sealed partial record TestEnum;
            """;

        var text = GetGeneratedText(source, "TestEnum");

        text.ShouldContain("public static readonly TestEnum One = new(\"One\");");
        text.ShouldContain("public static readonly TestEnum Two = new(\"Two\");");
        text.ShouldContain("public static readonly TestEnum Three = new(\"Three\");");
    }

    [Fact]
    public void GeneratesAllPropertyWithFrozenSet()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            [Enumeration("Alpha", "Beta")]
            public sealed partial record Sample;
            """;

        var text = GetGeneratedText(source, "Sample");

        text.ShouldContain("public static FrozenSet<Sample> All { get; }");
        text.ShouldContain("new Sample[] { Alpha, Beta }.ToFrozenSet()");
    }

    [Fact]
    public void GeneratesCreateFactoryMethod()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            [Enumeration("A", "B")]
            public sealed partial record Foo;
            """;

        var text = GetGeneratedText(source, "Foo");

        text.ShouldContain("public static Foo Create(string key)");
        text.ShouldContain("ArgumentException.ThrowIfNullOrWhiteSpace(key)");
        text.ShouldContain("throw new InvalidOperationException");
    }

    [Fact]
    public void GeneratesStringComparisonOperators()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            [Enumeration("X")]
            public sealed partial record Bar;
            """;

        var text = GetGeneratedText(source, "Bar");

        text.ShouldContain("public static bool operator ==(Bar? a, string? b)");
        text.ShouldContain("public static bool operator !=(Bar? a, string? b) => !(a == b);");
    }

    [Fact]
    public void GeneratesMatchWithReturnValue()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            [Enumeration("SqlServer", "Sqlite", "MongoDB")]
            public sealed partial record Provider;
            """;

        var text = GetGeneratedText(source, "Provider");

        text.ShouldContain("public T Match<T>(Func<T> onSqlServer, Func<T> onSqlite, Func<T> onMongoDB)");
        text.ShouldContain("if (Key == SqlServer.Key) return onSqlServer();");
        text.ShouldContain("if (Key == Sqlite.Key) return onSqlite();");
        text.ShouldContain("if (Key == MongoDB.Key) return onMongoDB();");
    }

    [Fact]
    public void GeneratesMatchWithAction()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            [Enumeration("SqlServer", "Sqlite", "MongoDB")]
            public sealed partial record Provider;
            """;

        var text = GetGeneratedText(source, "Provider");

        text.ShouldContain("public void Match(Action onSqlServer, Action onSqlite, Action onMongoDB)");
        text.ShouldContain("if (Key == SqlServer.Key) { onSqlServer(); return; }");
    }

    [Fact]
    public void RespectsNamespace()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            namespace My.Custom.Namespace;

            [Enumeration("A", "B")]
            public sealed partial record MyEnum;
            """;

        var text = GetGeneratedText(source, "MyEnum");

        text.ShouldContain("namespace My.Custom.Namespace;");
    }

    [Fact]
    public void GeneratesPrivateConstructorWithValidation()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            [Enumeration("Val")]
            public sealed partial record Single;
            """;

        var text = GetGeneratedText(source, "Single");

        text.ShouldContain("private Single(string key)");
        text.ShouldContain("ArgumentException.ThrowIfNullOrWhiteSpace(key);");
    }

    [Fact]
    public void GeneratesToStringOverride()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            [Enumeration("Val")]
            public sealed partial record MyType;
            """;

        var text = GetGeneratedText(source, "MyType");

        text.ShouldContain("public override string ToString() => Key;");
    }

    [Fact]
    public void ProducesNoDiagnosticsForValidInput()
    {
        var source = """
            using LinkDotNet.Blog.Generators;

            [Enumeration("One", "Two")]
            public sealed partial record Clean;
            """;

        var result = RunGenerator(source);

        var diagnostics = result.Diagnostics
            .Where(d => d.Severity == DiagnosticSeverity.Error)
            .ToImmutableArray();

        diagnostics.ShouldBeEmpty();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static string GetGeneratedText(string source, string typeName)
    {
        var result = RunGenerator(source);
        var tree = result.GeneratedTrees.SingleOrDefault(t => t.FilePath.EndsWith($"{typeName}.g.cs"));
        tree.ShouldNotBeNull($"Expected generated file '{typeName}.g.cs' was not found.");
        return tree!.GetText().ToString();
    }

    private static GeneratorDriverRunResult RunGenerator(string source)
    {
        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            [CSharpSyntaxTree.ParseText(source)],
            [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new EnumerationGenerator();
        var driver = CSharpGeneratorDriver
            .Create(generator)
            .RunGenerators(compilation);

        return driver.GetRunResult();
    }
}
