using System;
using System.Collections.Frozen;
using System.Linq;
using System.Reflection;

namespace LinkDotNet.Blog.Domain;

#pragma warning disable CA1724
public abstract class Enumeration<TEnumeration>
   where TEnumeration : Enumeration<TEnumeration>
#pragma warning restore
{
    protected Enumeration()
    {
    }

    protected Enumeration(string key)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        Key = key;
    }

    public static FrozenSet<TEnumeration> All { get; } = GetEnumerations();

    public string Key { get; }

    public static bool operator ==(Enumeration<TEnumeration> a, Enumeration<TEnumeration> b)
        => a is not null && b is not null && a.Key.Equals(b.Key, StringComparison.Ordinal);

    public static bool operator !=(Enumeration<TEnumeration> a, Enumeration<TEnumeration> b) => !(a == b);

    public static TEnumeration Create(string key)
        => All.SingleOrDefault(p => p.Key == key)
           ?? throw new InvalidOperationException($"{key} is not a valid value for {typeof(TEnumeration).Name}");

    public override int GetHashCode() => Key.GetHashCode(StringComparison.Ordinal);

    public override bool Equals(object obj) => obj?.GetType() == typeof(TEnumeration) && ((TEnumeration)obj).Key == Key;

    public override string ToString() => Key;

    private static FrozenSet<TEnumeration> GetEnumerations()
    {
        var enumerationType = typeof(TEnumeration);

        return enumerationType
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(info => info.FieldType == typeof(TEnumeration))
            .Select(info => (TEnumeration)info.GetValue(null))
            .ToFrozenSet();
    }
}
