using System.Collections.Generic;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Services;

public interface ISortOrderCalculator
{
    int GetSortOrder(ProfileInformationEntry target, IEnumerable<ProfileInformationEntry> all);
}