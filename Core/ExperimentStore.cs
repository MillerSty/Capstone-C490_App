using C490_App.MVVM.Model;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Windows;

namespace C490_App.Core
{
    public class ExperimentStore
    {

        public ExperimentStore()
        {
            ledParameters = new ObservableCollection<LEDParameter>(initLedArray());
            Model = new ExperimentModelBase();
        }
        public ExperimentModelBase Model { get => model; set => model = value; }

        private ExperimentModelBase model;
        public ObservableCollection<LEDParameter> ledParameters { get; set; } = new();

        public ObservableCollection<string> ledNames { get; set; } = new();

        public ObservableCollection<string> pots { get; set; } = new();
        public ObservableCollection<LEDParameter> initLedArray()
        {
            ledParameters = new ObservableCollection<LEDParameter>();

            for (int i = 0; i < 50; i++)
            {
                ledParameters.Add(new LEDParameter(false, $"{i}"));
                ledParameters[i][0] = ((ushort)(ushort.Parse(ledParameters[i].Name) % 10));
                ledParameters[i][1] = ((ushort)(ushort.Parse(ledParameters[i].Name) / 10));
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
        public void showMessageBox(string message, string title)
        {
            var thread = new Thread(() =>
            {
                MessageBox.Show(message, title);
            });
            thread.Start();

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
                    //write LED's 
                    //TODO add On times, and intensities
                    if (_leds.IsSelected)
                    {
                        List<char> serialSendChars = new List<char>();

                        serialSendChars.Add('L');
                        serialSendChars.Add(_leds.Name.ToString()[0]);
                        if (_leds.Name.ToString().Length > 1)
                        {
                            serialSendChars.Add(_leds.Name.ToString()[1]);
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
                            if (serialSendChars.Count == 3) // if not ==3 then we know it didnt have led times >=1
                            {
                                char check = bytes;
                                _serialPortWrapper.SerialPort.Write(bytes.ToString());
                                Thread.Sleep(50);
                            }
                        }
                    }

                    else count++;
                }
                //Checks how many unused LED's
                if (count == 50)
                {
                    showMessageBox("Running purely potentiostat experiment", "No LED's selected");
                }

                //TODO when led params sent to target, add a DateTime stamp to something to track on/off times

                //pots -- load pots in use to MCU
                String ExperimentPots = "Potentiostats ";
                foreach (var pots in this.pots)
                {
                    ExperimentPots += " " + pots;
                }


                //run experiment check if its not base experiment
                if (!Model.GetType().ToString().Equals("C490_App.MVVM.Model.ExperimentModel"))
                {
                    //run Experiment
                    var thread = new Thread(() =>
                    {
                        Model.runExperiment(this);
                    });
                    thread.Start();
                    //Model.runExperiment(this);
                }
                else
                {
                    showMessageBox("Running purely LED experiment", "No Experiment selected");
                }


            }
        }

    }
}
