namespace C490_App.MVVM.Model
{
    public class CVModel : ExperimentModel
    {
        static string type = "CVModel";
        public bool? isEnabled { get; set; }
        public double startVoltage { get; set; }
        public double voltageThresholdOne { get; set; }
        public double voltageThresholdTwo { get; set; }
        public double stepSize { get; set; }
        public double scanRate { get; set; }
        public double numOfScans { get; set; }
        public CVModel()
        {

        }
    }
}
