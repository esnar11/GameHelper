using System;
using System.Net.Http;
using System.Windows;
using GameHelper.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GameHelper
{
    public partial class App
    {
        public static ITranslateService TranslateService { get; private set; }

        public static IChangeKeyboardLayoutService ChangeKeyboardLayoutService { get; } // TODO

        public static void ShowError(Exception error)
        {
            var message = error.Message;
            if (error.GetBaseException() != error)
                message += Environment.NewLine + Environment.NewLine + error.GetBaseException().Message;
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public App()
        {
            var hostBuilder = new HostBuilder();
            hostBuilder.ConfigureServices(ConfigureServices);
            var host = hostBuilder.Build();
            host.Services.GetService<IHttpClientFactory>();

            TranslateService = null; // TODO
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //services
            //    .AddHttpClient<DeeplTranslateService>()
            //    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseDefaultCredentials = true });
        }
    }
}
