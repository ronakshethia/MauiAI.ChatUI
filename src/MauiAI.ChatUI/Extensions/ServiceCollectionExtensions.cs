using MauiAI.ChatUI.Abstractions;
using MauiAI.ChatUI.Internal;
using MauiAI.ChatUI.Providers;
using MauiAI.ChatUI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MauiAI.ChatUI.Extensions;

public static class ServiceCollectionExtensions
{
    public static ChatUIBuilder AddChatUI(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<ChatService>();
        services.TryAddSingleton<HttpClient>();

        return new ChatUIBuilder(services);
    }

    public static ChatUIBuilder AddOpenAI(this ChatUIBuilder builder, string apiKey, string? model = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddSingleton(new OpenAIOptions
        {
            ApiKey = apiKey ?? string.Empty,
            Model = string.IsNullOrWhiteSpace(model) ? OpenAIOptions.DefaultModel : model
        });

        builder.Services.AddSingleton<IChatProvider, OpenAIProvider>();
        return builder;
    }
}
