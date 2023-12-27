using C490_App.Core;
using C490_App.MVVM.Model;
using C490_App.MVVM.View;
using C490_App.MVVM.ViewModel;
using C490_App.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;

namespace C490_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        private readonly ServiceProvider _serviceProvider;
        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<HomeFrame>(provider => new HomeFrame
            {
                DataContext = provider.GetRequiredService<HomeFrameViewModel>()
            });
            services.AddSingleton<ObservableCollection<LEDParameter>>();


            services.AddSingleton<ExperimentService>();

            //services.AddSingleton<LEDParameterFrame>(provider => new LEDParameterFrame
            //{
            //    DataContext = provider.GetRequiredService<LEDParameterViewModel>()
            //});
            services.AddSingleton<LEDParameterViewModel>();
            services.AddSingleton<LEDParameterFrame>();
            services.AddSingleton<HomeFrameViewModel>();


            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<Func<Type, ViewModelBase>>(serviceProvider => viewModelType => (ViewModelBase)serviceProvider.GetRequiredService(viewModelType));
            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            HomeFrame homeFrame = new HomeFrame();
            var mainWindow = _serviceProvider.GetRequiredService<HomeFrame>();
            mainWindow.Show();
            //homeFrame.Show();
            // Navigation.NavigateTo<LEDParameterViewModel>()

            //var service = _serviceProvider.GetRequiredService<INavigationService>();
            //service.NavigateTo<HomeFrameViewModel>();
            base.OnStartup(e);
        }



    }
}
