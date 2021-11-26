namespace LinkDotNet.Blog.Web.Pages;

public static class DebugHelper
{
#if DEBUG
    public const bool IsDebug = true;
#else
    public const bool IsDebug = false;
#endif

}
