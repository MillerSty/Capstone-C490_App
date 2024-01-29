using C490_App.MVVM.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;

namespace C490_App.Services
{
    public class ExperimentStore
    {

        public ExperimentStore()
        {
            ledParameters = new ObservableCollection<LEDParameter>(initLedArray());
            Model = new ExperimentModel();
        }
        public ExperimentModel Model { get => model; set => model = value; }

        private ExperimentModel model;
        public ObservableCollection<LEDParameter> ledParameters { get; set; } = new();

        public ObservableCollection<String> ledNames { get; set; } = new();
        public ObservableCollection<LEDParameter> initLedArray()
        {

            for (int i = 0; i < 50; i++)
            {
                ledParameters.Add(new LEDParameter(false, Convert.ToUInt32(i), $"{i}"));
                ledNames.Add($"{i}");
            }
            return ledParameters;
        }

        public void UpdateLEDS(ObservableCollection<bool> ledsSelected)
        {
            for (int i = 0; i < 50; i++)
            {
                ledParameters[i].IsSelected = ledsSelected[i];
            }
        }
        public int btnstate { get; set; } = 0;

        private SerialPort mySerialPort = new SerialPort();
        public void ExperimentRun()
        {
            try
            {
                if (!mySerialPort.IsOpen)
                {
                    //mySerialPort = new SerialPort("COM3", 9600);
                    mySerialPort.BaudRate = 9600;
                    mySerialPort.PortName = "COM3";
                    mySerialPort.NewLine = "\r\n";
                    mySerialPort.ReadTimeout = 500;
                    mySerialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataRecieved);
                    mySerialPort.Open();
                }

                // ExperimentLocal.Model.runExperiment(mySerialPort);
                if (btnstate == 0)
                {
                    mySerialPort.Write("1");
                    btnstate = 1;
                }
                else
                {
                    mySerialPort.Write("2");
                    btnstate = 0;
                }
                //Adjust value if your output is not showing received data
                int value = 50;
                Thread.Sleep(value);

            }
            catch
            {
                Trace.WriteLine("Error with serial");
            }



        }

        private void OnDataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            var serialDevice = sender as SerialPort;
            var indata = serialDevice.ReadExisting();
            Trace.WriteLine(indata.ToString());
            Thread.Sleep(50);
        }


    }
}
