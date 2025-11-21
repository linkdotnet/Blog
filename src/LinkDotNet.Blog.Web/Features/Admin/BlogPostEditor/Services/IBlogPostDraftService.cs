using System;
using System.Threading.Tasks;
using LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Components;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;

public interface IBlogPostDraftService
{
    /// <summary>
    /// Checks if a saved draft exists in local storage.
    /// </summary>
    /// <returns>A tuple containing whether a draft exists and the saved time as a formatted string.</returns>
    ValueTask<(bool exists, string savedTime)> CheckForSavedDraftAsync();

    /// <summary>
    /// Saves a draft to local storage.
    /// </summary>
    /// <param name="model">The blog post model to save as a draft.</param>
    ValueTask SaveDraftAsync(CreateNewModel model);

    /// <summary>
    /// Restores a saved draft from local storage.
    /// </summary>
    /// <returns>The restored CreateNewModel, or null if no draft exists or an error occurred.</returns>
    ValueTask<CreateNewModel?> RestoreDraftAsync();

    /// <summary>
    /// Discards the saved draft from local storage.
    /// </summary>
    ValueTask DiscardDraftAsync();

    /// <summary>
    /// Gets the time elapsed since the last auto-save.
    /// </summary>
    /// <param name="lastAutoSaveTime">The time of the last auto-save.</param>
    /// <returns>A formatted string representing the elapsed time.</returns>
    string GetTimeSinceAutoSave(DateTime? lastAutoSaveTime);
}
