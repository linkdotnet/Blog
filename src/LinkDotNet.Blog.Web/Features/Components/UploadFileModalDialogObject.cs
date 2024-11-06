using System.ComponentModel.DataAnnotations;

namespace LinkDotNet.Blog.Web.Features.Components;

public class UploadFileModalDialogObject
{
    [Required]
    public string Name { get; set; }

    public bool CacheMedia { get; set; } = true;
}
