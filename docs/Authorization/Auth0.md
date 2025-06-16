### Auth0

Auth0 is a prominent provider of OpenID Connect (OIDC) services, which is an authentication protocol built on top of OAuth 2.0. OpenID Connect is specifically designed for identity layer applications and provides a standardized way for users to authenticate and authorize themselves on websites or applications while allowing third-party applications to access limited user information in a secure manner.

For more information, go to: [https://auth0.com/docs/](https://auth0.com/docs/)

#### Register a client app in Auth0

-   Navigate to the Auth0 portal. In the sidebar, select Applications. Select the Regular Web Application button.
-   Provide a Name for the app (for example, Blog Client AAD).
-   In the settings menu are available the Domain, Client ID, and Client Secret values.
-   Set the `Allowed Callback URLs` list to Web and provide the following redirect URI: `https://localhost:PORT/callback` (please change the port number). If you know the production redirect URI for Auth0 default host (for example, azurewebsites.net) or the custom domain host (for example, contoso.com), you can write both redirect URIs separated by a comma: `https://localhost:PORT/callback, https://contoso.com`. Be sure to include the port number for non-:443 ports in any production redirect URIs you add.
-   Set the `Allowed logout URLs` list to Web and provide the following redirect URI: `https://localhost:PORT/callback`, and if you know your production redirect URIs, write it too.
-   In the Login Experience menu. Select the Business Users button. So, only users who belong to an organization and exist in our AuthDB.
-   In the Connections menu, disable social logins and keep `Username-Password-Authentication`.
-   In the sidebar, select Organizations. Select the Create a new Organization button.
-   Provide a Name for the organization (for example, blog-client-login). It must be lowercase.
-   In the settings menu, assign a user to the organization, which can be the same as you log in with.
-   In the Connections menu, select the Connections button.
-   Pick `Username-Password-Authentication`, our AuthDB and organization user are linked now.
-   In the sidebar, select Authentication, and Select Database.
-   In the Settings menu, we enable `Disable Sign Ups` so outsiders cannot register to our DB of Auth.
-   In the Applications menu, we verify that our `Applications using this connection.` to our DB is enabled. 
-   This process allows users who belong to an organization and are already in our AuthDB to log in to our app.

For more about application registration, please visit: [https://auth0.com/quickstarts#webapp](https://auth0.com/docs/quickstarts#webapp)

### Configuration

In `appsettings.json` change the `Authentication:Provider` to `Auth0`
and add the following configurations

```json
{
	//other configuration
	,
	"Authentication": {
		"Provider": "Auth0",
		"Domain": "",
		"ClientId": "",
		"ClientSecret": ""
	}
	// other configuration
}
```

| Property                    | Type   | Description                                                                                             |
| --------------------------- | ------ | ------------------------------------------------------------------------------------------------------- |
| Authentication:Provider     |        | Name of the auth provider                                                                               |
| `name of the auth provider` |        | Configuration for setting up the auth provider, it should be same as the value of AuthProvider property |
| Domain                      | string | Application Domain value                                              |
| ClientId                    | string | Client ID                                              |
| ClientSecret                | string | Client Secret                                              |
