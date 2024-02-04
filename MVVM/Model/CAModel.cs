using C490_App.Core;
using System.Diagnostics;

namespace C490_App.MVVM.Model
{
    public class CAModel : ExperimentModel
    {
        static string type = "CAModel";
        public bool? isEnabled { get; set; }
        public double voltageRangeStart { get; set; }
        public double voltageRangeEnd { get; set; }
        public double runTimeStart { get; set; }
        public double runTimeEnd { get; set; }
        public double sampleInterval { get; set; }
        public CAModel()
        {

        }
        public override void setIsEnabled()
        {
            this.isEnabled = true;

        }

        public override void runExperiment(ExperimentStore _serialPort)

        {
            _serialPort.serialPortWrapper.SerialPort.WriteLine("1");
            Trace.WriteLine("CA Experiment running");
        }
    }
}
