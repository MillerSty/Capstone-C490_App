using C490_App.MVVM.Model;
using OxyPlot.Axes;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CsvHelper;
using OxyPlot.Series;
using System.Globalization;

namespace C490_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for GraphFrame.xaml
    /// </summary>
    public partial class GraphFrame : Window
    {
        private PlotModel plotModel;
        public GraphFrame()
        {
            InitializeComponent();
            LoadCsvFileNames();
            InitializePlotModel();
        }

        private void InitializePlotModel()
        {
            plotModel = new PlotModel();
            plotView.Model = plotModel;

            
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "X-Axis" });
            plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Y-Axis" });
        }

        private void LoadCsvFileNames()
        {
            string appFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSV Readings");

            string[] csvFiles = Directory.GetFiles(appFolder, "*.csv");

            // Populate the ListBox with file names
            foreach (string csvFile in csvFiles)
            {
                csvListBox.Items.Add(System.IO.Path.GetFileNameWithoutExtension(csvFile));
            }
        }

        private void CsvListBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            // Handle the selection change
            string? selectedCsvFileName = csvListBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedCsvFileName))
            {
                // Load and plot the selected CSV file
                PlotCsvFile(selectedCsvFileName);
            }
        }
        private void PlotCsvFile(string csvFileName)
        {
            string appFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSV Readings");
            string csvFilePath = System.IO.Path.Combine(appFolder, $"{csvFileName}.csv");

            using (var reader = new StreamReader(csvFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // CSV has two columns: X and Y
                var records = csv.GetRecords<CsvReadingsModel>();


                // Create a LineSeries to plot the data
                var lineSeries = new LineSeries();

                foreach (var record in records)
                {
                    lineSeries.Points.Add(new DataPoint(record.X, record.Y));
                }

                // Add the new series to the existing plot model
                plotModel.Series.Add(lineSeries);

                // Refresh the plot
                plotModel.InvalidatePlot(true);
            }
        }


    }
}
