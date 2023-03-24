using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

namespace LinkDotNet.Blog.UnitTests.Web.Features.Admin.BlogPostEditor.Components;

public class FutureDateValidationAttributeTests
{
    [Fact]
    public void GivenScheduledDateIsInThePast_WhenValidating_ThenError()
    {
        var model = new CreateNewModel
        {
            Title = "Title",
            ShortDescription = "Desc",
            Content = "Content",
            IsPublished = false,
            ScheduledPublishDate = DateTime.Now.AddDays(-1),
            PreviewImageUrl = "https://steven-giesel.com",
        };
        var validationContext = new ValidationContext(model);
        var results = new List<ValidationResult>();

        var result = Validator.TryValidateObject(model, validationContext, results, true);

        result.Should().BeFalse();
        results.Count.Should().Be(1);
    }
}
