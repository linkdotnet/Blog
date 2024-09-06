using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BlogPostEditor.Components;

public class FallbackUrlValidationAttributeTests
{
    [Fact]
    public void GivenPreviewImageUrlIsSameAsFallback_WhenValidating_ThenError()
    {
        const string url = "https://steven-giesel.com";
        var model = new CreateNewModel
        {
            Title = "Title",
            ShortDescription = "Desc",
            Content = "Content",
            PreviewImageUrl = url,
            PreviewImageUrlFallback = url,
        };
        var validationContext = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var result = Validator.TryValidateObject(model, validationContext, results, true);

        result.ShouldBeFalse();
        results.Count.ShouldBe(1);
    }

    [Fact]
    public void GivenPreviewImageUrlIsNotSameAsFallback_WhenValidating_ThenSuccess()
    {
        var model = new CreateNewModel
        {
            Title = "Title",
            ShortDescription = "Desc",
            Content = "Content",
            PreviewImageUrl = "https://steven-giesel.com",
            PreviewImageUrlFallback = "https://different.url",
        };
        var validationContext = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var result = Validator.TryValidateObject(model, validationContext, results, true);

        result.ShouldBeTrue();
    }
}
