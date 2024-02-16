using C490_App.Core;
using System.Diagnostics;

namespace C490_App.MVVM.Model
{
    public class DPVModel : ExperimentModelBase
    {
        static string type = "DPVModel";
        private bool? _isEnabled = false;
        public bool? isEnabled { get { return _isEnabled; } set { _isEnabled = value; OnPropertyChanged(); } }
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

        public override void runExperiment(ExperimentStore _serialPort)
        {
            _serialPort.serialPortWrapper.SerialPort.Write("1");
            Trace.WriteLine("DPV Experiment running");
        }

    }
}
