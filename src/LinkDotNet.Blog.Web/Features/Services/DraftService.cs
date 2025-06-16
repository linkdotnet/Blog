using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkDotNet.Blog.Web.Features.Services;

public sealed class DraftService : IDraftService
{
    private readonly ILocalStorageService localStorage;
    private const string DraftIndexKey = "blogpost_drafts_index";
    private const string DraftPrefix = "blogpost_draft_";

    public DraftService(ILocalStorageService localStorage)
    {
        this.localStorage = localStorage;
    }

    public async ValueTask SaveDraftAsync(BlogPostDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        draft.LastSavedAt = DateTime.UtcNow;

        var draftKey = GetDraftKey(draft.Id);
        await localStorage.SetItemAsync(draftKey, draft);

        await UpdateDraftIndexAsync(draft.Id);
    }

    public async ValueTask<BlogPostDraft?> GetDraftAsync(string draftId)
    {
        if (string.IsNullOrWhiteSpace(draftId))
            return null;

        try
        {
            var draftKey = GetDraftKey(draftId);
            if (!await localStorage.ContainsKeyAsync(draftKey))
                return null;

            return await localStorage.GetItemAsync<BlogPostDraft>(draftKey);
        }
        catch
        {
            await RemoveFromDraftIndexAsync(draftId);
            return null;
        }
    }

    public async ValueTask<IReadOnlyList<BlogPostDraft>> GetAllDraftsAsync()
    {
        var draftIds = await GetDraftIndexAsync();
        var drafts = new List<BlogPostDraft>();

        foreach (var draftId in draftIds)
        {
            var draft = await GetDraftAsync(draftId);
            if (draft != null)
            {
                drafts.Add(draft);
            }
        }

        return drafts.OrderByDescending(d => d.LastSavedAt).ToArray();
    }

    public async ValueTask<IReadOnlyList<BlogPostDraft>> GetDraftsForBlogPostAsync(string blogPostId)
    {
        if (string.IsNullOrWhiteSpace(blogPostId))
            return [];

        var allDrafts = await GetAllDraftsAsync();
        return allDrafts
            .Where(d => d.DraftType == DraftType.Edit && d.BlogPostId == blogPostId)
            .ToArray();
    }

    public async ValueTask<IReadOnlyList<BlogPostDraft>> GetNewPostDraftsAsync()
    {
        var allDrafts = await GetAllDraftsAsync();
        return allDrafts
            .Where(d => d.DraftType == DraftType.New)
            .ToArray();
    }

    public async ValueTask DeleteDraftAsync(string draftId)
    {
        if (string.IsNullOrWhiteSpace(draftId))
            return;

        var draftKey = GetDraftKey(draftId);

        if (await localStorage.ContainsKeyAsync(draftKey))
        {
            await localStorage.SetItemAsync<object?>(draftKey, null);
        }

        await RemoveFromDraftIndexAsync(draftId);
    }

    private static string GetDraftKey(string draftId) => $"{DraftPrefix}{draftId}";

    private async ValueTask<List<string>> GetDraftIndexAsync()
    {
        try
        {
            if (!await localStorage.ContainsKeyAsync(DraftIndexKey))
                return [];

            return await localStorage.GetItemAsync<List<string>>(DraftIndexKey);
        }
        catch
        {
            return [];
        }
    }

    private async ValueTask UpdateDraftIndexAsync(string draftId)
    {
        var index = await GetDraftIndexAsync();

        if (!index.Contains(draftId))
        {
            index.Add(draftId);
            await localStorage.SetItemAsync(DraftIndexKey, index);
        }
    }

    private async ValueTask RemoveFromDraftIndexAsync(string draftId)
    {
        var index = await GetDraftIndexAsync();

        if (index.Remove(draftId))
        {
            await localStorage.SetItemAsync(DraftIndexKey, index);
        }
    }
}
