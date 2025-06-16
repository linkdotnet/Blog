### Authentication and Authorization

When it comes to authentication and authorization, we employ [OpenID Connect](https://openid.net/developers/how-connect-works/) as our preferred method. The primary benefit of an OpenID Connect-based provider is the conveniently customizable dashboard accessible through their website. For the sake of testing, you have the option to employ the `UseDummyAuthentication();` service. This grants every user who clicks "Login" immediate access, effectively logging them in. Here are the platforms we support:

-   [Auth0](Auth0.md)
-   [Microsoft Entra ID(old Azure AD)](AzureAD.md)
-   And more...
