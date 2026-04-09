using Microsoft.Extensions.Logging;

using MauiAI.ChatUI.Extensions;

namespace MauiAI.ChatUI.Sample;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		const string fallbackApiKey = "PASTE_OPENAI_API_KEY_HERE";
		var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services
			.AddChatUI()
			.AddOpenAI(string.IsNullOrWhiteSpace(apiKey) ? fallbackApiKey : apiKey, model: "gpt-4.1-mini");

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
