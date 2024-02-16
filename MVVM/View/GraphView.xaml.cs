using C490_App.MVVM.ViewModel;
using System.Windows;

namespace C490_App.MVVM.View
{
    public partial class GraphView : Window
    {
        private GraphViewModel viewModel;

        public GraphView()
        {
            InitializeComponent();
            viewModel = new GraphViewModel();
            DataContext = viewModel;
        }
    }
}