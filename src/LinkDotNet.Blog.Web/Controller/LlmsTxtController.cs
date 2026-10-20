using System.Text;
using System.Threading.Tasks;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.Infrastructure.Persistence;
using LinkDotNet.Blog.Web.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace LinkDotNet.Blog.Web.Controller;

[Route("llms.txt")]
[EnableRateLimiting("ip")]
public sealed class LlmsTxtController : ControllerBase
{
    private readonly IOptions<ApplicationConfiguration> applicationConfiguration;
    private readonly IOptions<Introduction> introductionConfiguration;
    private readonly IRepository<BlogPost> blogPostRepository;

    public LlmsTxtController(
        IOptions<ApplicationConfiguration> applicationConfiguration,
        IOptions<Introduction> introductionConfiguration,
        IRepository<BlogPost> blogPostRepository)
    {
        this.applicationConfiguration = applicationConfiguration;
        this.introductionConfiguration = introductionConfiguration;
        this.blogPostRepository = blogPostRepository;
    }

    [ResponseCache(Duration = 1200)]
    [HttpGet]
    public async Task<IActionResult> GetLlmsTxt()
    {
        if (!applicationConfiguration.Value.EnableMarkdownForLlms)
        {
            return NotFound();
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        var sb = new StringBuilder();
        sb.Append("# ").Append(applicationConfiguration.Value.BlogName).Append("\n\n");
        var description = MarkdownConverter.ToPlainString(introductionConfiguration.Value.Description);
        if (description is not null)
        {
            sb.Append("> ").Append(description.ReplaceLineEndings(" ")).Append("\n\n");
        }

        sb.Append("## Blog posts\n\n");
        var blogPosts = await blogPostRepository.GetAllAsync(f => f.IsPublished, b => b.UpdatedDate);
        foreach (var blogPost in blogPosts)
        {
            sb.Append("- [").Append(blogPost.Title).Append("](").Append(baseUrl).Append("/blogPost/").Append(blogPost.Id);
            if (!string.IsNullOrEmpty(blogPost.Slug))
            {
                sb.Append('/').Append(blogPost.Slug);
            }

            sb.Append(".md)");
            var shortDescription = MarkdownConverter.ToPlainString(blogPost.ShortDescription);
            if (shortDescription is not null)
            {
                sb.Append(": ").Append(shortDescription.ReplaceLineEndings(" "));
            }

            sb.Append('\n');
        }

        return Content(sb.ToString(), "text/plain; charset=utf-8", Encoding.UTF8);
    }
}
