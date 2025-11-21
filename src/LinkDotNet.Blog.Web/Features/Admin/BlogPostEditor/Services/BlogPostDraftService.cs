using System;
using System.Globalization;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;
using LinkDotNet.Blog.Web.Features.Services;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;

public sealed class BlogPostDraftService : IBlogPostDraftService
{
    private const string DraftStorageKey = "blogpost-draft";
    private readonly ILocalStorageService localStorageService;

    public BlogPostDraftService(ILocalStorageService localStorageService)
    {
        this.localStorageService = localStorageService;
    }

    public async ValueTask<(bool exists, string savedTime)> CheckForSavedDraftAsync()
    {
        try
        {
            if (await localStorageService.ContainsKeyAsync(DraftStorageKey))
            {
                var draft = await localStorageService.GetItemAsync<DraftBlogPostModel>(DraftStorageKey);
                if (draft != null)
                {
                    var savedTime = draft.SavedAt.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
                    return (true, savedTime);
                }
            }
        }
        catch
        {
            // If there's any error reading the draft, just ignore it
        }

        return (false, string.Empty);
    }

    public async ValueTask SaveDraftAsync(CreateNewModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        try
        {
            var draft = DraftBlogPostModel.FromCreateNewModel(model);
            await localStorageService.SetItemAsync(DraftStorageKey, draft);
        }
        catch
        {
            // If auto-save fails, just ignore it silently
        }
    }

    public async ValueTask<CreateNewModel?> RestoreDraftAsync()
    {
        try
        {
            var draft = await localStorageService.GetItemAsync<DraftBlogPostModel>(DraftStorageKey);
            return draft != null ? CreateNewModel.FromDraft(draft) : null;
        }
        catch
        {
            // If there's any error restoring the draft, just ignore it
            return null;
        }
    }

    public async ValueTask DiscardDraftAsync()
    {
        try
        {
            await localStorageService.RemoveItemAsync(DraftStorageKey);
        }
        catch
        {
            // Ignore errors
        }
    }

    public string GetTimeSinceAutoSave(DateTime? lastAutoSaveTime)
    {
        if (!lastAutoSaveTime.HasValue)
            return string.Empty;

        var elapsed = DateTime.UtcNow - lastAutoSaveTime.Value;
        if (elapsed.TotalMinutes < 1)
            return "just now";
        if (elapsed.TotalMinutes < 60)
            return $"{(int)elapsed.TotalMinutes}m ago";
        return $"{(int)elapsed.TotalHours}h ago";
    }
}
