using Microsoft.Extensions.Logging;

namespace UpworkPdfGenerator.Apps;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif
		
		builder.Services
			.AddTransient<MainPage>()
			.AddScoped<MainViewModel>()
			;
		
		builder.Services
			.AddSingleton<IFilePicker>(_ => FilePicker.Default)
			.AddSingleton<ILauncher>(_ => Launcher.Default)
			.AddSingleton<IPreferences>(_ => Preferences.Default)
			;

		return builder.Build();
	}
}
