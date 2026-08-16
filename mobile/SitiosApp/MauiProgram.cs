using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;

namespace SitiosApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiMaps()               // habilita Microsoft.Maui.Controls.Maps
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddSingleton(AudioManager.Current); // Plugin.Maui.Audio



            return builder.Build();
        }
    }
}
