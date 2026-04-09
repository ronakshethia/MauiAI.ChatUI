namespace MauiAI.ChatUI.Internal;

internal sealed class OpenAIOptions
{
    public const string DefaultModel = "gpt-4.1-mini";

    public string ApiKey { get; init; } = string.Empty;

    public string Model { get; init; } = DefaultModel;
}
