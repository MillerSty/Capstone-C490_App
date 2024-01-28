using System.Diagnostics;
using System.IO.Ports;

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

        public override void runExperiment(SerialPort _serialPort)

        {
            _serialPort.WriteLine("1");
            Trace.WriteLine("CA Experiment running");
        }
    }
}
