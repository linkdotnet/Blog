using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace LinkDotNet.Blog.Web.Shared.Services
{
    public class GiscusService : IGiscusService
    {
        private readonly IJSRuntime jsRuntime;
        private readonly AppConfiguration appConfiguration;

        public GiscusService(IJSRuntime jsRuntime, AppConfiguration appConfiguration)
        {
            this.jsRuntime = jsRuntime;
            this.appConfiguration = appConfiguration;
        }

        public async Task EnableCommentSection(string className)
        {
            await jsRuntime.InvokeVoidAsync("initGiscus", "giscus", appConfiguration.Giscus);
        }
    }
}