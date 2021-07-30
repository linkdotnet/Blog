using LinkDotNet.Domain;

namespace LinkDotNet.Blog.UnitTests.Domain
{
    public sealed class TestEnumeration : Enumeration<TestEnumeration>
    {
        public static readonly TestEnumeration One = new TestEnumeration(nameof(One));

        public static readonly TestEnumeration Two = new TestEnumeration(nameof(Two));

        public TestEnumeration(string key) : base(key)
        {
        }
    }
}