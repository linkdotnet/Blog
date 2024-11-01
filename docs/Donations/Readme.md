## Donations

The blog software allows you to integrate via different micro-transaction services. The following chapter will show you how to set up donations.

### Ko-fi

You can use [Ko-fi](https://Ko-fi.com/) as a payment service to receive donations. To acquire the `KofiToken` as seen in the config above, head to [widgets page](https://Ko-fi.com/manage/widgets), click on "Ko-fi Button".
Now choose "Image" as the type. In the field below under `Copy & Paste Code` you see an `<a href='https://ko-fi.com/XYZ'` tag. Just take the `XYZ` part and put it into `KofiToken`.

### GitHub Sponsor

Enables the usage of [GitHub Sponsors](https://github.com/sponsors) as a payment service to receive donations. Only pass in your username. The button will use the following url: `https://github.com/sponsors/{your-user-name}`.

### Patreon

Enables the usage of [Patreon](https://www.patreon.com). Only pass the user name (public profile) as user name.

### Configuration
```json
"SupportMe": {
		"KofiToken": "ABC123",
		"GithubSponsorName": "your-tag-here",
		"PatreonName": "your-tag-here",
		"ShowUnderBlogPost": true,
		"ShowUnderIntroduction": true,
		"ShowInFooter": true,
		"ShowSupportMePage": true,
		"SupportMePageDescription": "Buy my book here: [My Blazor Book](https://google.com) or please contribute to my open-source project here: [My Awesome Repo](https://github.com) . This can be **markdown**."
}
```

| Property                                      | Type           | Description                                                                                                                                                                      |
| --------------------------------------------- | -------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| KofiToken                                     | string         | Enables the "Buy me a Coffee" button of Kofi. To aquire the token head down to the "Kofi" section                                                                                |
| GithubSponsorName                             | string         | Enables the "Github Sponsor" button which redirects to GitHub. Only pass in the user name instead of the url.                                                                    |
| PatreonName                                   | string         | Enables the "Become a patreon" button that redirects to patreon.com. Only pass the user name (public profile) as user name.                                                      |
| ShowUnderBlogPost                             | boolean         | Enables the donation section with the configured services to show under blog posts.                                                     |
| ShowUnderIntroduction                         | boolean         | Enables the donation section with the configured services to show the introduction on the main page.                                                     |
| ShowInFooter                                  | boolean         | Enables the donation section with the configured services to show on the blog's footer.                                                     |
| ShowSupportMePage                             | boolean         | Enables the Support Me page with shows the `SupportMePageDescription` as text then the configured donation buttons.                                                    |
| SupportMePageDescription                      | string         | Shows on the Support Me page, can be markdown.                                                    |
