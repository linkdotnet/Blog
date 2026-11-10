using System.Collections.Generic;
using System.Linq;

namespace LinkDotNet.Blog.Web.Features.Components;

public class UploadFileModalDialogObject
{
    public string Folder { get; set; } = string.Empty;

    public IList<string> Names { get; set; } = [];

    public bool CacheMedia { get; set; } = true;

    public bool IsValid => Names.Count > 0 && Names.All(name => !string.IsNullOrWhiteSpace(name));

    public IReadOnlyList<string> GetPaths() => Names.Select(ToPath).ToList();

    private string ToPath(string name)
    {
        var folder = Folder.Trim().Trim('/');
        return folder.Length == 0 ? name.Trim() : $"{folder}/{name.Trim()}";
    }
}
