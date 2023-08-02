### Auth0

Auth0 is a prominent provider of OpenID Connect (OIDC) services, which is an authentication protocol built on top of OAuth 2.0. OpenID Connect is specifically designed for identity layer applications and provides a standardized way for users to authenticate and authorize themselves on websites or applications while allowing third-party applications to access limited user information in a secure manner.

For more information go to: https://auth0.com/docs/applications

### Configuration

In `appsettings.json` change the `AuthProvider` to `Auth0`
and add following configurations

```json
{
	//other configuration
	"AuthProvider": "Auth0",
	"Auth0": {
		"Domain": "",
		"ClientId": "",
		"ClientSecret": ""
	}
	// other configuration
}
```

| Property                    | Type   | Description                                                                                             |
| --------------------------- | ------ | ------------------------------------------------------------------------------------------------------- |
| AuthProvider                |        | Name of the auth provider                                                                               |
| `name of the auth provider` |        | Configuration for setting up the auth provider, it should be same as the value of AuthProvider property |
| Domain                      | string | See more details here: https://manage.auth0.com/dashboard/                                              |
| ClientId                    | string | See more details here: https://manage.auth0.com/dashboard/                                              |
| ClientSecret                | string | See more details here: https://manage.auth0.com/dashboard/                                              |
