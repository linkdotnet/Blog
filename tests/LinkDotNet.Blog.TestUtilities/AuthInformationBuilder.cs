using LinkDotNet.Blog.Web.Authentication.OpenIdConnect;

namespace LinkDotNet.Blog.TestUtilities;

public class AuthInformationBuilder
{
    private string clientId = "clientId";
    private string clientSecret = "client";
    private string domain = "domain";
    private string provider = "provider";
    
    public AuthInformationBuilder WithClientId(string clientId)
    {
        this.clientId = clientId;
        return this;
    }
    
    public AuthInformationBuilder WithClientSecret(string clientSecret)
    {
        this.clientSecret = clientSecret;
        return this;
    }
    
    public AuthInformationBuilder WithDomain(string domain)
    {
        this.domain = domain;
        return this;
    }
    
    public AuthInformationBuilder WithProvider(string provider)
    {
        this.provider = provider;
        return this;
    }
    
    public AuthInformation Build()
    {
        return new AuthInformation
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            Domain = domain,
            Provider = provider,
        };
    }
}
