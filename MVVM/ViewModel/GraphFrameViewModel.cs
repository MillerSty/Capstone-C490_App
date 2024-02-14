using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
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
using OxyPlot.Wpf;
using System.Windows;
using System.Reflection.Metadata;
using System.Windows.Media;
using Microsoft.Win32;

namespace C490_App.MVVM.ViewModel
{
    public class GraphFrameViewModel : ViewModelBase
    {
        public PlotModel plotModel;
        private ObservableCollection<PlotItem> csvListBox;
        //private string selectedCsvListBox;
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

        public GraphFrameViewModel()
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

        private void ExecuteImportData(object o)
        {
            string initialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSVDATA");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv";

            openFileDialog.InitialDirectory = initialDirectory;

            if (openFileDialog.ShowDialog() == true)
            {
                // Use FileCSVImport class to read data from CSV
                ReadDataStructureModel dataStructure = FileCSVImport.ReadDataToArr(openFileDialog.FileName);
                
                if (dataStructure != null && dataStructure.xData != null && dataStructure.yData != null && dataStructure.TableIdentifiers != null)
                {
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

    // NEEDS TO BE CHANGED
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
                    OnPropertyChanged(nameof(Color));
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
                    OnPropertyChanged(nameof(WpfColor));

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

        private static OxyColor GetRandomOxyColor()
        {
            Random rand = new Random();
            byte[] rgb = new byte[3];
            rand.NextBytes(rgb);
            return OxyColor.FromRgb(rgb[0], rgb[1], rgb[2]);
        }
    }
}

