using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace LinkDotNet.Blog.Web.Fakes;

internal sealed class FakeCompletionService : IChatCompletionService
{
    public IReadOnlyDictionary<string, object> Attributes { get; }

    public Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings executionSettings = null,
        Kernel kernel = null, CancellationToken cancellationToken = new CancellationToken()) =>
        throw new NotImplementedException();

    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(ChatHistory chatHistory,
        PromptExecutionSettings executionSettings = null, Kernel kernel = null,
        CancellationToken cancellationToken = new CancellationToken()) =>
        throw new NotImplementedException();
}
