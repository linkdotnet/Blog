using FluentAssertions;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Shared.Services;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared
{
    public class SortOrderCalculatorTests
    {
        private readonly SortOrderCalculator sut;

        public SortOrderCalculatorTests()
        {
            sut = new SortOrderCalculator();
        }

        [Fact]
        public void ShouldProperlyCalculateNewSortOrder()
        {
            var target = new ProfileInformationEntryBuilder().WithSortOrder(1000).Build();
            var next = new ProfileInformationEntryBuilder().WithSortOrder(2000).Build();
            var all = new[] { target, next };

            var newSortOrder = sut.GetSortOrder(target, all);

            newSortOrder.Should().Be(1500);
        }

        [Fact]
        public void ShouldProperlyCalculateNewSortOrderWhenLastItem()
        {
            var target = new ProfileInformationEntryBuilder().WithSortOrder(2000).Build();
            var previous = new ProfileInformationEntryBuilder().WithSortOrder(1000).Build();
            var all = new[] { previous, target };

            var newSortOrder = sut.GetSortOrder(target, all);

            newSortOrder.Should().Be(1500);
        }
    }
}