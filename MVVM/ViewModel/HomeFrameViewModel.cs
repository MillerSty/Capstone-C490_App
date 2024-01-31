using C490_App.Core;
using C490_App.MVVM.Model;
using C490_App.MVVM.View;
using C490_App.Services;
using System.Diagnostics;
using System.IO.Ports;
using System.Windows;

namespace C490_App.MVVM.ViewModel
{
    public class HomeFrameViewModel : ViewModelBase
    {
        /*
         * Viewmodel variables for potentiostat and led selection
         */
        public PotentiostatViewModel PotentiostatViewModel { get; }

        public LedArrayViewModel LedArrayViewModel { get; }


        /*
         * Variables for experiment selection
         */
        private CAModel _caModel;
        private DPVModel _dpvModel;
        private CVModel _cvModel;

        public bool _cvEnabled;
        public bool cvEnabled
        {
            get { return _cvEnabled; }
            set
            {
                _cvEnabled = value;
                _cvModel.isEnabled = _cvEnabled;
                OnPropertyChanged();
            }
        }

        public bool _dpvEnabled;
        public bool dpvEnabled
        {
            get { return _dpvEnabled; }
            set
            {
                _dpvEnabled = value;
                _dpvModel.isEnabled = _dpvEnabled;
                OnPropertyChanged();
            }
        }

        public bool _caEnabled;
        public bool caEnabled
        {
            get { return _caEnabled; }
            set
            {
                _caEnabled = value;
                _caModel.isEnabled = _caEnabled;
                OnPropertyChanged();
            }
        }


        /*
         * Relay commands 
         */
        public RelayCommand ExperimentCheck { get; }
        public RelayCommand openWindow { get; set; }

        public RelayCommand openWindowLED { get; set; }

        public RelayCommand openGraphResults { get; set; }
        public RelayCommand imexParams { get; set; }
        public RelayCommand serialCommunicate { get; set; }

        private ExperimentStore ExperimentLocal { get; set; }


        /// <summary>
        /// HomeFrameViewModel's constructor. Stores ExperimentSingleton locally.
        /// Listens for relayCommands.
        /// </summary>
        /// <param name="ExperimentSingleton"> The global experimentStore</param>
        public HomeFrameViewModel(ExperimentStore ExperimentSingleton)
        {
            ExperimentLocal = ExperimentSingleton;

            _caModel = new CAModel();
            _dpvModel = new DPVModel();
            _cvModel = new CVModel();

            PotentiostatViewModel = new PotentiostatViewModel(ExperimentLocal);
            LedArrayViewModel = new LedArrayViewModel(ExperimentLocal);

            ExperimentCheck = new RelayCommand(o => experimentCheck(o), o => true);
            openWindow = new RelayCommand(o => experimentOpen(), o => true);
            openWindowLED = new RelayCommand(o => LEDOpen(), o => true);
            openGraphResults = new RelayCommand(o => GraphOpen(), o => true);
            imexParams = new RelayCommand(o => IMEXParams(o), o => true);
            serialCommunicate = new RelayCommand(o => SimpleSerial(), o => true);
        }


        //btn state is just for simple led turn on
        private int btnstate { get; set; } = 0;

        private SerialPort mySerialPort = new SerialPort();

        /// <summary>
        /// SimpleSerial communication.
        /// Check if port is open, and if not open it and send btn state to turn on LED.
        /// Does not close port.
        /// </summary>
        private void SimpleSerial()
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

                ExperimentLocal.Model.runExperiment(mySerialPort);
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
            //mySerialPort.Close();


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
        /// Utilises the file handler class to import and export parameters
        /// </summary>
        /// <param name="sender"></param>
        private void IMEXParams(object imexBool)
        {
            FileHandler fileHandler = new FileHandler();

            if (bool.Parse(imexBool.ToString()))
            {
                Trace.WriteLine("Import params in ViewModel");
                bool returned = fileHandler.fileImport(ExperimentLocal, LedArrayViewModel);
                if (returned)
                {
                    if (ExperimentLocal.Model.GetType().Name.ToString().ToLower().Contains("dpv"))
                    {
                        dpvEnabled = true;
                    }
                    else if (ExperimentLocal.Model.GetType().Name.ToString().ToLower().Contains("ca"))
                    {
                        caEnabled = true;
                    }
                    else if (ExperimentLocal.Model.GetType().Name.ToString().ToLower().Contains("cv"))
                    {
                        cvEnabled = true;
                    }

                    else
                    {
                        Trace.WriteLine("Not valid experiment file");
                    }
                }
                else
                {
                    {
                        Trace.WriteLine("Load File Failed");
                    }
                }
                Trace.WriteLine("Import params");


            }
            else
            {
                Trace.WriteLine("Export in VM");
                fileHandler.fileExport(ExperimentLocal);
            }



        }

        /// <summary>
        /// Navigates to the graphFrame
        /// </summary>
        private void GraphOpen()
        {
            GraphFrame graphFrame = new GraphFrame();
            graphFrame.Show();
        }

        /// <summary>
        /// Navigates to the LED parameter frame
        /// Updates the Global ExperimentStore with the Selected LED's from homeframeViewModel
        /// </summary>
        private void LEDOpen()
        {
            LEDParameterFrame ledFrameNavigation = new LEDParameterFrame();

            ExperimentLocal.UpdateLEDS(LedArrayViewModel.isSelected);

            ledFrameNavigation.DataContext = new LEDParameterViewModel(ExperimentLocal);
            ledFrameNavigation.Show();
        }

        /// <summary>
        /// Navigates to the proper ExperimentFrame depending on which Experiment was selected
        /// If no experiment selected show messagebox error
        /// </summary>
        public void experimentOpen()
        {
            //we  should be able to dynamically set which  experiment to open here... and  itll be clunky butwe  can make it better
            //This might be able to be replaced with switch statement too using get type
            if (_dpvEnabled)
            {
                if (ExperimentLocal.Model.GetType().Name.Equals("ExperimentModel"))
                {
                    ExperimentLocal.Model = new DPVModel();
                }

                Trace.WriteLine(ExperimentLocal.Model.GetType().ToString() + "Debugging trace");

                DPVExperimentFrame dpv = new DPVExperimentFrame();
                dpv.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                dpv.Show();
            }
            else if (_cvEnabled)
            {
                CVExperimentFrame cv = new CVExperimentFrame();
                if (ExperimentLocal.Model.GetType().Name.Equals("ExperimentModel"))
                {
                    ExperimentLocal.Model = new CVModel();
                }
                cv.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                cv.Show();
            }
            else if (_caEnabled)
            {
                CAExperimentFrame ca = new CAExperimentFrame();
                if (ExperimentLocal.Model.GetType().Name.Equals("ExperimentModel"))
                {
                    ExperimentLocal.Model = new CAModel();
                }
                ca.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                ca.Show();
            }
            else
            {
                MessageBox.Show("No Experiment Selected");
            }
        }

        /// <summary>
        /// Binds to the xml HomeFrameView for selecting one experiment
        /// </summary>
        /// <param name="experimentCheckBox"> This is the checkbox passed from HomeFrameView to the viewmodel</param>
        public void experimentCheck(object experimentCheckBox)
        {
            switch (experimentCheckBox.ToString())
            {
                case "cv": dpvEnabled = false; caEnabled = false; break;

                case "dpv": cvEnabled = false; caEnabled = false; break;

                case "ca": dpvEnabled = false; cvEnabled = false; break;
            }
        }
    }
}