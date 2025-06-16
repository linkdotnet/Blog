### Azure Active Directory

Microsoft Entra ID (old Azure AD) is Microsoft's cloud-based identity and access management service. It supports various identity protocols, including OpenID Connect (OIDC), a standard protocol built on top of OAuth 2.0 for authentication and identity verification.

For more information, go to: https://learn.microsoft.com/en-us/azure/active-directory/fundamentals/active-directory-whatis

#### Register a client app in Azure

-   Navigate to Azure Active Directory in the Azure portal. In the sidebar, select App registrations. Select the New registration button.
-   Provide a Name for the app (for example, Blog Client AAD).
-   Choose a Supported account types. You may select `Accounts in this organizational directory only (single tenant)`.
-   Set the Redirect URI dropdown list to Web and provide the following redirect URI: `https://localhost:PORT/callback` (please change the port number). If you know the production redirect URI for the Azure default host (for example, azurewebsites.net) or the custom domain host (for example, contoso.com), you can also add the production redirect URI at the same time that you're providing the localhost redirect URI. Be sure to include the port number for non-:443 ports in any production redirect URIs that you add.
-   Select Register.
-   After Registration, go to Authentication and enable Access tokens and ID tokens inside Implicit grant and hybrid flows
-   Write down the Application (client) ID and tenant ID.
-   Go to Certificates & secrets, generate a new secret, and write it down for future use.
For more about application registration, please visit: https://learn.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app

### Configuration

In `appsettings.json`, change the `AuthenticationProvider` to `AzureAD`
and add the following configurations

```json
{
	//other configuration

	"Authentication": {
		"Provider": "AzureAD",
		"Domain": "",
		"ClientId": "",
		"ClientSecret": "",
		"LogoutUri": ""
	}
	// other configuration
}
```

| Property                    | Type   | Description                                                                                                         |
| --------------------------- | ------ | ------------------------------------------------------------------------------------------------------------------- |
| Authentication:Provider     |        | Name of the auth provider                                                                                           |
| `name of the auth provider` |        | Configuration for setting up the auth provider, it should be same as the value of AuthProvider property             |
| Domain                      | string | `login.microsoftonline.com/<TENANT_ID>/v2.0`                                                                        |
| ClientId                    | string | Application (client) ID                                                                                             |
| ClientSecret                | string | Client Secret (Value)                                                                                                     |
| LogoutUri                   | string | `https://login.microsoftonline.com/<TENANT_ID>/oauth2/v2.0/logout?post_logout_redirect_uri=https://localhost:44389` |
