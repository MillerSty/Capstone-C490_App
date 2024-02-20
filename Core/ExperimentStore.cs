using C490_App.MVVM.Model;
using C490_App.MVVM.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace C490_App.Core
{
    public class ExperimentStore : ViewModelBase
    {

        private SerialPortWrapper _serialPortWrapper;
        public SerialPortWrapper serialPortWrapper
        {
            get { return _serialPortWrapper; }
            set { _serialPortWrapper = value; OnPropertyChanged(); }
        }

        public ExperimentStore()
        {
            //serialPortWrapper.PropertyChanged
            ledParameters = new ObservableCollection<LEDParameter>(initLedArray());
            Model = new ExperimentModelBase();
        }
        public void setPropertyChange()
        {
            serialPortWrapper.PropertyChanged += DataPropertyChanged;
        }
        private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case ("Text2"):
                    var i = sender as SerialPortWrapper;
                    var ikky = 0.0;
                    if (double.TryParse(i.ExperimentData.ToString(), out ikky))
                    {
                        foreach (var y in readDataStructure.yData)
                        {
                            y.Add((float)ikky);
                            var xy = 0;
                        }
                    }
                    break;
                default: break;
            }
        }
        private ExperimentModelBase model;
        public ExperimentModelBase Model { get => model; set => model = value; }



        private ObservableCollection<LEDParameter> _ledParameters { get; set; } = new();
        public ObservableCollection<LEDParameter> ledParameters { get { return _ledParameters; } set { _ledParameters = value; } }
        public ReadDataStructureModel2 readDataStructure { get; set; } = new();

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
                readDataStructure.Date = DateTime.Now;
                readDataStructure.Title = this.Model.GetType().Name; //this could be more dynamic/improved

                List<System.Timers.Timer> ledTimer = new List<System.Timers.Timer>();
                int ledTimerIndex = 0;

                foreach (LEDParameter _leds in ledParameters)//write LED's 
                {
                    //TODO add On times, and intensities
                    if (_leds.IsSelected)
                    {
                        ledTimer.Add(new System.Timers.Timer());
                        //ledTimer[ledTimerIndex].AutoReset = false;

                        bool r = false, g = false, b = false;

                        List<char> serialSendChars = new List<char>();

                        serialSendChars.Add('L');
                        serialSendChars.Add(_leds.Name.ToString()[0]);

                        if (_leds.Name.ToString().Length > 1) //if >1 we have a two digit name
                        {
                            serialSendChars.Add(_leds.Name.ToString()[1]);
                        }
                        if (_leds.GOnTime >= 1)
                        {
                            g = true;
                            serialSendChars.Add('G');
                            var i = ledParameters.IndexOf(_leds);
                            ledTimer[ledTimerIndex].Interval = _leds.GOnTime * 1000;
                            ledTimer[ledTimerIndex].Elapsed += (sender, e) => OnTimedEvent(sender, e, ledParameters.IndexOf(_leds).ToString() + " G ON");
                        }
                        else if (_leds.ROnTime >= 1)
                        {
                            r = true;
                            serialSendChars.Add('R');
                            var i = ledParameters.IndexOf(_leds);
                            ledTimer[ledTimerIndex].Interval = _leds.ROnTime * 1000;
                            ledTimer[ledTimerIndex].Elapsed += (sender, e) => OnTimedEvent(sender, e, ledParameters.IndexOf(_leds).ToString() + " R ON");
                        }
                        else if (_leds.BOnTime >= 1)
                        {
                            b = true;
                            serialSendChars.Add('B');
                            var i = ledParameters.IndexOf(_leds);
                            ledTimer[ledTimerIndex].Interval = _leds.BOnTime * 1000;
                            ledTimer[ledTimerIndex].Elapsed += (sender, e) => OnTimedEvent(sender, e, ledParameters.IndexOf(_leds).ToString() + " B ON");
                        }
                        foreach (var bytes in serialSendChars)
                        {
                            if (serialSendChars.Count == 3) // if not ==3 then we know it didnt have led times >=1
                            {
                                _serialPortWrapper.SendData.Add(bytes);
                            }
                        }

                        serialSendChars.Clear();
                        serialSendChars.Add('R');
                        serialSendChars.Add(_leds.Name.ToString()[0]);
                        if (_leds.Name.ToString().Length > 1) //if >1 we have a two digit name
                        {
                            serialSendChars.Add(_leds.Name.ToString()[1]);
                        }
                        if (g && !r && !b)
                        {
                            serialSendChars.Add('G');
                            foreach (char c in _leds.GIntensity.ToString())
                            {
                                serialSendChars.Add(c);
                            }
                        }
                        else if (r && !g && !b)
                        {
                            serialSendChars.Add('R');
                            foreach (char c in _leds.RIntensity.ToString())
                            {
                                serialSendChars.Add(c);
                            }

                        }
                        else if (b && !g && !r)
                        {
                            serialSendChars.Add('B');
                            foreach (char c in _leds.BIntensity.ToString())
                            {
                                serialSendChars.Add(c);
                            }

                        }
                        foreach (var bytes in serialSendChars)
                        {
                            if (serialSendChars.Count > 0) // if not ==3 then we know it didnt have led times >=1
                            {
                                _serialPortWrapper.SendData.Add(bytes);
                            }
                        }

                    }
                    else count++;

                    ledTimerIndex++;
                }
                foreach (var index in ledTimer)
                {
                    index.Enabled = true;

                }

                //Checks how many unused LED's
                if (count == 50)
                {
                    showMessageBox("Running purely potentiostat experiment", "No LED's selected");
                }
                //this threaded send could work
                //var thread = new Thread(() =>
                //{
                //    _serialPortWrapper.send();
                //});
                //thread.Start();
                //TODO when led params sent to target, add a DateTime stamp to something to track on/off times

                //pots -- load pots in use to MCU
                String ExperimentPots = "Potentiostats ";
                foreach (var pots in this.pots)
                {
                    readDataStructure.TableIdentifiers.Add(int.Parse(pots));
                    readDataStructure.xData.Add(new List<float>());
                    readDataStructure.yData.Add(new List<float>());
                    ExperimentPots += " " + pots;
                }


                //run experiment check if its not base experiment
                if (!Model.GetType().ToString().Equals("C490_App.MVVM.Model.ExperimentModelBase"))
                {
                    //run Experiment
                    var thread = new Thread(() =>
                    {
                        Model.runExperiment(this);
                    });
                    thread.Start();
                }
                else
                {
                    showMessageBox("Running purely LED experiment", "No Experiment selected");
                }


            }
            //ENSURE WE DISPOSE OF TIMERS HERE
            Trace.WriteLine("End of experiment");
        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e, String s)
        {
            System.Timers.Timer newJ = source as System.Timers.Timer;
            String[] parse = s.Split(" ");
            ledParameters[0].Name = "";
            switch (parse[1])
            {
                case ("G"): Trace.WriteLine("G timer goes off"); break;
                case ("R"): Trace.WriteLine("R timer goes off"); break;
                case ("B"): Trace.WriteLine("B timer goes off"); break;


            }


        }

    }
}
