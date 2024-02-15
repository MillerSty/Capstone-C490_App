using C490_App.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace C490_App.MVVM.ViewModel
{
    public class LEDParameterViewModel : ViewModelBase
    {
        //Grouped into GRB
        //Sets all attributes per group, ie: G on, G off, G int, R on, R off, R int...

        private int _GonTime;

        public int GOnTime
        {
            get { return _GonTime; }
            set
            {
                _GonTime = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GOnTime = (UInt32)_GonTime;

                OnPropertyChanged();
            }
        }
        private int _GoffTime;

        public int GOffTime
        {
            get { return _GoffTime; }
            set
            {
                _GoffTime = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GOffTime = (UInt32)_GoffTime;
                OnPropertyChanged();
            }
        }
        private string? _gIntensity { get; set; }
        public string GreenIntensity
        {
            get { return _gIntensity; }
            set
            {
                _gIntensity = double.Floor(double.Parse(value)).ToString();
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GIntensity = UInt32.Parse(_gIntensity);
                OnPropertyChanged();

            }
        }
        private int _RonTime;

        public int ROnTime
        {
            get { return _RonTime; }
            set
            {
                _RonTime = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].ROnTime = (UInt32)_RonTime;
                OnPropertyChanged();
            }
        }
        private int _RoffTime;

        public int ROffTime
        {
            get { return _RoffTime; }
            set
            {
                _RoffTime = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].ROffTime = (UInt32)_RoffTime;
                OnPropertyChanged();
            }
        }
        private string? _rIntensity { get; set; }
        public string RedIntensity
        {
            get { return _rIntensity; }
            set
            {
                _rIntensity = double.Floor(double.Parse(value)).ToString();
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].RIntensity = UInt32.Parse(_rIntensity);
                OnPropertyChanged();
            }

        }
        private int _BonTime;

        public int BOnTime
        {
            get { return _BonTime; }
            set
            {
                _BonTime = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BOnTime = (UInt32)_BonTime;
                OnPropertyChanged();
            }
        }

        private int _BoffTime;

        public int BOffTime
        {
            get { return _BoffTime; }
            set
            {
                _BoffTime = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BOffTime = (UInt32)_BoffTime;
                OnPropertyChanged();
            }
        }

        private string? _bIntensity { get; set; }
        public string BlueIntensity
        {
            get { return _bIntensity; }
            set
            {
                _bIntensity = double.Floor(double.Parse(value)).ToString();
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BIntensity = UInt32.Parse(_bIntensity);
                OnPropertyChanged();
            }

        }
        private int selectedIndex { get; set; }
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;
                int checkIndex = int.Parse(LEDS[selectedIndex]);


                BlueIntensity = ExperimentLocal.ledParameters[checkIndex].BIntensity.ToString();
                BOnTime = (int)ExperimentLocal.ledParameters[checkIndex].BOnTime;
                BOffTime = (int)ExperimentLocal.ledParameters[checkIndex].BOffTime;

                GreenIntensity = ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GIntensity.ToString();
                GOnTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GOnTime;
                GOffTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GOffTime;


                RedIntensity = ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].RIntensity.ToString();
                ROnTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].ROnTime;
                ROffTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].ROffTime;
                Trace.WriteLine("Selected: " + checkIndex);
                Trace.WriteLine(ExperimentLocal.ledParameters[checkIndex].BIntensity.ToString());
                OnPropertyChanged();
            }
        }
        private ObservableCollection<int> isModified { get; set; }
        private ObservableCollection<String> _leds { get; set; }
        public ObservableCollection<String> LEDS
        {
            get => _leds;
            set
            {
                _leds = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand Save { get; set; }

        public RelayCommand Cancel { get; set; }

        public ExperimentStore ExperimentLocal { get; set; }

        public LEDParameterViewModel(ExperimentStore ExperimentSingleton)
        {
            ExperimentLocal = ExperimentSingleton;
            check();
            SelectedIndex = 0;

            Save = new RelayCommand(o => save(), o => true);
            Cancel = new RelayCommand(o => this.cancel(o), o => true);
        }
        public void check() // maybe do this with data trigger ?
        {
            _leds = new ObservableCollection<string>(new List<String>(50));
            foreach (var led in ExperimentLocal.ledParameters)
            {
                if (led.IsSelected)
                {
                    _leds.Add(led.Name);
                }
            }

        }
        public void save()
        {
            Trace.WriteLine("Saving");
        }
        public void cancel(Object o)
        {
            Trace.WriteLine("Canceling");
            close();
        }

        private void close()
        {
            //reset everything that wasnt modified?
            foreach (System.Windows.Window window in System.Windows.Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                }
            }
        }


    }
}
