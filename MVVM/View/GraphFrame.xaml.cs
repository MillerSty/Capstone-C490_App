using C490_App.MVVM.ViewModel;
using System.Windows;

namespace C490_App.MVVM.View
{
    public partial class GraphFrame : Window
    {
        private GraphViewModel viewModel;

        public GraphFrame()
        {
            InitializeComponent();
            viewModel = new GraphViewModel();
            DataContext = viewModel;
        }
    }
}