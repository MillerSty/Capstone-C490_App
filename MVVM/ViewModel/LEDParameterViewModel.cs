using C490_App.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace C490_App.MVVM.ViewModel
{
    public class LEDParameterViewModel : ViewModelBase
    {
        //Grouped into GRB
        //Sets all attributes per group, ie: G on, G off, G int, R on, R off, R int...
        private bool _green = false;
        private bool _red = false;
        private bool _blue = false;
        public bool Green { get => _green; set => _green = value; }
        public bool Red { get => _red; set => _red = value; }
        public bool Blue { get => _blue; set => _blue = value; }

        private int _GonTime;

        public int GOnTime
        {
            get { return _GonTime; }
            set
            {
                _GonTime = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GOnTime = (ushort)_GonTime;
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
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GOffTime = (ushort)_GoffTime;
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
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GIntensity = ushort.Parse(_gIntensity);
                OnPropertyChanged();

            }
        }
        private int _gCycle;

        public int GCycle
        {
            get { return _gCycle; }
            set
            {
                _gCycle = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GreenCycle = (ushort)_gCycle;
                OnPropertyChanged();
            }
        }
        private int _gPrior { get; set; }
        public int GPrior
        {
            get
            {
                return _gPrior;
            }
            set
            {
                _gPrior = value;
                prioritySet(value, "G");


            }
        }
        private int _RonTime;

        public int ROnTime
        {
            get { return _RonTime; }
            set
            {
                _RonTime = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].ROnTime = (ushort)_RonTime;
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
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].ROffTime = (ushort)_RoffTime;
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
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].RIntensity = ushort.Parse(_rIntensity);
                OnPropertyChanged();
            }

        }
        private int _rCycle;

        public int RCycle
        {
            get { return _rCycle; }
            set
            {
                _rCycle = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].RedCycle = (ushort)_rCycle;
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
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BOnTime = (ushort)_BonTime;
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
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BOffTime = (ushort)_BoffTime;
                OnPropertyChanged();
            }
        }
        private int _bCycle;

        public int BCycle
        {
            get { return _bCycle; }
            set
            {
                _bCycle = value;
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BlueCycle = (ushort)_bCycle;
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
                ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BIntensity = ushort.Parse(_bIntensity);
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
                int checkIndex = int.Parse(LEDS[selectedIndex]); //TODO error index out of range, no led selected



                GreenIntensity = ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GIntensity.ToString();
                GOnTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GOnTime;
                GOffTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GOffTime;
                GCycle = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].GreenCycle;


                RedIntensity = ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].RIntensity.ToString();
                ROnTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].ROnTime;
                ROffTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].ROffTime;
                RCycle = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].RedCycle;

                BlueIntensity = ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BIntensity.ToString();
                BOnTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BOnTime;
                BOffTime = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BOffTime;
                BCycle = (int)ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].BlueCycle;

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
        public RelayCommand StartLed { get; set; }
        public RelayCommand StopLed { get; set; }
        public RelayCommand Priorities { get; set; }

        public ExperimentStore ExperimentLocal { get; set; }

        public LEDParameterViewModel(ExperimentStore ExperimentSingleton)
        {
            ExperimentLocal = ExperimentSingleton;
            check();
            SelectedIndex = 0;

            Save = new RelayCommand(o => save(), o => true);
            Cancel = new RelayCommand(o => this.cancel(o), o => true);
            StartLed = new RelayCommand(o => startStop(), o => true);
            StopLed = new RelayCommand(o => startStop(), o => true);
            Priorities = new RelayCommand(o => prioritySet(o, ""), o => true);
        }
        public void prioritySet(object o, string s)
        {
            var x = ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityOne;
            switch (s)
            {
                case "G":
                    switch (o)
                    {
                        case 1: ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityOne = 'G'; break;
                        case 2: ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityTwo = 'G'; break;
                        case 3: ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityThree = 'G'; break;
                    }
                    break;
                case "R":
                    switch (o)
                    {
                        case 1: ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityOne = 'R'; break;
                        case 2: ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityTwo = 'R'; break;
                        case 3: ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityThree = 'R'; break;
                    }
                    break;
                case "B":
                    switch (o)
                    {
                        case 1: ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityOne = 'B'; break;
                        case 2: ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityTwo = 'B'; break;
                        case 3: ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].PriorityThree = 'B'; break;
                    }
                    break;
                default: break;

            }

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
        public void startStop()
        {
            if (Green)
            {
                ExperimentLocal.serialPortWrapper.SendData.Add('L');
                ExperimentLocal.serialPortWrapper.SendData.Add(ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].Name[0]);
                ExperimentLocal.serialPortWrapper.SendData.Add('G');
            }
            if (Red)
            {
                ExperimentLocal.serialPortWrapper.SendData.Add('L');
                ExperimentLocal.serialPortWrapper.SendData.Add(ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].Name[0]);
                ExperimentLocal.serialPortWrapper.SendData.Add('R');
            }
            if (Blue)
            {
                ExperimentLocal.serialPortWrapper.SendData.Add('L');
                ExperimentLocal.serialPortWrapper.SendData.Add(ExperimentLocal.ledParameters[int.Parse(LEDS[selectedIndex])].Name[0]);
                ExperimentLocal.serialPortWrapper.SendData.Add('B');
            }
            ExperimentLocal.serialPortWrapper.send();

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
