# LinkDotNet.Blog
This is a blog software completely written in C# / Blazor. The aim is to have it configurable as possible. 

## How does it work
The basic idea is that the content creator writes his posts in markdown language (like this readme file). 
The markdown will then by translated to HTML and displayed to the client. This gives an easy entry to writing posts with all the flexibility markdown has.

## Setup
Just clone this repository and you are good to go. There are some settings you can tweak.

### appsettings.json
The appsettings.json file has a lot of options to customize the content of the blog. The following table shows which values are used when.

```json
{
  ...
  "BlogName": "linkdotnet",
  "GithubAccountUrl": "",
  "LinkedInAccountUrl": "",
  "Introduction": {
    "Description": "Hey, my name is **Steven**. I am a **.NET Developer** based in Zurich, Switzerland. This is my small blog, which I wrote completely in Blazor. If you want to know more about me just check out my LinkedIn or Github.\nAlso this blogsoftware is open source on [Github](https://github.com/linkdotnet/blog)",
    "BackgroundUrl": "assets/profile-background.png",
    "ProfilePictureUrl": "assets/profile-picture.jfif"
  },
  "ConnectionString": "",
  "DatabaseName": "",
  "Auth0": {
    "Domain": "",
    "ClientId": "",
    "ClientSecret": ""
  }
}

```

| Property | Type | Description |
|----------|------|-------|
|BlogName|string|Name of your blog. Is used in the navbar|
|GithubAccountUrl|string|Url to your github account. If not set the navigation link is not shown|
|LinkedInAccountUrl|string|Url to your LinkedIn account. If not set the navigation link is not shown|
|Introduction| |Is used for the introduction part of the blog|
|Description|MarkdownString|Small introduction text for yourself.|
|BackgroundUrl|string|Url or path to the background image|
|ProfilePictureUrl|string|Url or path to your profile picture|
|ConnectionString|string|Is used for connection to a database. Not used when `InMemoryStorageProvider` is used|
|DatabaseName|string|Name of the database. Only used with `RavenDbStorageProvider`|
|Auth0| |Configuration for setting up Auth0|
|Domain|string|See more details here: https://manage.auth0.com/dashboard/|
|ClientId|string|See more details here: https://manage.auth0.com/dashboard/|
|ClientSecret|string|See more details here: https://manage.auth0.com/dashboard/|