namespace C490_App.MVVM.Model
{
    public class DPVModel : ExperimentModel
    {
        public bool? isEnabled { get; set; }
        public double startVoltage { get; set; }
        public double endVoltage { get; set; }
        public double stepSize { get; set; }
        public double scanRate { get; set; }
        public double pulsePotential { get; set; }
        public double pulseTime { get; set; }


        public DPVModel()
        {

        }



    }
}
