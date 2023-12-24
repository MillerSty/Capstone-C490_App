using C490_App.Core;
using C490_App.MVVM.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace C490_App.MVVM.ViewModel
{


    public class LedArrayViewModel
    {

        public RelayCommand ledSelect { get; set; }
        public RelayCommand doNothing { get; set; }
        public bool controlCheck { get; set; }


        ObservableCollection<LEDParameter> lEDParameters { get; set; }
        public ObservableCollection<bool> isSelected { get; set; }
        ObservableCollection<LEDParameter> _ledParameters { get; set; } = new ObservableCollection<LEDParameter>();

        public bool itemZero { get; set; }

        public LedArrayViewModel()
        {
            itemZero = true;
            isSelected = new ObservableCollection<bool>();
            _ledParameters = new ObservableCollection<LEDParameter>(initLedArray());

            doNothing = new RelayCommand(o => empty(o), o => true);
            ledSelect = new RelayCommand(o => SelectLed(o), o => true);


        }
        public void empty(object o)
        {
            this.isSelected[1] = true;
            this.isSelected[1] = false;
            //this.isSelected[1] = true;
            Trace.WriteLine("THIS LED SELECTIONE MPTY  " + this.isSelected[0]);


        }

        /*SelectLed 
         * @binding control checkboxes
         * @param is a tuple i,k,j
         * if j != null we are doing row wise selection where we start at i and increment with k
         * if j==null we can do column wise selection where i is start of for loop and k is ending  
         * we use var set so we can update var bool isSelected , and the bool in LEDParamter collection
         */
        public void SelectLed(object commandParameter)
        {
            String[] parameterSubString = commandParameter.ToString().Split(",");
            if (parameterSubString.Length == 3)
            {
                int k = int.Parse(parameterSubString[0]);
                for (int i = 0; i < 10; i++)
                {
                    bool set = !_ledParameters[k].isSelected;
                    this._ledParameters[k].isSelected = set;
                    this.isSelected[k] = set;
                    k += int.Parse(parameterSubString[1]);
                }
            }
            else
            {
                for (int i = int.Parse(parameterSubString[0]); i < int.Parse(parameterSubString[1]); i++)
                {
                    bool set = !_ledParameters[i].isSelected;
                    this._ledParameters[i].isSelected = set;
                    this.isSelected[i] = set;
                }
            }
        }

        internal List<LEDParameter> initLedArray()
        {
            List<LEDParameter> ledParameters = new List<LEDParameter>();

            for (int i = 0; i < 50; i++)
            {
                ledParameters.Add(new LEDParameter(false, Convert.ToUInt32(i)));
                this.isSelected.Add(false);
            }
            //this.isSelected[42] = true; //this is like a control jawn atm
            UInt32 addr = ledParameters[16].Address;
            bool check = ledParameters[16].isSelected;
            return ledParameters;
        }
    }
}