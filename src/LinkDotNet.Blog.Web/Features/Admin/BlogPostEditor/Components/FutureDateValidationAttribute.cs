using System;
using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FutureDateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return value is not DateTime dt || dt > DateTime.UtcNow
            ? ValidationResult.Success
            : new ValidationResult("The scheduled publish date must be in the future.");
    }
}
