using System;
using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.TestUtilities;

public class TalkBuilder
{
    private string title = "Some presentation";
    private string place = "NDC Oslo";
    private string description = "Details";
    private DateTime publishDate = new DateTime(2022, 10, 2);

    public TalkBuilder WithTitle(string title)
    {
        this.title = title;
        return this;
    }

    public TalkBuilder WithPlace(string place)
    {
        this.place = place;
        return this;
    }

    public TalkBuilder WithDescription(string description)
    {
        this.description = description;
        return this;
    }

    public TalkBuilder WithPublishedDate(DateTime publishDate)
    {
        this.publishDate = publishDate;
        return this;
    }

    public Talk Build()
    {
        return Talk.Create(title, place, description, publishDate);
    }
}