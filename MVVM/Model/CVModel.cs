using C490_App.Core;
using System.Diagnostics;

namespace C490_App.MVVM.Model
{
    public class CVModel : ExperimentModelBase
    {
        static string type = "CVModel"; //used for getting type ... maybe deprecated
        private bool? _isEnabled = false;//used for setting experiment type
        public bool? isEnabled { get { return _isEnabled; } set { _isEnabled = value; OnPropertyChanged(); } }
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
        public override void runExperiment(ExperimentStore _serialPort)
        {
            Trace.WriteLine("CV Experiment running");

            //little turn of turn off LED 
            //_serialPort.serialPortWrapper.SerialPort.Write(check.ToString());
            //check = ~check;
            //end little led turn on/off

            float temp_volt = 0;

            //from start voltage(sV) increase by stepSize every scanRate...
            if (startVoltage <= voltageThresholdOne) runForward();
            else runBackward();
            //if sV greater then voltageThresholdOne decrease by step size every scanRate until... 0?

        }
        public Timer timer { get; set; }
        public void runForward()
        {
            var time = Stopwatch.StartNew();
            time.Start();
            var s = time.ToString();
            //probably needs to be in its own thread 
            for (int i = 1; i <= numOfScans; i++)
            {

                while (time.ElapsedMilliseconds < scanRate * 1000) ;

                Trace.WriteLine("Out of forever loop");
                time.Restart();

            }
            //var timey = Stopwatch.GetElapsedTime((long)time);
            decimal milliseconds = DateTime.Now.Ticks / (decimal)TimeSpan.TicksPerMillisecond;
            //send start voltage



        }

        public void runBackward()
        {

        }
    }
}
