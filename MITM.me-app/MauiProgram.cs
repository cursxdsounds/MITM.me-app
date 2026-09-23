using Microsoft.Extensions.Logging;
using MITM.me_app.SettingsService;
using MITM.me_app.ViewModels;

namespace MITM.me_app;

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
        builder.Services.AddSingleton<SettingsService.Interfaces.ISettingsService, SettingsService.SettingsService>();
        builder.Services.AddSingleton<ChatViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}