using System;

namespace LinkDotNet.Blog.Domain;

public class Talk : Entity
{
    private Talk()
    {
    }

    public string PresentationTitle { get; private set; }

    public string Place { get; private set; }

    public string Description { get; private set; }

    public DateTime PublishedDate { get; private set; }

    public static Talk Create(string presentationTitle, string place, string description, DateTime publishedDate)
    {
        presentationTitle = presentationTitle.Trim();
        place = place.Trim();
        description = description.Trim();
        ArgumentException.ThrowIfNullOrEmpty(presentationTitle);
        ArgumentException.ThrowIfNullOrEmpty(place);
        ArgumentException.ThrowIfNullOrEmpty(description);

        return new Talk
        {
            PresentationTitle = presentationTitle,
            PublishedDate = publishedDate,
            Place = place,
            Description = description,
        };
    }
}