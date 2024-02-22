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
            float interval;


            //from start voltage(sV) increase by stepSize every scanRate...
            if (startVoltage <= voltageThresholdOne) runForward(_serialPort);
            else runBackward();
            //if sV greater then voltageThresholdOne decrease by step size every scanRate until... 0?

        }
        public Timer timer { get; set; }
        public void runForward(ExperimentStore _serialPort)
        {
            _serialPort.serialPortWrapper.SerialPort.DiscardInBuffer();
            _serialPort.serialPortWrapper.SerialPort.DiscardOutBuffer();
            //var s = time.ToString();
            double interval = stepSize / scanRate;
            List<double> potsActive = new();
            foreach (var pot in _serialPort.pots)
            {
                potsActive.Add(double.Parse(pot));
            }


            foreach (var x in _serialPort.readDataStructure.xData)
            {
                x.Add(0);
            }
            foreach (var y in _serialPort.readDataStructure.yData)
            {
                y.Add((float)startVoltage);
            }

            double tempvolt = (float)startVoltage;
            double temptemp = tempvolt;
            var time = Stopwatch.StartNew();
            for (int i = 1; i <= numOfScans; i++)
            {
                time.Restart();
                time.Start();
                double timeCount = 0.00;

                for (double j = temptemp; j < voltageThresholdOne; j += stepSize)
                {
                    while (time.ElapsedMilliseconds < interval * 1000) ;
                    timeCount += ((double)time.ElapsedMilliseconds / 1000);
                    float kj = (float)j;
                    Trace.WriteLine(kj.ToString());
                    //_serialPort.serialPortWrapper.writeString(kj.ToString());
                    Thread.Sleep(50);

                    foreach (var x in _serialPort.readDataStructure.xData)
                    {
                        x.Add((float)timeCount);
                    }
                    foreach (var y in _serialPort.readDataStructure.yData)
                    {
                        y.Add((float)j);
                    }
                    tempvolt = j;
                    time.Restart();
                }

                for (double j = tempvolt; j >= voltageThresholdTwo; j -= stepSize)
                {
                    while (time.ElapsedMilliseconds < interval * 1000) ;
                    timeCount += ((double)time.ElapsedMilliseconds / 1000);
                    foreach (var x in _serialPort.readDataStructure.xData)
                    {
                        x.Add((float)timeCount);
                    }
                    foreach (var y in _serialPort.readDataStructure.yData)
                    {
                        y.Add((float)j);
                    }
                    time.Restart();
                }

                time.Restart();

            }



        }

        public void runBackward()
        {

        }
    }
}
