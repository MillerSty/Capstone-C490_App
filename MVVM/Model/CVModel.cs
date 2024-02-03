using System.Diagnostics;
using System.IO.Ports;

namespace C490_App.MVVM.Model
{
    public class CVModel : ExperimentModel
    {
        static string type = "CVModel"; //used for getting type ... maybe deprecated
        public bool? isEnabled { get; set; }  //used for setting experiment type
        public double startVoltage { get; set; } //Potential where scan starts and stops.
        public double voltageThresholdOne { get; set; } //First potential where direction reverses.
        public double voltageThresholdTwo { get; set; } //Second potential where direction reverses.
        public double stepSize { get; set; } //Step potential
        public double scanRate { get; set; } // palmsens4 is .02 mV/s(.075mV step) - 500V/s (10mV step) , how fast we can sweep potential, slope of triangle
        public double numOfScans { get; set; } //The number of repetitions for this scan.
        //note: no final potential?
        public CVModel()
        {

        }
        public override void setIsEnabled()
        {
            this.isEnabled = true;

        }
        int check = 0b1;
        public override void runExperiment(SerialPort _serialPort)

        {
            _serialPort.Write(check.ToString());
            check = ~check;
            Trace.WriteLine("CV Experiment running");
            float temp_volt = 0;
            //from start voltage(sV) increase by stepSize every scanRate...
            if (startVoltage <= voltageThresholdOne) runForward();
            else runBackward();
            //if sV greater then voltageThresholdOne decrease by steo size every scanRate until... 0?

        }

        public void runForward()
        {


        }

        public void runBackward()
        {

        }
    }
}
