using System.Diagnostics;
using System.IO.Ports;

namespace C490_App.MVVM.Model
{
    public class DPVModel : ExperimentModel
    {
        static string type = "DPVModel";
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

        public override void setIsEnabled()
        {
            this.isEnabled = true;

        }

        public override void runExperiment(SerialPort _serialPort)
        {
            _serialPort.Write("1");
            Trace.WriteLine("DPV Experiment running");
        }

    }
}
