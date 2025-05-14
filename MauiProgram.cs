using Microsoft.Extensions.Logging;
using VSmauiApp.Controls;
using SkiaSharp.Views.Maui.Controls.Hosting;
using LiveChartsCore.SkiaSharpView.Maui;

namespace VSmauiApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // github chart
            // https://www.nuget.org/packages/LiveChartsCore.SkiaSharpView.Maui
            // link: https://livecharts.dev/docs/maui/2.0.0-rc5.4/gallery

            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp()
                .UseLiveCharts()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            FormHandler.RemoveBorders();
            return builder.Build();
        }
    }
}
