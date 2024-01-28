using System.Diagnostics;
using System.IO.Ports;

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
        public override void setIsEnabled()
        {
            this.isEnabled = true;

        }

        public override void runExperiment(SerialPort _serialPort)

        {
            _serialPort.WriteLine("1");
            Trace.WriteLine("CV Experiment running");
        }
    }
}
