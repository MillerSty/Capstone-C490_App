using C490_App.Core;
using C490_App.Services;
using System.Collections.ObjectModel;

namespace C490_App.MVVM.ViewModel
{


    public class LedArrayViewModel : ViewModelBase
    {

        public RelayCommand ledSelect { get; set; }

        //<controlCheck>
        //this List represents the outside checkboxes of the array
        //it is used for control logic of what is selected?
        //0 is select all
        //1-5 is row index
        //6-16 is column index
        public bool controlCheckAll { get; set; }

        public List<bool> controlCheckRow { get; set; } = new List<bool>() { false, false, false, false, false };

        public List<bool> controlCheckCol { get; set; } = new List<bool>() { false, false, false, false, false, false, false, false, false, false };


        public ObservableCollection<bool> isSelected
        {
            get;
            set;
        }
        ExperimentStore ExperimentLocal { get; set; }

        public LedArrayViewModel(ExperimentStore ExperimentSingleton)
        {
            ExperimentLocal = ExperimentSingleton;
            isSelected = new ObservableCollection<bool>(initLedArray()); //this could probably be handled more eloquently
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
                int controlIndex = k;

                for (int i = 0; i < 10; i++)
                {
                    bool set = controlCheckRow[controlIndex];
                    //bool set = !ExperimentLocal.ledParameters[k].IsSelected;
                    this.isSelected[k] = set;
                    k += int.Parse(parameterSubString[1]);
                }
            }
            else
            {
                if (controlCheckAll)
                {
                    for (int i = int.Parse(parameterSubString[0]); i < int.Parse(parameterSubString[1]); i++)
                    {
                        bool set = controlCheckAll;
                        this.isSelected[i] = set;
                    }
                }
                else
                    for (int i = int.Parse(parameterSubString[0]); i < int.Parse(parameterSubString[1]); i++)
                    {
                        bool set;
                        set = controlCheckCol[i / 5];
                        this.isSelected[i] = set;
                    }
            }
            ExperimentLocal.UpdateLEDS(isSelected);
        }

        internal List<bool> initLedArray()
        {
            List<bool> result = new List<bool>();

            for (int i = 0; i < 50; i++)
            {
                result.Add(false);
            }

            return result;
        }

    }
}