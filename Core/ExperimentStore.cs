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

        //really id break this up into two functions. One for sendData/colour. One for setting timerHelper properties
        public void formatSendData(ref List<char> sendData, ref TimerHelper th, char colour, int ledIndex, int timerIndex)
        {

            sendData.Add(colour);
            th.OnOrOff = "On";
            th.colour = colour;
            th.ledIndex = ledIndex;
            th.timerIndex = timerIndex;
        }

        public List<System.Timers.Timer> ledTimer = new List<System.Timers.Timer>();
        /// <summary>
        /// SimpleSerial communication. Second function for sending more detailed experiment to hardware
        /// </summary>
        public void RunExperiment12()
        {
            //If serial is not open, open it
            if (serialPortWrapper.Open())
            {
                _serialPortWrapper.SerialPort.DiscardInBuffer();
                //load target parameters
                //led's -- Could be made into its own function?
                int count = 0;

                readDataStructure.Date = DateTime.Now;
                readDataStructure.Title = this.Model.GetType().Name; //this could be more dynamic/improved

                int ledTimerIndex = 0;

                foreach (LEDParameter _leds in ledParameters)//write LED's 
                {
                    if (_leds.IsSelected)
                    {
                        ledTimer.Add(new System.Timers.Timer());
                        ledTimer[ledTimerIndex].AutoReset = false;
                        TimerHelper th = new TimerHelper();

                        bool r = false, g = false, b = false; //bools for intensity

                        List<char> serialSendChars = ['L', _leds.Name.ToString()[0]];
                        //if >1 we have a two digit name 
                        //actually this should be unnecessary since we send it as char anyways, if we send 22 as a char[] it will be fine
                        //note if we do do col rol sending we will need to prefix a 0 to #'s < 10                        
                        if (_leds.Name.ToString().Length > 1)
                        {
                            serialSendChars.Add(_leds.Name.ToString()[1]);
                        }

                        if (_leds.GOnTime >= 1)
                        {
                            g = true;
                            formatSendData(ref serialSendChars, ref th, 'G', ledParameters.IndexOf(_leds), ledTimerIndex);
                            ledTimer[ledTimerIndex].Interval = _leds.GOnTime * 1000;
                            ledTimer[ledTimerIndex].Elapsed += (sender, e) => OnTimedEvent(sender, e, ref th);
                        }
                        else if (_leds.ROnTime >= 1)
                        {
                            r = true;
                            formatSendData(ref serialSendChars, ref th, 'R', ledParameters.IndexOf(_leds), ledTimerIndex);
                            ledTimer[ledTimerIndex].Interval = _leds.ROnTime * 1000;
                            ledTimer[ledTimerIndex].Elapsed += (sender, e) => OnTimedEvent(sender, e, ref th);
                        }
                        else if (_leds.BOnTime >= 1)
                        {
                            b = true;
                            formatSendData(ref serialSendChars, ref th, 'B', ledParameters.IndexOf(_leds), ledTimerIndex);
                            ledTimer[ledTimerIndex].Interval = _leds.BOnTime * 1000;
                            ledTimer[ledTimerIndex].Elapsed += (sender, e) => OnTimedEvent(sender, e, ref th);
                        }
                        foreach (var bytes in serialSendChars)
                        {
                            // if not ==3 then we know it didnt have led times >=1
                            //in other words if not ==3 it is an unused LED
                            //this should be reworked such that, if selected its turned on and if it has times, it gets a timer?
                            if (serialSendChars.Count == 3)
                            {
                                _serialPortWrapper.SendData.Add(bytes);
                            }
                        }

                        serialSendChars.Clear();
                        //this moves on to appending intensity to SendData
                        serialSendChars.Add('R');
                        serialSendChars.Add(_leds.Name.ToString()[0]);
                        if (_leds.Name.ToString().Length > 1) //if >1 we have a two digit name
                        {
                            serialSendChars.Add(_leds.Name.ToString()[1]);
                        }
                        if (g && !r && !b)
                        {
                            serialSendChars.Add('G');
                            if (_leds.GIntensity < 100)
                            {
                                serialSendChars.Add('0');
                            }
                            foreach (char c in _leds.GIntensity.ToString())
                            {
                                serialSendChars.Add(c);
                            }
                        }
                        else if (r && !g && !b)
                        {
                            serialSendChars.Add('R');
                            if (_leds.RIntensity < 100)
                            {
                                serialSendChars.Add('0');
                            }
                            foreach (char c in _leds.RIntensity.ToString())
                            {
                                serialSendChars.Add(c);
                            }

                        }
                        else if (b && !g && !r)
                        {
                            serialSendChars.Add('B');
                            if (_leds.BIntensity < 100)
                            {
                                serialSendChars.Add('0');
                            }
                            foreach (char c in _leds.BIntensity.ToString())
                            {
                                serialSendChars.Add(c);
                            }

                        }
                        //Adds intensity to send queue
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

                Stopwatch sw = Stopwatch.StartNew();
                _serialPortWrapper.send();
                sw.Stop();
                Trace.WriteLine(sw.ToString());//00.14 on my home pc

                //enables LED timers
                foreach (var index in ledTimer)
                {
                    index.Enabled = true;

                }

                //Checks how many unused LED's
                if (count == 50)
                {
                    showMessageBox("Running purely potentiostat experiment", "No LED's selected");
                }

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
                        // Model.runExperiment(this);
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
        public struct TimerHelper
        {
            public char colour { get; set; }
            public String OnOrOff { get; set; }
            public int timerIndex { get; set; }
            public int ledIndex { get; set; }

        }
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e, ref TimerHelper th)
        {

            if (th.OnOrOff.Equals("On"))
            {
                //if any other colour has an on time switch to it
                if (ledParameters[th.ledIndex].BOnTime > 0 || ledParameters[th.ledIndex].ROnTime > 0)
                {

                }
                else
                {
                    if (!(ledParameters[th.ledIndex].GOffTime == 0))
                    {
                        ledTimer[th.timerIndex].Interval = ledParameters[th.ledIndex].GOffTime * 1000;
                        th.OnOrOff = "Off";
                        _serialPortWrapper.SendData.Add('L');
                        _serialPortWrapper.SendData.Add(ledParameters[th.ledIndex].Name.ToString()[0]);
                        _serialPortWrapper.SendData.Add(th.colour);

                    }
                    else
                    {
                        th.OnOrOff = "On";
                        ledTimer[th.timerIndex].Interval = ledParameters[th.ledIndex].GOnTime * 1000;
                        //no append to SendData here as we dont want toggle led
                    }
                }
            }
            else
            {
                th.OnOrOff = "On";
                ledTimer[th.timerIndex].Interval = ledParameters[th.ledIndex].GOnTime * 1000;
                _serialPortWrapper.SendData.Add('L');
                _serialPortWrapper.SendData.Add(ledParameters[th.ledIndex].Name.ToString()[0]);
                _serialPortWrapper.SendData.Add(th.colour);
            }
            _serialPortWrapper.send();
            ledTimer[th.timerIndex].Enabled = true;
        }

    }
}
