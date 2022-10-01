using System;

namespace LinkDotNet.Blog.Domain;

public class Talk : Entity
{
    private Talk()
    {
    }

    public string Title { get; private set; }

    public DateTime PublishedDate { get; private set; }

    public string Url { get; private set; }

    public static Talk Create(string title, DateTime publishedDate, string url)
    {
        ArgumentException.ThrowIfNullOrEmpty(title);

        return new Talk
        {
            Title = title,
            PublishedDate = publishedDate,
            Url = url,
        };
    }
}