using System;
using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.Blog.Web.Features.AboutMe.Components.Talk;

public sealed class AddTalkEntryModel
{
    [Required]
    [MaxLength(256)]
    public string PresentationTitle { get; set; }

    [Required]
    [MaxLength(256)]
    public string Place { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public DateTime PublishedDate { get; set; } = DateTime.Now;
}