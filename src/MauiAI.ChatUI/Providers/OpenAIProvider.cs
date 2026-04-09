using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MauiAI.ChatUI.Abstractions;
using MauiAI.ChatUI.Internal;
using MauiAI.ChatUI.Models;

namespace MauiAI.ChatUI.Providers;

internal sealed class OpenAIProvider : IChatProvider
{
    private static readonly Uri ChatCompletionsUri = new("https://api.openai.com/v1/chat/completions");
    private readonly HttpClient _httpClient;
    private readonly OpenAIOptions _options;

    public OpenAIProvider(HttpClient httpClient, OpenAIOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async IAsyncEnumerable<string> StreamAsync(List<ChatMessage> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);

        if (string.IsNullOrWhiteSpace(_options.ApiKey) || _options.ApiKey.Contains("PASTE_OPENAI_API_KEY_HERE", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Configure a valid OpenAI API key with builder.Services.AddChatUI().AddOpenAI(\"YOUR_API_KEY\").");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, ChatCompletionsUri);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
        request.Content = new StringContent(CreateRequestPayload(messages), Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new InvalidOperationException($"OpenAI returned {(int)response.StatusCode}: {GetErrorMessage(error)}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        using var reader = new StreamReader(stream);

        while (true)
        {
            var line = await reader.ReadLineAsync().ConfigureAwait(false);
            if (line is null)
            {
                yield break;
            }

            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:", StringComparison.Ordinal))
            {
                continue;
            }

            var payload = line["data:".Length..].Trim();
            if (payload.Equals("[DONE]", StringComparison.Ordinal))
            {
                yield break;
            }

            var chunk = ExtractContent(payload);
            if (!string.IsNullOrEmpty(chunk))
            {
                yield return chunk;
            }
        }
    }

    private string CreateRequestPayload(List<ChatMessage> messages)
    {
        var payload = new
        {
            model = string.IsNullOrWhiteSpace(_options.Model) ? OpenAIOptions.DefaultModel : _options.Model,
            stream = true,
            messages = messages.Select(message => new
            {
                role = message.IsUser ? "user" : "assistant",
                content = message.Content
            })
        };

        return JsonSerializer.Serialize(payload);
    }

    private static string? ExtractContent(string payload)
    {
        using var document = JsonDocument.Parse(payload);
        if (!document.RootElement.TryGetProperty("choices", out var choices) || choices.GetArrayLength() == 0)
        {
            return null;
        }

        var delta = choices[0].GetProperty("delta");
        if (!delta.TryGetProperty("content", out var contentElement))
        {
            return null;
        }

        return contentElement.ValueKind == JsonValueKind.String
            ? contentElement.GetString()
            : null;
    }

    private static string GetErrorMessage(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return "The API returned an empty error response.";
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            if (document.RootElement.TryGetProperty("error", out var errorElement) &&
                errorElement.TryGetProperty("message", out var messageElement) &&
                messageElement.ValueKind == JsonValueKind.String)
            {
                return messageElement.GetString() ?? responseBody;
            }
        }
        catch (JsonException)
        {
        }

        return responseBody;
    }
}
