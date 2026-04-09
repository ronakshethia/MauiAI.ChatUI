using System.Collections.ObjectModel;
using System.Text;
using MauiAI.ChatUI.Abstractions;
using MauiAI.ChatUI.Models;

namespace MauiAI.ChatUI.Services;

public class ChatService
{
    private readonly IChatProvider _chatProvider;

    public ChatService(IChatProvider chatProvider)
    {
        _chatProvider = chatProvider;
    }

    public async Task SendAsync(ObservableCollection<ChatMessage> messages, string input)
    {
        ArgumentNullException.ThrowIfNull(messages);

        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        var trimmedInput = input.Trim();
        var userMessage = new ChatMessage
        {
            Content = trimmedInput,
            IsUser = true
        };

        var assistantMessage = new ChatMessage
        {
            Content = string.Empty,
            IsUser = false
        };

        List<ChatMessage> requestMessages = [];

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            requestMessages = messages
                .Select(message => new ChatMessage
                {
                    Content = message.Content,
                    IsUser = message.IsUser
                })
                .ToList();

            messages.Add(userMessage);
            messages.Add(assistantMessage);
            requestMessages.Add(new ChatMessage
            {
                Content = userMessage.Content,
                IsUser = true
            });
        });

        var builder = new StringBuilder();

        try
        {
            await foreach (var chunk in _chatProvider.StreamAsync(requestMessages))
            {
                if (string.IsNullOrEmpty(chunk))
                {
                    continue;
                }

                builder.Append(chunk);
                var updatedContent = builder.ToString();
                await MainThread.InvokeOnMainThreadAsync(() => assistantMessage.Content = updatedContent);
            }

            if (builder.Length == 0)
            {
                await MainThread.InvokeOnMainThreadAsync(() => assistantMessage.Content = "No response was returned.");
            }
        }
        catch (Exception ex)
        {
            var errorMessage = $"I couldn't complete the request: {ex.Message}";
            await MainThread.InvokeOnMainThreadAsync(() => assistantMessage.Content = errorMessage);
        }
    }
}
