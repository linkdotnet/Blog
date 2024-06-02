using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LinkDotNet.Blog.Web.Features.Admin.BlogPostEditor.Services;

public class AutocompleteService
{
    private readonly IChatCompletionService chatCompletionService;

    public AutocompleteService(IChatCompletionService chatCompletionService)
    {
        this.chatCompletionService = chatCompletionService;
    }

    public async IAsyncEnumerable<string> GetAutocomplete(AutocompleteOptions options, [EnumeratorCancellation] CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(options);

        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage(GenerateSystemMessage(options));
        chatHistory.AddUserMessage(options.UserInput);
        await foreach (var part in chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory, cancellationToken: token))
        {
            yield return part.Content;
        }
    }

    private static string GenerateSystemMessage(AutocompleteOptions options)
    {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(options.Title))
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"Title: {options.Title}");
        }

        if (!string.IsNullOrEmpty(options.Description))
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"Description: {options.Description}");
        }

        if (!string.IsNullOrEmpty(options.Tags))
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"Tags: {options.Tags}");
        }

        if (!string.IsNullOrEmpty(options.Content))
        {
            sb.AppendLine(CultureInfo.InvariantCulture, $"Content: {options.Content}");
        }

        sb.AppendLine(options.ReplaceContent
            ? "You are allowed to replace the Content"
            : "You are not allowed to replace the Content. Only append.");

        return sb.ToString();
    }
}

public record AutocompleteOptions(
    string Title,
    string Description,
    string Tags,
    string Content,
    string UserInput,
    bool ReplaceContent);
