### Disqus

For disqus you only need the short name (site-name) which you can find for example under your [home-tab](https://disqus.com/home/).

#### Configuration

In `appsettings.json` change following

```json
  "Disqus": {
    "Shortname": ""
  }
```

| Property  | Type   | Description                                                                                 |
| --------- | ------ | ------------------------------------------------------------------------------------------- |
| Disqus    | node   | Enables the comment section via disqus. If left empty the comment secion will not be shown. |
| Shortname | string |                                                                                             |
