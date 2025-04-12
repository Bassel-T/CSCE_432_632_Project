using Microsoft.Extensions.Logging;

namespace RemindMe
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
                    fonts.AddFont("AtkinsonHyperlegible-Regular.ttf", "AtkinsonHyperlegibleRegular");
                    fonts.AddFont("AtkinsonHyperlegible-Bold.ttf", "AtkinsonHyperlegibleBold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
