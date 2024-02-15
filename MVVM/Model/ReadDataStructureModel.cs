using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C490_App.MVVM.Model
{
    public class ReadDataStructureModel
    {
        public ReadDataStructureModel()
        {
            // Initialize lists
            TableIdentifiers = new List<int>();
            axisName = new List<string>();
            xData = new List<List<double>>();
            yData = new List<List<double>>();
        }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public List<int> TableIdentifiers { get; set; }
        public List<string> axisName { get; set; }
        public List<List<double>> xData { get; set; }
        public List<List<double>> yData { get; set; }
    }
}
