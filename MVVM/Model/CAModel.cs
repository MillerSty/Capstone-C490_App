namespace C490_App.MVVM.Model
{
    public class CAModel : ExperimentModel
    {
        public bool? isEnabled { get; set; }
        public double voltageRangeStart { get; set; }
        public double voltageRangeEnd { get; set; }
        public double runTimeStart { get; set; }
        public double runTimeEnd { get; set; }
        public double sampleInterval { get; set; }
        public CAModel()
        {

        }

    }
}
