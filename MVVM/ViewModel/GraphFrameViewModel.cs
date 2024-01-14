using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using C490_App.Core;
using C490_App.MVVM.Model;
using CsvHelper;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace C490_App.MVVM.ViewModel
{
    public class GraphFrameViewModel : ViewModelBase
    {
        private PlotModel plotModel;
        private ObservableCollection<string> csvFileNames;
        private string selectedCsvFileName;

        public PlotModel PlotModel
        {
            get { return plotModel; }
            set
            {
                if (plotModel != value)
                {
                    plotModel = value;
                    OnPropertyChanged(nameof(PlotModel));
                }
            }
        }

        public ObservableCollection<string> CsvFileNames
        {
            get { return csvFileNames; }
            set
            {
                if (csvFileNames != value)
                {
                    csvFileNames = value;
                    OnPropertyChanged(nameof(CsvFileNames));
                }
            }
        }

        public string SelectedCsvFileName
        {
            get { return selectedCsvFileName; }
            set
            {
                if (selectedCsvFileName != value)
                {
                    selectedCsvFileName = value;
                    OnPropertyChanged(nameof(SelectedCsvFileName));

                    // Load and plot the selected CSV file
                    PlotCsvFile(selectedCsvFileName);
                }
            }
        }

        public ICommand LoadSelectedCsvCommand { get; private set; }

        public GraphFrameViewModel()
        {
            LoadCsvFileNames();
            InitializePlotModel();
            LoadSelectedCsvCommand = new RelayCommand(ExecuteLoadSelectedCsv, CanExecuteLoadSelectedCsv);
        }

        private void InitializePlotModel()
        {
            PlotModel = new PlotModel();

            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X-Axis" });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y-Axis" });
        }

        private void LoadCsvFileNames()
        {
            string appFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSV Readings");

            string[] csvFiles = Directory.GetFiles(appFolder, "*.csv");

            CsvFileNames = new ObservableCollection<string>(csvFiles.Select(csvFile => Path.GetFileNameWithoutExtension(csvFile)));
        }

        private void ExecuteLoadSelectedCsv(object parameter)
        {
            // Handle the execution of the LoadSelectedCsvCommand
            PlotCsvFile(SelectedCsvFileName);
        }

        private bool CanExecuteLoadSelectedCsv(object parameter)
        {
            // You can add conditions for whether the command can be executed or not
            return !string.IsNullOrEmpty(SelectedCsvFileName);
        }

        public void PlotCsvFile(string csvFileName)
        {
            string appFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSV Readings");
            string csvFilePath = Path.Combine(appFolder, $"{csvFileName}.csv");

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CsvReadingsModel>();

                var lineSeries = new LineSeries();

                foreach (var record in records)
                {
                    lineSeries.Points.Add(new DataPoint(record.X, record.Y));
                }

                PlotModel.Series.Add(lineSeries);
                PlotModel.InvalidatePlot(true);
            }
        }
    }
}