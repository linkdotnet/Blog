using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services.Tags;

public interface ITagQueryService
{
    Task<IReadOnlyList<TagCount>> GetAllOrderedByUsageAsync();
}
