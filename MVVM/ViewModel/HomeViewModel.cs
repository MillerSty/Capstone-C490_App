using C490_App.Core;
using C490_App.MVVM.Model;
using C490_App.MVVM.View;
using System.Diagnostics;
using System.Windows;

namespace C490_App.MVVM.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {
        /*
         * Viewmodel variables for potentiostat and led selection
         */
        public PotentiostatViewModel PotentiostatViewModel { get; }

        public LedArrayViewModel LedArrayViewModel { get; }


        /*
         * Variables for experiment selection
         */
        private CAModel _caModel = new CAModel();
        public CAModel CAModel
        {
            get { return _caModel; }
            set
            {
                _caModel = value;
                OnPropertyChanged();
            }
        }
        private DPVModel _dpvModel = new DPVModel();
        public DPVModel DPVModel
        {
            get { return _dpvModel; }
            set
            {
                _dpvModel = value;
            }
        }
        private CVModel _cvModel = new CVModel();
        public CVModel CVModel
        {
            get { return _cvModel; }
            set
            {
                _cvModel = value;
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
        public RelayCommand openDebug { get; set; }

        private ExperimentStore ExperimentLocal { get; set; }


        /// <summary>
        /// HomeFrameViewModel's constructor. Stores ExperimentSingleton locally.
        /// Listens for relayCommands.
        /// </summary>
        /// <param name="ExperimentSingleton"> The global experimentStore</param>
        public HomeViewModel(ExperimentStore ExperimentSingleton)
        {
            ExperimentLocal = ExperimentSingleton;

            PotentiostatViewModel = new PotentiostatViewModel(ExperimentLocal);
            LedArrayViewModel = new LedArrayViewModel(ExperimentLocal);

            ExperimentCheck = new RelayCommand(o => experimentCheck(o), o => true);
            openWindow = new RelayCommand(o => experimentOpen(), o => true);
            openWindowLED = new RelayCommand(o => LEDOpen(), o => true);
            openGraphResults = new RelayCommand(o => GraphOpen(), o => true);
            imexParams = new RelayCommand(o => IMEXParams(o), o => true);
            serialCommunicate = new RelayCommand(o => SimpleSerial(), o => true);
            openDebug = new RelayCommand(o => OpenDebug(), o => true);
            //Application.Current.Dispatcher.Invoke((Action)delegate
            //{
            //    OpenDebug();
            //});
        }
        /// <summary>
        /// Opens the Debug View 
        /// </summary>
        private void OpenDebug()
        {

            DebugView debug = new DebugView();
            debug.DataContext = new DebugViewModel(ExperimentLocal);
            debug.Show();
        }

        /// <summary>
        /// SimpleSerial communication.
        /// Check if port is open, and if not open it and send btn state to turn on LED.
        /// Does not close port.
        /// </summary>
        private void SimpleSerial()
        {
            //var threadSend = new Thread(() =>
            //{
            ExperimentLocal.RunExperiment12();
            //});
            // threadSend.Start();
            //ExperimentLocal.RunExperiment12();
        }

        /// <summary>
        /// Utilises the file handler class to import and export parameters
        /// </summary>
        /// <param name="imexBool">True = Import. False = Export</param>
        private void IMEXParams(object imexBool)
        {
            FileHandler fileHandler = new FileHandler();

            if (bool.Parse(imexBool.ToString()))
            {
                Trace.WriteLine("Import params in ViewModel");
                bool returned = fileHandler.fileImport(ExperimentLocal, LedArrayViewModel, PotentiostatViewModel);
                if (returned) //NOTE we are setting is enabled here.... but also in the import function?
                {
                    if (ExperimentLocal.Model.GetType().Name.ToString().ToLower().Contains("dpv"))
                    {
                        DPVModel.isEnabled = true;
                    }
                    else if (ExperimentLocal.Model.GetType().Name.ToString().ToLower().Contains("ca"))
                    {
                        CAModel.isEnabled = true;
                    }
                    else if (ExperimentLocal.Model.GetType().Name.ToString().ToLower().Contains("cv"))
                    {
                        CVModel.isEnabled = true;
                    }
                }
            }
            else
            {
                ExperimentLocal.UpdateLEDS(LedArrayViewModel.isSelected);
                fileHandler.fileExport(ExperimentLocal);
            }
        }

        /// <summary>
        /// Navigates to the graphFrame
        /// </summary>
        private void GraphOpen()
        {
            GraphView graphView = new GraphView();
            graphView.Show();
        }

        /// <summary>
        /// Navigates to the LED parameter frame
        /// Updates the Global ExperimentStore with the Selected LED's from homeViewModel
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

            if ((bool)DPVModel.isEnabled)
            {
                if (!ExperimentLocal.Model.GetType().Name.Equals("DPVModel"))
                {
                    ExperimentLocal.Model = DPVModel;
                }

                DPVExperimentFrame dpv = new DPVExperimentFrame();
                dpv.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                dpv.Show();
            }
            else if ((bool)CVModel.isEnabled)
            {
                CVExperimentFrame cv = new CVExperimentFrame();
                if (!ExperimentLocal.Model.GetType().Name.Equals("CVModel"))
                {
                    ExperimentLocal.Model = CVModel;
                }
                cv.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                cv.Show();
            }
            else if ((bool)CAModel.isEnabled)
            {
                CAExperimentFrame ca = new CAExperimentFrame();
                if (!ExperimentLocal.Model.GetType().Name.Equals("CAModel"))
                {
                    ExperimentLocal.Model = CAModel;
                }
                ca.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                ca.Show();
            }
            else
            {
                MessageBox.Show("No Experiment Selected", "Experiment Parameter Error");
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
                case "cv":
                    DPVModel.isEnabled = false;
                    CAModel.isEnabled = false;
                    CVModel.isEnabled ??= false; //if this==null assign false to it
                    break;

                case "dpv":
                    CVModel.isEnabled = false;
                    CAModel.isEnabled = false;
                    DPVModel.isEnabled ??= false;  //if this==null assign false to it
                    break;

                case "ca":
                    DPVModel.isEnabled = false;
                    CVModel.isEnabled = false;
                    CAModel.isEnabled ??= false;//if this==null assign false to it
                    break;
            }
        }
    }
}