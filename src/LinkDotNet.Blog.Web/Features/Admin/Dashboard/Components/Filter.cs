using System;

namespace LinkDotNet.Blog.Web.Features.Admin.Dashboard.Components;

public sealed class Filter
{
    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
