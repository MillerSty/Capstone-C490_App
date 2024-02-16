using C490_App.Core;
using C490_App.MVVM.Model;
using Microsoft.Win32;
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
    public class GraphViewModel : ViewModelBase
    {
        public PlotModel plotModel;
        private ObservableCollection<PlotItem> csvListBox;
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

        public ObservableCollection<PlotItem> CsvListBox
        {
            get { return csvListBox; }
            set
            {
                if (csvListBox != value)
                {
                    csvListBox = value;
                    OnPropertyChanged(nameof(CsvListBox));
                }
            }
        }

        public RelayCommand LoadSelectedCsv { get; set; }
        public RelayCommand ToggleCsvVisibility { get; set; }
        public RelayCommand SelectAll { get; private set; }
        public RelayCommand ResetSelectedCheckboxes { get; set; }
        public RelayCommand ResetPlotAxes { get; set; }
        public RelayCommand MarkerTypeClick { get; set; }
        public RelayCommand ImportData { get; set; }
        public RelayCommand RandomizeColours { get; set; }
        public RelayCommand OpenRecent { get; set; }
        public GraphViewModel()
        {
            InitializePlotModel();
            CsvListBox = new ObservableCollection<PlotItem>();
            LoadSelectedCsv = new RelayCommand(o => ExecuteLoadSelectedCsv(o), o => true);
            ToggleCsvVisibility = new RelayCommand(o => ExecuteToggleCsvVisibility(o), o => true);
            SelectAll = new RelayCommand(o => ExecuteSelectAll(o), o => true);
            ResetSelectedCheckboxes = new RelayCommand(o => ExecuteResetSelectedCheckboxes(o), o => true);
            ResetPlotAxes = new RelayCommand(o => ExecuteResetPlotAxes(o), o => true);
            MarkerTypeClick = new RelayCommand(o => ExecuteMarkerTypeClick(o), o => true);
            ImportData = new RelayCommand(o => ExecuteImportData(o), o => true);
            RandomizeColours = new RelayCommand(o => ExecuteRandomizeColours(o), o => true);
            OpenRecent = new RelayCommand(o => ExecuteOpenRecent(o), o => true);
        }
        private void ExecuteOpenRecent(object o)
        {
            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSVDATA");

            // Get all CSV files in the directory
            string[] csvFiles = Directory.GetFiles(directoryPath, "*.csv");

            if (csvFiles.Length == 0)
            {
                MessageBox.Show("No CSV files found in the directory.");
                return;
            }

            // Extract dates from filenames and find the most recent one
            DateTime mostRecentDate = DateTime.MinValue;
            string mostRecentFilename = "";

            foreach (string filePath in csvFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                if (DateTime.TryParseExact(fileName, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime currentDate))
                {
                    if (currentDate > mostRecentDate)
                    {
                        mostRecentDate = currentDate;
                        mostRecentFilename = filePath;
                    }
                }
            }

            if (!string.IsNullOrEmpty(mostRecentFilename))
            {
                // Call ExecuteImportData with the most recent filename
                ExecuteImportData(o, mostRecentFilename);
            }
            else
            {
                MessageBox.Show("Error finding the most recent CSV file.");
            }
        }
        private void ExecuteRandomizeColours(object o)
        {
            // Iterate through CsvListBox and randomize colors
            foreach (var csvListBoxItem in CsvListBox)
            {
                csvListBoxItem.Color = PlotItem.GetRandomOxyColor();
            }

            // Refresh the plot
            ReloadVisiblePlotItems();
        }
        private void ExecuteMarkerTypeClick(object o)
        {
            string menuItemName = (string)o;

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
            ReloadVisiblePlotItems();

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
                foreach (var plotItem in CsvListBox)
                {
                    plotItem.IsVisible = true;
                }
            }
            else
            {
                foreach (var plotItem in CsvListBox)
                {
                    plotItem.IsVisible = false;
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
                        foreach (var plotItem in CsvListBox)
                        {
                            plotItem.IsVisible = true;
                        }
                    }
                    else
                    {
                        foreach (var plotItem in CsvListBox)
                        {
                            plotItem.IsVisible = false;
                        }
                    }

                }
            }
        }

        private void ExecuteResetSelectedCheckboxes(object o)
        {
            isSelectAllChecked = false;
            OnPropertyChanged(nameof(IsSelectAllChecked));
            foreach (var csvListBox in CsvListBox)
            {
                csvListBox.IsVisible = false;
            }
        }

        private void ExecuteImportData(object o, string filename = null)
        {
            if (filename == null)
            {
                string initialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSVDATA");
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv";
                if (openFileDialog.ShowDialog() == true)
                {
                    filename = openFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }

            // Use FileCSVImport class to read data from CSV
            ReadDataStructureModel dataStructure = FileHandler.ReadDataToArr(filename);

            if (dataStructure != null && dataStructure.xData != null && dataStructure.yData != null && dataStructure.TableIdentifiers != null)
            {
                // clear old listbox data
                CsvListBox.Clear();
                // clear plot of old data
                PlotModel.Series.Clear();
                PlotModel.InvalidatePlot(true);

                // Create a new PlotItem for each pair of xData and yData
                for (int i = 0; i < dataStructure.xData.Count && i < dataStructure.yData.Count && i < dataStructure.TableIdentifiers.Count; i++)
                {
                    // Check if xData and yData for the current index are not null
                    if (dataStructure.xData[i] != null && dataStructure.yData[i] != null)
                    {
                        // Create a new PlotItem using the xData and yData for each table
                        PlotItem plotItem = new PlotItem
                        {
                            XData = dataStructure.xData[i],
                            YData = dataStructure.yData[i],
                            PlotDisplayName = "Potentiostat " + dataStructure.TableIdentifiers[i].ToString()
                        };
                        plotItem.PropertyChanged += CsvListBox_PropertyChanged;
                        CsvListBox.Add(plotItem);
                        OnPropertyChanged(nameof(CsvListBox));

                    }
                    else
                    {
                        Console.WriteLine($"Error: xData or yData is null for index {i}.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Error reading CSV file or data is null.");
            }
        }

        private void ExecuteLoadSelectedCsv(object o)
        {
            // Handle the execution of the LoadSelectedCsvCommand
            PlotListBoxItem();
        }

        private void ExecuteToggleCsvVisibility(object o)
        {
            // Handle the execution of the ToggleCsvVisibilityCommand
            if (o is PlotItem csvPlotItem)
            {
                csvPlotItem.IsVisible = !csvPlotItem.IsVisible;
            }
        }


        private void InitializePlotModel()
        {
            PlotModel = new PlotModel();

            // NEEDS TO BE CHANGED set title using axis names from csv file
            PlotModel.Title = "";
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X-Axis" });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y-Axis" });
        }



        private void CsvListBox_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PlotItem.IsVisible))
            {
                ReloadVisiblePlotItems();
                plotModel.ResetAllAxes();
                PlotModel.InvalidatePlot(true);
            }
        }


        private void CsvFileInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)

        {
            if (e.PropertyName == nameof(PlotItem.IsVisible))
            {
                ReloadVisiblePlotItems();
                plotModel.ResetAllAxes();
                PlotModel.InvalidatePlot(true);
            }
        }

        // function to reload the visible plot items
        private void ReloadVisiblePlotItems()
        {
            // Clear existing series in the plot
            PlotModel.Series.Clear();

            // Reload data 
            PlotListBoxItem();


            // Refresh the plot
            PlotModel.InvalidatePlot(true);
        }

        // NEEDS TO BE CHANGED
        // function to plot the selected listbox item
        public void PlotListBoxItem()
        {
            // Clear existing series in the plot
            PlotModel.Series.Clear();

            foreach (var csvListBoxItem in CsvListBox)
            {
                if (csvListBoxItem.IsVisible)
                {
                    // Create a new LineSeries
                    LineSeries lineSeries = new LineSeries
                    {
                        Title = csvListBoxItem.PlotDisplayName,
                        Color = csvListBoxItem.Color,
                        MarkerType = SelectedMarkerType,
                        //MarkerSize = 3,
                        //MarkerStroke = OxyColors.Black,
                        MarkerFill = csvListBoxItem.Color,
                        //MarkerStrokeThickness = 1
                    };

                    // Add data points to the LineSeries
                    for (int j = 0; j < csvListBoxItem.XData.Count; j++)
                    {
                        lineSeries.Points.Add(new DataPoint(csvListBoxItem.XData[j], csvListBoxItem.YData[j]));
                    }

                    // Add the LineSeries to the plot
                    PlotModel.Series.Add(lineSeries);
                }
            }

            // Refresh the plot
            PlotModel.InvalidatePlot(true);
        }
    }

    public class PlotItem : ViewModelBase
    {
        private Dictionary<string, OxyColor> fileColors = new Dictionary<string, OxyColor>();

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
                }
            }
        }

        public string PlotDisplayName { get; set; }

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

        // Properties to store X and Y data points
        public List<double> XData { get; set; }
        public List<double> YData { get; set; }

        // Default constructor
        public PlotItem()
        {
            Color = GetRandomOxyColor();
            IsVisible = false;
        }

        // Constructor with parameters
        public PlotItem(ReadDataStructureModel data, int tableIndex)
            : this()
        {
            // Set X and Y data points
            XData = data.xData[tableIndex];
            YData = data.yData[tableIndex];
        }

        public static OxyColor GetRandomOxyColor()
        {
            Random rand = new Random();
            byte[] rgb = new byte[3];

            // Generate colors until a suitable one is found
            do
            {
                rand.NextBytes(rgb);
            } while (IsColorTooLight(rgb));

            return OxyColor.FromRgb(rgb[0], rgb[1], rgb[2]);
        }

        // Check if the color is too light (close to white)
        private static bool IsColorTooLight(byte[] rgb)
        {
            const double LightnessThreshold = 0.7; // Adjust as needed

            // Convert to HSL (Hue, Saturation, Lightness) color space
            double r = rgb[0] / 255.0;
            double g = rgb[1] / 255.0;
            double b = rgb[2] / 255.0;

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double lightness = (min + max) / 2;

            return lightness > LightnessThreshold;
        }
    }
}

