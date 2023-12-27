using C490_App.Core;
using C490_App.MVVM.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace C490_App.MVVM.ViewModel
{
    public class LEDParameterViewModel
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
            set { _rIntensity = value; }

        }
        public string _bIntensity { get; set; }
        public string BlueIntensity
        {
            get { return _bIntensity; }
            set { _bIntensity = value; }

        }
        ObservableCollection<LEDParameter> lEDs { get; set; }

        public RelayCommand Save { get; set; }

        public RelayCommand Cancel { get; set; }

        public LEDParameterViewModel()
        {

            Save = new RelayCommand(o => save(), o => true);

            Cancel = new RelayCommand(o => cancel(), o => true);
        }

        public void save()
        {
            Trace.WriteLine("Saving");
        }
        public void cancel() { Trace.WriteLine("Canceling"); }




    }
}
