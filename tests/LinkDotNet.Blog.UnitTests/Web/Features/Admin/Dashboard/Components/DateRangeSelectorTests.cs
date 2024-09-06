using System;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.Dashboard.Components;

public class DateRangeSelectorTests : BunitContext
{
    [Fact]
    public void ShouldSetBeginDateWhenSet()
    {
        Filter filter = null;
        var cut = Render<DateRangeSelector>(p => p.Add(s => s.FilterChanged, f =>
        {
            filter = f;
        }));

        cut.Find("#startDate").Change(new DateOnly(2020, 1, 1));

        filter.ShouldNotBeNull();
        filter.StartDate.ShouldBe(new DateOnly(2020, 1, 1));
        filter.EndDate.ShouldBeNull();
    }

    [Fact]
    public void ShouldSetEndDateWhenSet()
    {
        Filter filter = null;
        var cut = Render<DateRangeSelector>(p => p.Add(s => s.FilterChanged, f =>
        {
            filter = f;
        }));

        cut.Find("#endDate").Change(new DateOnly(2020, 1, 1));

        filter.ShouldNotBeNull();
        filter.EndDate.ShouldBe(new DateOnly(2020, 1, 1));
        filter.StartDate.ShouldBeNull();
    }

    [Fact]
    public void ShouldReset()
    {
        Filter filter = null;
        var cut = Render<DateRangeSelector>(p => p.Add(s => s.FilterChanged, f =>
        {
            filter = f;
        }));
        cut.Find("#startDate").Change(new DateOnly(2020, 1, 1));
        cut.Find("#endDate").Change(new DateOnly(2020, 1, 1));

        cut.Find("#startDate").Change(string.Empty);
        cut.Find("#endDate").Change(string.Empty);

        filter.StartDate.ShouldBeNull();
        filter.EndDate.ShouldBeNull();
    }
}
