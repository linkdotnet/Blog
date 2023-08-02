### Giscus

To provide the necessary values head over to https://giscus.app/ and go to the configuration section.
There you can enter all the information. You will find a detailed guide on the site.

In short:

-   You need a public repository where the comments are hosted. Recommendation: Create a new repository just for the comments
-   You have to link the [giscus app](https://github.com/apps/giscus) to at least the repository where the comments are hosted
-   You have to enable the discussion feature in the repository (see [here](https://docs.github.com/en/github/administering-a-repository/managing-repository-settings/enabling-or-disabling-github-discussions-for-a-repository)
    )

After you configured everything on the site, you get the `<script>` tag which you could embed. The blog needs the following information.

Here you can find an example. This is how the script tag looks on giscus.

```javascript
<script
	src="https://giscus.app/client.js"
	data-repo="your_username/reponame"
	data-repo-id="M9/ab=="
	data-category="General"
	data-category-id="AbC==/8_D"
	async
></script>
```

#### Configuration

In `appsettings.json` change following

```json
  "Giscus": {
    "Repository": "your_username/reponame",
    "RepositoryId": "M9/ab==",
    "Category": "General",
    "CategoryId": "AbC==/8_D"
  }
```

| Property     | Type   | Description                                                                                 |
| ------------ | ------ | ------------------------------------------------------------------------------------------- |
| Giscus       | node   | Enables the comment section via giscus. If left empty the comment secion will not be shown. |
| Repository   | string | path of you github repository, example `linkdotnet/Blog`                                    |
| RepositoryId | string |                                                                                             |
| Category     | string |                                                                                             |
| CategoryId   | string |                                                                                             |
| Disqus       | node   | Enables the comment section via disqus. If left empty the comment secion will not be shown. |
