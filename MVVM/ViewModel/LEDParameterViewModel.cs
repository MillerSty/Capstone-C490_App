using C490_App.Core;
using C490_App.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace C490_App.MVVM.ViewModel
{
    public class LEDParameterViewModel : ViewModelBase
    {
        public string _gIntensity { get; set; }
        public string GreenIntensity
        {
            get { return _gIntensity; }
            set
            {
                _gIntensity = double.Floor(double.Parse(value)).ToString();

            }

        }
        public string _rIntensity { get; set; }
        public string RedIntensity
        {
            get { return _rIntensity; }
            set
            {
                _rIntensity = double.Floor(double.Parse(value)).ToString();
            }

        }
        public string _bIntensity { get; set; }
        public string BlueIntensity
        {
            get { return _bIntensity; }
            set
            {
                _bIntensity = double.Floor(double.Parse(value)).ToString();
            }

        }
        public int selectedIndex { get; set; }
        public int SelectedIndex
        {
            get => selectedIndex;
            set { selectedIndex = value; OnPropertyChanged(); }
        }
        public ObservableCollection<String> lEDs { get; set; }
        public ObservableCollection<String> LEDS
        {
            get => lEDs;
            set { lEDs = value; OnPropertyChanged(); }
        }

        public RelayCommand Save { get; set; }

        public RelayCommand Cancel { get; set; }

        ExperimentStore ExperimentLocal { get; set; }
        public LEDParameterViewModel(ExperimentStore ExperimentSingleton)
        {

            ExperimentLocal = ExperimentSingleton;
            check();
            //lEDs = ExperimentLocal.ledParameters;
            Save = new RelayCommand(o => save(), o => true);

            Cancel = new RelayCommand(o => cancel(), o => true);
        }
        public void check()
        {
            lEDs = new ObservableCollection<string>(new List<String>(50));
            foreach (var led in ExperimentLocal.ledParameters)
            {
                if (led.isSelected)
                {
                    lEDs.Add(led.name);
                }
            }

        }
        public void save()
        {
            Trace.WriteLine("Saving");
        }
        public void cancel() { Trace.WriteLine("Canceling"); }




    }
}
