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
        public PotentiostatViewModel PotentiostatViewModel { get; }

        public LedArrayViewModel LedArrayViewModel { get; }

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

        public RelayCommand ExperimentCheck { get; }
        public RelayCommand openWindow { get; set; }

        public RelayCommand openWindowLED { get; set; }

        public RelayCommand openGraphResults { get; set; }
        public RelayCommand imexParams { get; set; }
        public RelayCommand serialCommunicate { get; set; }

        private ExperimentStore ExperimentLocal { get; set; }

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
        private void SimpleSerial()
        {

            if (!mySerialPort.IsOpen)
            {
                //mySerialPort = new SerialPort("COM3", 9600);
                mySerialPort.BaudRate = 9600;
                mySerialPort.PortName = "COM3";
                mySerialPort.Open();

                mySerialPort.NewLine = "\r\n";
                mySerialPort.ReadTimeout = 500;
                mySerialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataRecieved);
            }

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
            //mySerialPort.Close();


        }

        private void OnDataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            var serialDevice = sender as SerialPort;
            var indata = serialDevice.ReadExisting();
            Trace.WriteLine(indata.ToString());
        }
        private void IMEXParams(object o)
        {
            FileHandler f = new FileHandler();

            if (bool.Parse(o.ToString()))
            {
                Trace.WriteLine("Import params in VM");
                bool returned = f.fileImport(o, ExperimentLocal, LedArrayViewModel);
                if (returned)
                {
                    if (ExperimentLocal.Model.getType().ToLower().Contains("dpv"))
                    {
                        dpvEnabled = true;
                    }
                    else if (ExperimentLocal.Model.getType().ToLower().ToLower().Contains("ca"))
                    {
                        caEnabled = true;
                    }
                    else if (ExperimentLocal.Model.getType().ToLower().ToLower().Contains("cv"))
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
                f.fileExport(ExperimentLocal);
            }



        }






        private void GraphOpen()
        {
            GraphFrame graphFrame = new GraphFrame();
            graphFrame.Show();
        }

        private void LEDOpen()
        {
            LEDParameterFrame ledFrameNavigation = new LEDParameterFrame();

            ExperimentLocal.UpdateLEDS(LedArrayViewModel.isSelected);

            ledFrameNavigation.DataContext = new LEDParameterViewModel(ExperimentLocal);
            ledFrameNavigation.Show();
        }

        public void experimentOpen()
        {
            //we  should be able to dynamically set which  experiment to open here... and  itll be clunky butwe  can make it better
            //This might be able to be replaced with switch statement too using get type
            if (_dpvEnabled)
            {
                if (ExperimentLocal.Model == null)
                {
                    ExperimentLocal.Model = new DPVModel();
                }
                Trace.WriteLine(ExperimentLocal.Model.getType());
                DPVExperimentFrame dpv = new DPVExperimentFrame();
                dpv.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                dpv.Show();
            }
            else if (_cvEnabled)
            {
                CVExperimentFrame cv = new CVExperimentFrame();
                if (ExperimentLocal.Model == null)
                {
                    ExperimentLocal.Model = new CVModel();
                }
                cv.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                cv.Show();
            }
            else if (_caEnabled)
            {
                CAExperimentFrame ca = new CAExperimentFrame();
                if (ExperimentLocal.Model == null)
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

        public void experimentCheck(object o)
        {
            switch (o.ToString())
            {
                case "cv": dpvEnabled = false; caEnabled = false; break;

                case "dpv": cvEnabled = false; caEnabled = false; break;

                case "ca": dpvEnabled = false; cvEnabled = false; break;
            }
        }
    }
}