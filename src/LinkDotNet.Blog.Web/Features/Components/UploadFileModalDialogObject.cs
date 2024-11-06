using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.Blog.Web.Features.Components;

public class UploadFileModalDialogObject
{
    [Required]
    [MinLength(1)]
    public string Name { get; set; } = string.Empty;

    public bool CacheMedia { get; set; } = true;
}
