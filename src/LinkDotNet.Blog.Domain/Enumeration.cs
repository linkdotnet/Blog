using System;
using System.Collections.Frozen;
using System.Linq;
using System.Reflection;

namespace LinkDotNet.Blog.Domain;

public abstract class Enumeration<TEnumeration>
   where TEnumeration : Enumeration<TEnumeration>
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
    {
        if (a is null || b is null)
        {
            return false;
        }

        return a.Key.Equals(b.Key);
    }

    public static bool operator !=(Enumeration<TEnumeration> a, Enumeration<TEnumeration> b)
    {
        return !(a == b);
    }

    public static TEnumeration Create(string key)
        => All.SingleOrDefault(p => p.Key == key)
           ?? throw new InvalidOperationException($"{key} is not a valid value for {typeof(TEnumeration).Name}");

    public override int GetHashCode() => Key.GetHashCode();

    public override bool Equals(object obj)
    {
        if (obj?.GetType() != typeof(TEnumeration))
        {
            return false;
        }

        return ((TEnumeration)obj).Key == Key;
    }

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
