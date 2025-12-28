namespace LinkDotNet.Blog.Domain;

public sealed class LikeIconStyle : Enumeration<LikeIconStyle>
{
    public static readonly LikeIconStyle ThumbsUp = new(nameof(ThumbsUp));
    public static readonly LikeIconStyle PlusPlus = new(nameof(PlusPlus));

    private LikeIconStyle(string key)
        : base(key)
    {
    }
}
