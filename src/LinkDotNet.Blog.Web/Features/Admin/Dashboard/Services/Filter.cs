using System;

namespace LinkDotNet.Blog.Web.Features.Admin.Dashboard.Services;

public sealed class Filter
{
    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}
