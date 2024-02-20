namespace C490_App.MVVM.Model
{
    public class ReadDataStructureModel2
    {
        public ReadDataStructureModel2()
        {
            // Initialize lists
            TableIdentifiers = new List<int>();
            axisName = new List<string>();
            xData = new List<List<float>>();
            yData = new List<List<float>>();
        }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public List<int> TableIdentifiers { get; set; }
        public List<string> axisName { get; set; }
        public List<List<float>> xData { get; set; }
        public List<List<float>> yData { get; set; }
    }
}
