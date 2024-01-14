using System;
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
using System.ComponentModel;

namespace C490_App.MVVM.ViewModel
{
    public class GraphFrameViewModel : ViewModelBase
    {
        private PlotModel plotModel;
        private ObservableCollection<CsvFileInfo> csvFilesInfo;
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

        public ObservableCollection<CsvFileInfo> CsvFilesInfo
        {
            get { return csvFilesInfo; }
            set
            {
                if (csvFilesInfo != value)
                {
                    csvFilesInfo = value;
                    OnPropertyChanged(nameof(CsvFilesInfo));
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
                    PlotCsvFile(SelectedCsvFileName);
                }
            }
        }

        public ICommand LoadSelectedCsvCommand { get; private set; }
        public ICommand ToggleCsvVisibilityCommand { get; private set; }

        public GraphFrameViewModel()
        {
            LoadCsvFileNames();
            InitializePlotModel();
            LoadSelectedCsvCommand = new RelayCommand(ExecuteLoadSelectedCsv, CanExecuteLoadSelectedCsv);
            ToggleCsvVisibilityCommand = new RelayCommand(ExecuteToggleCsvVisibility, CanExecuteToggleCsvVisibility);
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

            CsvFilesInfo = new ObservableCollection<CsvFileInfo>(
                csvFiles.Select(csvFile => new CsvFileInfo { FileName = Path.GetFileNameWithoutExtension(csvFile), IsVisible = false })
            );

            foreach (var fileInfo in CsvFilesInfo)
            {
                fileInfo.PropertyChanged += CsvFileInfo_PropertyChanged;
            }
        }

        private void CsvFileInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CsvFileInfo.IsVisible))
            {
                ReloadVisibleCsvFiles();
            }
        }

        private void ExecuteLoadSelectedCsv(object parameter)
        {
            // Handle the execution of the LoadSelectedCsvCommand
            PlotCsvFile(SelectedCsvFileName);
        }

        private bool CanExecuteLoadSelectedCsv(object parameter)
        {
            // Can add conditions for whether the command can be executed or not
            return !string.IsNullOrEmpty(SelectedCsvFileName);
        }

        private void ExecuteToggleCsvVisibility(object parameter)
        {
            // Handle the execution of the ToggleCsvVisibilityCommand
            if (parameter is CsvFileInfo csvFileInfo)
            {
                csvFileInfo.IsVisible = !csvFileInfo.IsVisible;
            }
        }

        private bool CanExecuteToggleCsvVisibility(object parameter)
        {
            // Can add conditions for whether the command can be executed or not
            return parameter is CsvFileInfo;
        }

        private void ReloadVisibleCsvFiles()
        {
            // Clear existing series in the plot
            PlotModel.Series.Clear();

            // Add visible CSV files to the plot
            foreach (var csvFileInfo in CsvFilesInfo.Where(info => info.IsVisible))
            {
                PlotCsvFile(csvFileInfo.FileName);
            }

            // Refresh the plot
            PlotModel.InvalidatePlot(true);
        }

        public void PlotCsvFile(string csvFileName)
        {
            string appFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSV Readings");
            string csvFilePath = Path.Combine(appFolder, $"{csvFileName}.csv");

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CsvReadingsModel>();

                var lineSeries = new LineSeries
                {
                    Title = csvFileName,
                    MarkerType = MarkerType.Circle,
                    
                };

                foreach (var record in records)
                {
                    lineSeries.Points.Add(new DataPoint(record.X, record.Y));
                }

                PlotModel.Series.Add(lineSeries);
                
            }
        }
    }

    public class CsvFileInfo : ViewModelBase
    {
        private bool isVisible;

        public string FileName { get; set; }

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (isVisible != value)
                {
                    isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }
    }
}
