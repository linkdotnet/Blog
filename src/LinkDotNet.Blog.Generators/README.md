# LinkDotNet.Blog.Generators

This project is a lightweight version of https://github.com/linkdotnet/Enumeration

The goal is that we have this:
```csharp
public abstract record Enumeration<TEnumeration>(string Key)
    where TEnumeration : Enumeration<TEnumeration>
{
    public static FrozenSet<TEnumeration> All { get; } = GetEnumerations();

    public static bool operator ==(Enumeration<TEnumeration>? a, string? b)
        => a is not null && b is not null && a.Key.Equals(b, StringComparison.Ordinal);

    public static bool operator !=(Enumeration<TEnumeration>? a, string? b) => !(a == b);

    public static TEnumeration Create(string key)
        => All.SingleOrDefault(p => p.Key == key)
           ?? throw new InvalidOperationException($"{key} is not a valid value for {typeof(TEnumeration).Name}");

    public sealed override string ToString() => Key;

    private static FrozenSet<TEnumeration> GetEnumerations()
    {
        var enumerationType = typeof(TEnumeration);

        return enumerationType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(info => info.FieldType == typeof(TEnumeration))
            .Select(info => (TEnumeration)info.GetValue(null)!)
            .ToFrozenSet();
    }
}
```

At compile time! The idea is that we have the `EnumerationAttribute` that will scaffold the code for us.
