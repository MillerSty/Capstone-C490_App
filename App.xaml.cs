﻿using C490_App.Core;
using C490_App.MVVM.View;
using C490_App.MVVM.ViewModel;
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
        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<HomeViewModel>(); //might be neccesary to delete

            services.AddSingleton<HomeView>(provider => new HomeView
            {
                DataContext = provider.GetRequiredService<HomeViewModel>()
            });

            services.AddSingleton<ExperimentStore>();


            _serviceProvider = services.BuildServiceProvider();
            var ExperimentStore = _serviceProvider.GetService<ExperimentStore>();

            ExperimentStore.serialPortWrapper = new SerialPortWrapper(new System.IO.Ports.SerialPort());
            ExperimentStore.serialPortWrapper.initSerial();

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            var k = _serviceProvider.GetRequiredService<ExperimentStore>();
            var viewmodel = new HomeViewModel(k);
            var mainWindow = _serviceProvider.GetRequiredService<HomeView>();
            mainWindow.DataContext = viewmodel;
            mainWindow.Show();

            base.OnStartup(e);
        }


    }

}
