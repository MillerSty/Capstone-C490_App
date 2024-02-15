using C490_App.Core;
using C490_App.MVVM.Model;
using CsvHelper;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace C490_App.MVVM.ViewModel
{
    public class GraphFrameViewModel : ViewModelBase
    {
        public PlotModel plotModel;
        private ObservableCollection<CsvFileInfo> csvFilesInfo;
        private string selectedCsvFileName;
        private bool isSelectAllChecked;
        private MarkerType selectedMarkerType = MarkerType.Circle;

        public MarkerType SelectedMarkerType
        {
            get { return selectedMarkerType; }
            set
            {
                if (selectedMarkerType != value)
                {
                    selectedMarkerType = value;
                    OnPropertyChanged(nameof(SelectedMarkerType));
                }
            }
        }
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

        public RelayCommand LoadSelectedCsv { get; set; }
        public RelayCommand ToggleCsvVisibility { get; set; }
        public RelayCommand SelectAll { get; private set; }
        public RelayCommand ResetSelectedCheckboxes { get; set; }
        public RelayCommand ResetPlotAxes { get; set; }
        public RelayCommand MarkerTypeClick { get; set; }

        public GraphFrameViewModel()
        {
            LoadCsvFileNames();
            InitializePlotModel();
            LoadSelectedCsv = new RelayCommand(o => ExecuteLoadSelectedCsv(o), o => true);
            ToggleCsvVisibility = new RelayCommand(o => ExecuteToggleCsvVisibility(o), o => true);
            SelectAll = new RelayCommand(o => ExecuteSelectAll(o), o => true);
            ResetSelectedCheckboxes = new RelayCommand(o => ExecuteResetSelectedCheckboxes(o), o => true);
            ResetPlotAxes = new RelayCommand(o => ExecuteResetPlotAxes(o), o => true);
            MarkerTypeClick = new RelayCommand(o => ExecuteMarkerTypeClick(o), o => true);
        }

        private void ExecuteMarkerTypeClick(object o)
        {
            string menuItemName = o as string;

            switch (menuItemName)
            {
                case "Circle":
                    SelectedMarkerType = MarkerType.Circle;
                    break;
                case "Diamond":
                    SelectedMarkerType = MarkerType.Diamond;
                    break;
                case "Square":
                    SelectedMarkerType = MarkerType.Square;
                    break;
                case "Triangle":
                    SelectedMarkerType = MarkerType.Triangle;
                    break;
                case "None":
                    SelectedMarkerType = MarkerType.None;
                    break;
                default:
                    break;
            }
            ReloadVisibleCsvFiles();

        }

        private void ExecuteResetPlotAxes(object o)
        {
            if (plotModel != null)
            {
                plotModel.ResetAllAxes();
                PlotModel.InvalidatePlot(true);
            }

        }

        private void ExecuteSelectAll(object o)
        {
            // Handle the "Select All" checkbox state change
            IsSelectAllChecked = !IsSelectAllChecked;

            if (IsSelectAllChecked)
            {
                foreach (var fileInfo in CsvFilesInfo)
                {
                    fileInfo.IsVisible = true;
                }
            }
            else
            {
                foreach (var fileInfo in CsvFilesInfo)
                {
                    fileInfo.IsVisible = false;
                }
            }
        }

        public bool IsSelectAllChecked
        {
            get { return isSelectAllChecked; }
            set
            {
                if (isSelectAllChecked != value)
                {
                    isSelectAllChecked = value;
                    OnPropertyChanged(nameof(IsSelectAllChecked));

                    // Handle the "Select All" checkbox state change
                    if (value)
                    {
                        foreach (var fileInfo in CsvFilesInfo)
                        {
                            fileInfo.IsVisible = true;
                        }
                    }
                    else
                    {
                        foreach (var fileInfo in CsvFilesInfo)
                        {
                            fileInfo.IsVisible = false;
                        }
                    }

                }
            }
        }

        private void ExecuteResetSelectedCheckboxes(object o)
        {
            isSelectAllChecked = false;
            OnPropertyChanged(nameof(IsSelectAllChecked));
            foreach (var csvFileInfo in CsvFilesInfo)
            {
                csvFileInfo.IsVisible = false;
            }
        }


        private void ExecuteLoadSelectedCsv(object o)
        {
            // Handle the execution of the LoadSelectedCsvCommand
            PlotCsvFile(SelectedCsvFileName);
        }

        private void ExecuteToggleCsvVisibility(object o)
        {
            // Handle the execution of the ToggleCsvVisibilityCommand
            if (o is CsvFileInfo csvFileInfo)
            {
                csvFileInfo.IsVisible = !csvFileInfo.IsVisible;
            }
        }

        // Dictionnary for file string colours
        private Dictionary<string, OxyColor> fileColors = new Dictionary<string, OxyColor>();
        private Random rand = new Random();

        private void InitializePlotModel()
        {
            PlotModel = new PlotModel();

            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X-Axis" });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y-Axis" });
        }

        private void LoadCsvFileNames()
        {
            string appFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSV Readings");

            try
            {
                string[] csvFiles = Directory.GetFiles(appFolder, "*.csv");

                CsvFilesInfo = new ObservableCollection<CsvFileInfo>(
                    csvFiles.Select(csvFile => new CsvFileInfo { FileName = Path.GetFileNameWithoutExtension(csvFile), IsVisible = false })
                );

                foreach (var fileInfo in CsvFilesInfo)
                {
                    fileInfo.PropertyChanged += CsvFileInfo_PropertyChanged;
                }
            }
            catch (Exception ex) { MessageBox.Show("No files available"); }
        }

        private void CsvFileInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CsvFileInfo.IsVisible))
            {
                ReloadVisibleCsvFiles();
            }
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
                    MarkerType = SelectedMarkerType,
                };

                // Use existing color or generate a new one and store it
                if (!fileColors.TryGetValue(csvFileName, out OxyColor seriesColor))
                {
                    seriesColor = OxyColor.FromRgb((byte)rand.Next(0, 256), (byte)rand.Next(0, 256), (byte)rand.Next(0, 256));
                    fileColors.Add(csvFileName, seriesColor);
                }

                lineSeries.Color = seriesColor;

                var fileInfo = CsvFilesInfo.FirstOrDefault(info => info.FileName == csvFileName);
                if (fileInfo != null)
                {
                    fileInfo.Color = seriesColor;
                }


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
        private OxyColor color;
        private SolidColorBrush wpfColor;

        public SolidColorBrush WpfColor
        {
            get { return wpfColor; }
            set
            {
                if (wpfColor != value)
                {
                    wpfColor = value;
                    OnPropertyChanged(nameof(WpfColor));
                }
            }
        }

        public OxyColor Color
        {
            get { return color; }
            set
            {
                if (color != value)
                {
                    color = value;
                    WpfColor = new SolidColorBrush(OxyColor.FromRgb(color.R, color.G, color.B).ToColor());
                    OnPropertyChanged(nameof(Color));
                }
            }
        }

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
