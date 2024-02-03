using C490_App.MVVM.Model;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace C490_App.Core
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

        public ObservableCollection<string> ledNames { get; set; } = new();

        public ObservableCollection<string> pots { get; set; } = new();
        public ObservableCollection<LEDParameter> initLedArray()
        {

            for (int i = 0; i < 50; i++)
            {
                ledParameters.Add(new LEDParameter(false, Convert.ToUInt32(i), $"{i}"));
                ledNames.Add($"{i}");
            }
            return ledParameters;
        }

        //this updates the ExperimentStore LEDSelected from the LEDArrayViewModel
        public void UpdateLEDS(ObservableCollection<bool> ledsSelected)
        {
            for (int i = 0; i < 50; i++)
            {
                ledParameters[i].IsSelected = ledsSelected[i];
            }
        }
        public void UpdatePots(ObservableCollection<string> activePotentiostats)
        {
            pots = activePotentiostats;
        }

        /// <summary>
        /// this can be used to cull unused LEDParamters
        /// Currently not used as it puts us into unrecoverable LED state
        /// </summary>
        public void checkLeds()
        {
            ObservableCollection<LEDParameter> temp = new ObservableCollection<LEDParameter>();
            for (int i = 0; i < 50; i++)
            {
                if (ledParameters[i].IsSelected == true)
                {
                    temp.Add(ledParameters[i]);
                }

            }
            ledParameters = temp;
        }
        int btnstate = 0;
        private SerialPort _serialPort;
        public SerialPort mySerialPort { get { return _serialPort; } set { _serialPort = value; } }

        /// <summary>
        /// SimpleSerial communication.
        /// Check if port is open, and if not open it and send btn state to turn on LED.
        /// Does not close port.
        /// </summary>
        public void RunExperiment()
        {
            //List<string> comports = ComPortNames(vid, pid);

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

                //ExperimentLocal.Model.runExperiment(mySerialPort);
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
                //TODO this becomes a message box
                Trace.WriteLine("Error with serial");
            }

        }

        /// <summary>
        /// SimpleSerial communication. Second function for sending more detailed experiment to hardware
        /// </summary>
        public void RunExperiment12()
        {
            //List<string> comports = ComPortNames(vid, pid);

            try
            {
                //If serial is not open, open it
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
                //load target parameters
                //led's -- Could be made into its own function?
                int count = 0;
                String ExperimentLeds = "";
                foreach (LEDParameter _leds in ledParameters)
                {
                    if (_leds.IsSelected)
                    {
                        string name = _leds.Address.ToString();
                        string gaddress = _leds.Gaddress.ToString();
                        string gintensity = _leds.GIntensity.ToString();
                        string gon = _leds.GOnTime.ToString();
                        string goff = _leds.GOffTime.ToString();

                        string baddress = _leds.Baddress.ToString();
                        string bintensity = _leds.BIntensity.ToString();
                        string bon = _leds.BOnTime.ToString();
                        string boff = _leds.BOffTime.ToString();

                        string raddress = _leds.Raddress.ToString();
                        string rintensity = _leds.RIntensity.ToString();
                        string ron = _leds.ROnTime.ToString();
                        string roff = _leds.ROffTime.ToString();
                        ExperimentLeds += name;
                        ExperimentLeds += " " + gaddress + " " + gintensity + " " + gon + " " + goff;
                        ExperimentLeds += " " + raddress + " " + rintensity + " " + ron + " " + roff;
                        ExperimentLeds += " " + baddress + " " + bintensity + " " + bon + " " + boff;
                    }
                    else count++;
                }
                if (count == 50)
                {
                    //TODO change to messagebox
                    Trace.WriteLine("No leds selected, running purely potentiostat experiment");
                }
                //TODO when led params sent to target, add a DateTime stamp to something to track on/off times

                //pots -- load pots in use to MCU
                String ExperimentPots = "Potentiostats ";
                foreach (var pots in this.pots)
                {
                    ExperimentPots += " " + pots;
                }

                //run experiment
                Model.runExperiment(mySerialPort);
                Trace.WriteLine("Out of sleep");
                //mySerialPort.Close();


            }
            catch
            {
                Trace.WriteLine("Error with serial");
            }

        }
        /// <summary>
        /// This is for initializing the serial port
        /// </summary>
        public void initSerial()
        {
            if (!mySerialPort.IsOpen)
            {
                mySerialPort.BaudRate = 9600;
                mySerialPort.PortName = "COM3";
                mySerialPort.NewLine = "\r\n";
                mySerialPort.ReadTimeout = 500;
                // mySerialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataRecieved);



            }

        }

        public void assignEventHandler()
        {

        }
        /// <summary>
        /// This handles receiving data from the mCU
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SerialDataReceivedEventArgs</param>
        private void OnDataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            var serialDevice = sender as SerialPort;
            var indata = serialDevice.ReadExisting();
            Trace.WriteLine(indata.ToString());
            Thread.Sleep(50);
        }
        /// <summary>
        /// Used for getting comports used by VID/PID
        /// </summary>
        /// <param name="VID"></param>
        /// <param name="PID"></param>
        /// <returns></returns>
        static List<string> ComPortNames()
        {
            String VID = "";
            String PID = "";
            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();

            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

            foreach (String s3 in rk2.GetSubKeyNames())
            {

                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }

        String vid = "2341";
        String pid = "0043";

    }
}
