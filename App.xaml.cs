using C490_App.MVVM.View;
using C490_App.MVVM.ViewModel;
using C490_App.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace C490_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 

    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly TestStore _testStore;
        public App()
        {
            IServiceCollection services = new ServiceCollection();
            _testStore = new TestStore();

            services.AddSingleton<INavigationService, NavigationService>();

            services.AddSingleton<HomeFrameViewModel>();

            services.AddSingleton<HomeFrame>(provider => new HomeFrame
            {
                DataContext = provider.GetRequiredService<HomeFrameViewModel>()
            });
            //services.AddSingleton<LEDParameterFrame>(provider => new LEDParameterFrame
            //{
            //    DataContext = provider.GetRequiredService<LEDParameterViewModel>()
            //});

            services.AddSingleton<ExperimentStore>();

            //services.AddSingleton<INavigationService, NavigationService>();

            _serviceProvider = services.BuildServiceProvider();


        }
        protected override void OnStartup(StartupEventArgs e)
        {

            //HomeFrame homeFrame = new HomeFrame()
            //{
            //    DataContext = new HomeFrameViewModel(i)
            //};
            var k = _serviceProvider.GetRequiredService<ExperimentStore>();
            var viewmodel = new HomeFrameViewModel(k);
            var mainWindow = _serviceProvider.GetRequiredService<HomeFrame>();
            mainWindow.DataContext = viewmodel;
            mainWindow.Show();


            //homeFrame.Show();
            // Navigation.NavigateTo<LEDParameterViewModel>()

            //var service = _serviceProvider.GetRequiredService<INavigationService>();
            //service.NavigateTo<HomeFrameViewModel>();
            base.OnStartup(e);
        }


    }

}
