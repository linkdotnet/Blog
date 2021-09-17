using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace LinkDotNet.Blog.Web.Shared.Services
{
    public class GiscusService : ICommentService
    {
        private readonly IJSRuntime jsRuntime;

        public GiscusService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task EnableCommentSection(string className)
        {
            var giscus = new Giscus
            {
                Repository = "linkdotnet/Blog.Discussions",
                RepositoryId = "MDEwOlJlcG9zaXRvcnk0MDc1MzQ0OTA=",
                Category = "General",
                CategoryId = "DIC_kwDOGEp7ms4B_Fx_",
            };

            await jsRuntime.InvokeVoidAsync("initGiscus", "giscus", giscus);
        }
    }
}