using System;
using Bunit;
using FluentAssertions;
using LinkDotNet.Blog.Web.Shared.Admin.Dashboard;
using Xunit;

namespace LinkDotNet.Blog.UnitTests.Web.Shared.Admin.Dashboard;

public class DateRangeSelectorTests : TestContext
{
    [Fact]
    public void ShouldSetBeginDateWhenSet()
    {
        Filter filter = null;
        var cut = RenderComponent<DateRangeSelector>(p => p.Add(s => s.FilterChanged, f =>
        {
            filter = f;
        }));

        cut.Find("#startDate").Change(new DateTime(2020, 1, 1));

        filter.Should().NotBeNull();
        filter.StartDate.Should().Be(new DateTime(2020, 1, 1));
        filter.EndDate.Should().BeNull();
    }

    [Fact]
    public void ShouldSetEndDateWhenSet()
    {
        Filter filter = null;
        var cut = RenderComponent<DateRangeSelector>(p => p.Add(s => s.FilterChanged, f =>
        {
            filter = f;
        }));

        cut.Find("#endDate").Change(new DateTime(2020, 1, 1));

        filter.Should().NotBeNull();
        filter.EndDate.Should().Be(new DateTime(2020, 1, 1));
        filter.StartDate.Should().BeNull();
    }
}
