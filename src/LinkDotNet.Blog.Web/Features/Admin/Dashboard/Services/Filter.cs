using System;

namespace LinkDotNet.Blog.Web.Features.Admin.Dashboard.Services;

public sealed class Filter
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}