using LinkDotNet.Blog.Domain;

namespace LinkDotNet.Blog.UnitTests.Domain;

public class LikeIconStyleTests
{
    [Fact]
    public void ShouldContainThumbsUpIcon()
    {
        var thumbsUp = LikeIconStyle.ThumbsUp;

        thumbsUp.Key.ShouldBe("ThumbsUp");
    }

    [Fact]
    public void ShouldContainPlusPlusIcon()
    {
        var plusPlus = LikeIconStyle.PlusPlus;

        plusPlus.Key.ShouldBe("PlusPlus");
    }

    [Fact]
    public void ShouldGetAllLikeIconStyles()
    {
        var all = LikeIconStyle.All;

        all.Count.ShouldBe(2);
        all.ShouldContain(LikeIconStyle.ThumbsUp);
        all.ShouldContain(LikeIconStyle.PlusPlus);
    }

    [Fact]
    public void ShouldCreateFromKey()
    {
        var thumbsUp = LikeIconStyle.Create("ThumbsUp");
        var plusPlus = LikeIconStyle.Create("PlusPlus");

        thumbsUp.ShouldBe(LikeIconStyle.ThumbsUp);
        plusPlus.ShouldBe(LikeIconStyle.PlusPlus);
    }
}
