using MauiAI.ChatUI.Models;

namespace MauiAI.ChatUI.Abstractions;

public interface IChatProvider
{
    IAsyncEnumerable<string> StreamAsync(List<ChatMessage> messages);
}
