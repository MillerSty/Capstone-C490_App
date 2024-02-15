using C490_App.MVVM.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;

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
            ledParameters = new ObservableCollection<LEDParameter>();

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

        private SerialPort _serialPort;
        public SerialPort mySerialPort
        {
            get { return _serialPort; }
            set { _serialPort = value; }
        }

        private SerialPortWrapper _serialPortWrapper;
        public SerialPortWrapper serialPortWrapper
        {
            get { return _serialPortWrapper; }
            set { _serialPortWrapper = value; }
        }

        /// <summary>
        /// SimpleSerial communication. Second function for sending more detailed experiment to hardware
        /// </summary>
        public void RunExperiment12()
        {
            //If serial is not open, open it
            if (serialPortWrapper.Open())
            {
                //load target parameters
                //led's -- Could be made into its own function?
                int count = 0;
                String ExperimentLeds = "";
                foreach (LEDParameter _leds in ledParameters)
                {
                    /*if (_leds.IsSelected)
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
                    }*/
                    // for demo
                    if (_leds.IsSelected)
                    {
                        List<char> serialSendChars = new List<char>();

                        serialSendChars.Add('L');
                        serialSendChars.Add(_leds.Address.ToString()[0]);
                        if (_leds.Address.ToString().Length > 1)
                        {
                            serialSendChars.Add(_leds.Address.ToString()[1]);
                        }

                        if (_leds.GOnTime >= 1)
                        {
                            serialSendChars.Add('G');
                        }
                        else if (_leds.ROnTime >= 1)
                        {
                            serialSendChars.Add('R');
                        }
                        else if (_leds.BOnTime >= 1)
                        {
                            serialSendChars.Add('B');
                        }

                        foreach (var bytes in serialSendChars)
                        {
                            char check = bytes;
                            _serialPortWrapper.SerialPort.Write(bytes.ToString());
                            Thread.Sleep(50);
                        }
                    }
                    // for demo

                    else count++;
                }
                //Checks how many unused LED's
                if (count == 50)
                {
                    //TODO change to messagebox
                    var thread = new Thread(() =>
                    {
                        MessageBox.Show("Running purely potentiostat experiment", "No LED's selected");
                    });
                    thread.Start();
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
                Model.runExperiment(this);
                Trace.WriteLine("Out of sleep");
            }
        }

    }
}
