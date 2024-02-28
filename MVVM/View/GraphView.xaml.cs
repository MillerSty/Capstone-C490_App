using C490_App.MVVM.ViewModel;
using System.Windows;

namespace C490_App.MVVM.View
{
    public partial class GraphView : Window
    {
        private GraphViewModel viewModel;

        private HomeViewModel homeViewModel;

        public GraphView(HomeViewModel homeViewModel)
        {
            InitializeComponent();

            // Initialize GraphViewModel with HomeViewModel instance
            viewModel = new GraphViewModel(homeViewModel);

            DataContext = viewModel;
        }
    }
}