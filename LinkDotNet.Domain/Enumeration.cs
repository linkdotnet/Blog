using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LinkDotNet.Domain
{
     public abstract class Enumeration<TEnumeration>
        where TEnumeration : Enumeration<TEnumeration>
    {
        private readonly string key;

        protected Enumeration()
        {
        }

        protected Enumeration(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("The enum key cannot be null or empty");
            }

            this.key = key;
        }

        public static IReadOnlyCollection<TEnumeration> All => GetEnumerations();

        public string Key => key;

        public static bool operator ==(Enumeration<TEnumeration> a, Enumeration<TEnumeration> b)
        {
            if (a is null || b is null)
            {
                return false;
            }

            return a.key.Equals(b.key);
        }

        public static bool operator !=(Enumeration<TEnumeration> a, Enumeration<TEnumeration> b)
        {
            return !(a == b);
        }

        public static TEnumeration Create(string key)
        {
            var enumeration = All.SingleOrDefault(p => p.key == key);

            if (enumeration is null)
            {
                throw new InvalidOperationException($"{key} is not a valid value for {typeof(TEnumeration).Name}");
            }

            return enumeration;
        }

        public override int GetHashCode() => Key.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != typeof(TEnumeration))
            {
                return false;
            }

            return (obj as TEnumeration).key == key;
        }

        public override string ToString() => Key;

        private static TEnumeration[] GetEnumerations()
        {
            var enumerationType = typeof(TEnumeration);

            return enumerationType
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(info => info.FieldType == typeof(TEnumeration))
                .Select(info => (TEnumeration)info.GetValue(null))
                .ToArray();
        }
    }
}