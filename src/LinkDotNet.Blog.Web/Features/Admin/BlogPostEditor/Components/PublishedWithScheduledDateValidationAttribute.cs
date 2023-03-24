using System;
using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PublishedWithScheduledDateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        return validationContext.ObjectInstance is CreateNewModel { IsPublished: true, ScheduledPublishDate: { } }
            ? new ValidationResult("Cannot publish the post right away and schedule it for later.")
            : ValidationResult.Success;
    }
}
