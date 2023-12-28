using C490_App.Core;

namespace C490_App.Services
{
    public interface INavigationService
    {
        ViewModelBase CurrentView { get; }

        void NavigateTo<T>() where T : ViewModelBase;
    }

    public class NavigationService : ObservableObject, INavigationService
    {
        public Func<Type, ViewModelBase> _viewModelFactory { get; }


        //public Func<Type, Window> _viewFrameFactory { get; }
        private ViewModelBase _currentView;
        public ViewModelBase CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }



        public NavigationService(Func<Type, ViewModelBase> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {

            var check = typeof(TViewModel);
            // Window check = _viewModelFactory.Invoke(typeof(Window));
            ViewModelBase viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
            CurrentView = viewModel;
        }
        //public void NavigateToWindow<TWindow>() where TWindow : Window
        //{

        //    var check = typeof(TWindow);
        //    // Window check = _viewModelFactory.Invoke(typeof(Window));
        //    ViewModelBase viewModel = _viewModelFactory.Invoke(typeof(TWindow));
        //    CurrentView = viewModel;
        //}
    }
}