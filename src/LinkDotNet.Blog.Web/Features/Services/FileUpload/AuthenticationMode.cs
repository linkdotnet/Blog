using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.Web.Features.Services.FileUpload;

public class AuthenticationMode : Enumeration<AuthenticationMode>
{
    public static readonly AuthenticationMode Default = new(nameof(Default));

    public static readonly AuthenticationMode ConnectionString = new(nameof(ConnectionString));

    public AuthenticationMode(string key) : base(key)
    {
    }
}
