using System;
using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PublishedWithScheduledDateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        ArgumentNullException.ThrowIfNull(validationContext);

        return validationContext.ObjectInstance is CreateNewModel { IsPublished: true, ScheduledPublishDate: not null }
            ? new ValidationResult("Cannot publish the post right away and schedule it for later.")
            : ValidationResult.Success;
    }
}
