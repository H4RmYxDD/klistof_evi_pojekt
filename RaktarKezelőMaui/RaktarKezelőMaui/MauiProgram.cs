//TODO: mukodik a rendeles, kell meg:
//rendeles osszege (keszen van), statisztika (ez is megvan) , tobb termeket tudjon rendelni pipa, termeket kategoriahoz tudjunk adni pipa
//mi a gyasz az a "d" betu a rendelesnel (megcsinaltam elvileg! puszi, balazs <3)

using System;
using DataBase;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace RaktarKezelőMaui
{
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
            builder.Services.AddDbContext<ApplicationDbContext>();
            builder.Services.AddScoped<DeliveryRepository>();
            builder.Services.AddTransient<DeliveryViewModel>();
#endif
            builder.UseMauiApp<App>().UseSkiaSharp();
            return builder.Build();
        }
    }
}
