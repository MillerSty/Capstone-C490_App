using C490_App.Core;
using C490_App.MVVM.Model;
using C490_App.MVVM.View;
using System.Windows;

namespace C490_App.MVVM.ViewModel
{
    public class HomeFrameViewModel : ObservableObject
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


        public HomeFrameViewModel()
        {
            _caModel = new CAModel();
            _dpvModel = new DPVModel();
            _cvModel = new CVModel();
            PotentiostatViewModel = new PotentiostatViewModel();
            LedArrayViewModel = new LedArrayViewModel();

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
            LEDParameterFrame dpv = new LEDParameterFrame();

            dpv.Show();
        }

        public void experimentOpen()
        {
            //we  should be able to dynamically set which  experiment to open here... and  itll be clunky butwe  can make it better
            if (_dpvEnabled)
            {
                //we can pass things to constructor aswell to pass data?
                DPVExperimentFrame dpv = new DPVExperimentFrame();

                dpv.Show();
            }
            else if (_cvEnabled)
            {
                CVExperimentFrame cv = new CVExperimentFrame();
                cv.Show();
            }
            else
            {

                MessageBox.Show("This works  as expected  but  janky    ");
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
