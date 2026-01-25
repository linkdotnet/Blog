using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.MarkdownImport;

public interface IMarkdownSourceProvider
{
    Task<IReadOnlyCollection<MarkdownFile>> GetMarkdownFilesAsync(CancellationToken cancellationToken = default);
}

public sealed record MarkdownFile(string FileName, string Content);
