using Microsoft.Extensions.DependencyInjection;

namespace MauiAI.ChatUI.Internal;

public sealed class ChatUIBuilder
{
    public ChatUIBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public IServiceCollection Services { get; }
}
