using C490_App.MVVM.Model;
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

namespace C490_App.MVVM.View
{
    /// <summary>
    /// Interaction logic for GraphFrame.xaml
    /// </summary>
    public partial class GraphFrame : Window
    {
        public GraphFrame()
        {
            InitializeComponent();
            LoadCsvFileNames();
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
            // Clear the plot
            plotView.Model = null;

            // Load the CSV file
            string appFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Split(new[] { "\\bin\\" }, StringSplitOptions.None)[0], "Resources", "CSV Readings");
            string csvFilePath = appFolder + csvFileName + ".csv";

            // Read the CSV file
            var csvFile = new CsvReadingsModel(csvFilePath);

            // Plot the CSV file
            plotView.Model = csvFile.PlotModel;
        }   
    }
}
