using C490_App.Core;
using C490_App.MVVM.Model;
using C490_App.MVVM.View;
using C490_App.Services;
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

        //public bool _dpvEnabled, _cvEnabled;
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

        // public IExperimentStore m { get; set; }
        ExperimentStore ExperimentLocal { get; set; }
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


        }
        private void GraphOpen()
        {
            GraphFrame graphFrame = new GraphFrame();
            graphFrame.Show();
        }
        private void LEDOpen()
        {
            LEDParameterFrame ledFrameNavigation = new LEDParameterFrame();
            ledFrameNavigation.DataContext = new LEDParameterViewModel(ExperimentLocal);
            ledFrameNavigation.Show();
        }

        public void experimentOpen()
        {
            //we  should be able to dynamically set which  experiment to open here... and  itll be clunky butwe  can make it better
            if (_dpvEnabled)
            {
                ExperimentLocal.setModel(new DPVModel());
                DPVExperimentFrame dpv = new DPVExperimentFrame();

                dpv.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                dpv.Show();
            }
            else if (_cvEnabled)
            {
                CVExperimentFrame cv = new CVExperimentFrame();
                ExperimentLocal.setModel(new CVModel());

                cv.DataContext = new ExperimentParameterViewModel(ExperimentLocal);
                cv.Show();
            }
            else if (_caEnabled)
            {
                CAExperimentFrame ca = new CAExperimentFrame();
                ExperimentLocal.setModel(new CAModel());

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
