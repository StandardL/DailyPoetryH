using DailyPoetryH.Data;
using DailyPoetryH.Library.Services;
using DailyPoetryH.Services;
using Microsoft.Extensions.Logging;

namespace DailyPoetryH;
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
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddBootstrapBlazor();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif
        // Add app services
        builder.Services.AddSingleton<WeatherForecastService>();
        builder.Services.AddScoped<IPoetryStorage, PoetryStorage>();
        builder.Services.AddScoped<IPreferenceStorage, PreferenceStorage>();

        return builder.Build();
    }
}
