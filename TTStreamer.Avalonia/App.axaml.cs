using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using Microsoft.Extensions.DependencyInjection;

using TTStreamer.Common.Extensions;
using TTStreamer.Models;
using TTStreamer.Views;

namespace TTStreamer
{
    public class App : Application
    {
        private static ServiceProvider serviceProvider;
        public static T GetService<T>() where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T;
        }

        public override void Initialize()
        {
            ServiceCollection services = new ServiceCollection();
            services.AddSingleton<SoundsWindow>();
            services.AddSingleton<SoundsView>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainView>();
            services.AddAppServices();

            serviceProvider = services.BuildServiceProvider();

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) desktop.MainWindow = GetService<MainWindow>();
            base.OnFrameworkInitializationCompleted();
        }
    }
}