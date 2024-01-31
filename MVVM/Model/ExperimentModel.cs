using System.IO.Ports;

namespace C490_App.MVVM.Model
{
    public class ExperimentModel
    {
        public virtual void setIsEnabled() { }
        public virtual void runExperiment(SerialPort _serialPort) { }
    }
}
