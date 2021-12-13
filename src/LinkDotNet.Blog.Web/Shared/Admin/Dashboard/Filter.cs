using System;

namespace LinkDotNet.Blog.Web.Shared.Admin.Dashboard;

public sealed class Filter
{
    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }
}