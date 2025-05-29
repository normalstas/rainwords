using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Plugin.Maui.Audio;

namespace rainwords
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();

			builder
				.UseMauiApp<App>()
				.UseMauiCommunityToolkit() // Активируем CommunityToolkit
				.ConfigureFonts(fonts => fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));
			// Регистрируем AudioManager и наш AudioService
			builder.Services.AddSingleton(AudioManager.Current); // Plugin.Maui.Audio
			builder.Services.AddSingleton<IAudioService, AudioServiceTwo>();

			// Регистрируем все страницы, где будет музыка
			builder.Services.AddTransient<MainPage>();
			builder.Services.AddTransient<Menu>();
			builder.Services.AddTransient<Settings>();

			return builder.Build();
		}
	}
}
