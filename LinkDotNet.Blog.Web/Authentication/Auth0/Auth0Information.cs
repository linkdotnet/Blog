namespace LinkDotNet.Blog.Web.Authentication.Auth0
{
    public record Auth0Information
    {
        public string Domain { get; init; }

        public string ClientId { get; init; }
        
        public string ClientSecret { get; init; }
    }
}