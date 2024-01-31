using System.IO.Ports;

namespace C490_App.MVVM.Model
{
    public class ExperimentModel
    {
        public virtual void setIsEnabled() { }
        /// <summary>
        /// Note: this getType can be replaced with implement GetType().Name 
        /// </summary>
        /// <returns></returns>
        public virtual string getType() { return ""; }
        public virtual void runExperiment(SerialPort _serialPort) { }
    }
}
