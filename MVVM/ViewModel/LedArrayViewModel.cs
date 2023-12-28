using C490_App.Core;
using C490_App.MVVM.Model;
using C490_App.Services;
using System.Collections.ObjectModel;

namespace C490_App.MVVM.ViewModel
{


    public class LedArrayViewModel : ViewModelBase
    {

        public RelayCommand ledSelect { get; set; }
        public RelayCommand doNothing { get; set; }
        public bool controlCheck { get; set; }


        private ObservableCollection<LEDParameter> lEDParameters { get; set; }
        public ObservableCollection<bool> isSelected { get; set; }
        ObservableCollection<LEDParameter> ledParameters { get; set; }

        ExperimentStore ExperimentLocal { get; set; }
        public LedArrayViewModel(ExperimentStore ExperimentSingleton)
        {
            ExperimentLocal = ExperimentSingleton;
            isSelected = new ObservableCollection<bool>();
            ledParameters = new ObservableCollection<LEDParameter>(initLedArray());
            ledSelect = new RelayCommand(o => SelectLed(o), o => true);
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
                    bool set = !ledParameters[k].isSelected;
                    this.ledParameters[k].isSelected = set;
                    this.isSelected[k] = set;
                    k += int.Parse(parameterSubString[1]);
                }
            }
            else
            {
                for (int i = int.Parse(parameterSubString[0]); i < int.Parse(parameterSubString[1]); i++)
                {
                    bool set = !ledParameters[i].isSelected;
                    this.ledParameters[i].isSelected = set;
                    this.isSelected[i] = set;
                }
            }
            //this is to test datapassing 
            ExperimentLocal.ledParameters = ledParameters;
        }

        internal List<LEDParameter> initLedArray()
        {
            List<LEDParameter> ledParameters = new List<LEDParameter>();

            for (int i = 0; i < 50; i++)
            {
                ledParameters.Add(new LEDParameter(false, Convert.ToUInt32(i)));
                this.isSelected.Add(false);
            }

            return ledParameters;
        }

    }
}