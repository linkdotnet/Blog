using System;
using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

[AttributeUsage(AttributeTargets.Property)]
public class FallbackUrlValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var model = validationContext.ObjectInstance as CreateNewModel;

        return model.PreviewImageUrl == model.PreviewImageUrlFallback
            ? new ValidationResult("Preview image url and the fallback preview image url should not be the same.")
            : ValidationResult.Success;
    }
}
