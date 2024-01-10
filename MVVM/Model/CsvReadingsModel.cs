using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C490_App.MVVM.Model
{
    internal class CsvReadingsModel
    {
        private string csvFilePath;

        public CsvReadingsModel(string csvFilePath)
        {
            this.csvFilePath = csvFilePath;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public PlotModel? PlotModel { get; internal set; }
    }
}
