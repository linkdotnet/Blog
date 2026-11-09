using System;
using System.Buffers;
using System.Collections.Generic;
using LinkDotNet.Blog.Domain;
using LinkDotNet.Blog.TestUtilities;
using LinkDotNet.Blog.Web.Features;

namespace LinkDotNet.Blog.UnitTests.Web.Features;

public class PersistedStateSerializerTests
{
    [Fact]
    public void ShouldRoundTripEntitiesWithPrivateMembers()
    {
        var blogPost = new BlogPostBuilder().WithTitle("Title").WithTags("a", "b").WithLikes(3).Build();
        blogPost.Id = "post";
        var skill = Skill.Create("C#", null, "Backend", "Expert");
        var talk = Talk.Create("Talk", "Place", "Desc", new DateTime(2025, 1, 2));
        var entry = ProfileInformationEntry.Create("Key", 1000);
        var shortCode = ShortCode.Create("name", "content");

        var restored = RoundTrip(new Snapshot([blogPost], [skill], [talk], [entry], [shortCode]));

        restored.Posts[0].Id.ShouldBe("post");
        restored.Posts[0].Title.ShouldBe("Title");
        restored.Posts[0].Tags.ShouldBe(["a", "b"]);
        restored.Posts[0].Likes.ShouldBe(3);
        restored.Posts[0].Slug.ShouldBe(blogPost.Slug);
        restored.Skills[0].ProficiencyLevel.ShouldBe(ProficiencyLevel.Expert);
        restored.Skills[0].Capability.ShouldBe("Backend");
        restored.Talks[0].PublishedDate.ShouldBe(talk.PublishedDate);
        restored.Talks[0].PresentationTitle.ShouldBe("Talk");
        restored.Entries[0].Content.ShouldBe("Key");
        restored.Entries[0].SortOrder.ShouldBe(1000);
        restored.ShortCodes[0].MarkdownContent.ShouldBe("content");
    }

    private static T RoundTrip<T>(T value)
    {
        var serializer = new PersistedStateSerializer<T>();
        var buffer = new ArrayBufferWriter<byte>();
        serializer.Persist(value, buffer);
        return serializer.Restore(new ReadOnlySequence<byte>(buffer.WrittenMemory));
    }

    private sealed record Snapshot(
        List<BlogPost> Posts,
        List<Skill> Skills,
        List<Talk> Talks,
        List<ProfileInformationEntry> Entries,
        List<ShortCode> ShortCodes);
}
