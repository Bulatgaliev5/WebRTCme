using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using WebRTCme.Middleware;
using Xamarinme;
namespace WebRTCme.DemoApp.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			})
			.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler(typeof(Media), typeof(MediaHandler));
			});

		builder.Configuration.AddConfiguration(
			new ConfigurationBuilder()
			.AddEmbeddedResource(new EmbeddedResourceConfigurationOptions
			{
                Assembly = Assembly.GetExecutingAssembly(),
                Prefix = "WebRTCme.DemoApp.Maui"
            })
			.Build());


        var webRtcMiddleware = CrossWebRtcMiddlewareMaui.Current;

        builder.Services.AddSingleton(webRtcMiddleware.WebRtc);
        builder.Services.AddSingleton(webRtcMiddleware);

        builder.Services.AddMauiMiddleware();

#if DEBUG
        // ТОЛЬКО для разработки - отключаем проверку SSL
        System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            (sender, certificate, chain, sslPolicyErrors) => true;

        // Или через HttpClientHandler
        builder.Services.AddSingleton(sp =>
        {
            var handler = new HttpClientHandler();
#if ANDROID
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                // Разрешаем для локального IP
                if (cert != null && cert.Issuer.Contains("http://37.252.23.169:5053"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
#endif
            return new HttpClient(handler);
        });
#endif

        return builder.Build();
	}
}
