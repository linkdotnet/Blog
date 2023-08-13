﻿using System;
using LinkDotNet.Blog.Web.Features.Admin.Dashboard.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.Dashboard.Components;

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

        cut.Find("#startDate").Change(new DateOnly(2020, 1, 1));

        filter.Should().NotBeNull();
        filter.StartDate.Should().Be(new DateOnly(2020, 1, 1));
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

        cut.Find("#endDate").Change(new DateOnly(2020, 1, 1));

        filter.Should().NotBeNull();
        filter.EndDate.Should().Be(new DateOnly(2020, 1, 1));
        filter.StartDate.Should().BeNull();
    }

    [Fact]
    public void ShouldReset()
    {
        Filter filter = null;
        var cut = RenderComponent<DateRangeSelector>(p => p.Add(s => s.FilterChanged, f =>
        {
            filter = f;
        }));
        cut.Find("#startDate").Change(new DateOnly(2020, 1, 1));
        cut.Find("#endDate").Change(new DateOnly(2020, 1, 1));

        cut.Find("#startDate").Change(string.Empty);
        cut.Find("#endDate").Change(string.Empty);

        filter.StartDate.Should().BeNull();
        filter.EndDate.Should().BeNull();
    }
}
