using C490_App.Core;
using System.Collections.ObjectModel;

namespace C490_App.MVVM.ViewModel
{


    public class LedArrayViewModel : ViewModelBase
    {

        public RelayCommand ledSelect { get; set; }

        /// <summary>
        /// these Lists represents the outside checkboxes of the array
        /// it is used for control logic of what is selected using LEDSelect and appropriate called functions
        /// controlCheckAll is select all
        /// controlCheckRow is row checkbox's
        /// controlCheckCol is column checkbox's
        /// </summary>
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
            isSelected = new ObservableCollection<bool>(initIsSelected()); //this could probably be handled more eloquently
            ledSelect = new RelayCommand(o => SelectLed2(o), o => true);
        }

        //SelectLed Should be deprecated now
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

        //Replaces deprecated selectLed
        //TODO add parameter list to this
        public void SelectLed2(object commandParameter)
        {
            SelectAllLED(0, 50);

            foreach (var selected in controlCheckRow.Select((truth, index) => (truth, index)))
            {
                if (selected.truth)
                {
                    SelectRowLED(selected.index);
                }
            }
            foreach (var selected in controlCheckCol.Select((truth, index) => (truth, index)))
            {
                if (selected.truth)
                {
                    SelectColLED(selected.index * 5, selected.index * 5 + 5);
                }
            }


            ExperimentLocal.UpdateLEDS(isSelected);
        }
        private void SelectRowLED(int startIndex)
        {
            int counter = startIndex;

            for (int i = 0; i < 10; i++)
            {
                bool set = controlCheckRow[startIndex];
                this.isSelected[counter] = set;
                counter += 5;
            }

        }
        private void SelectColLED(int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                bool set = controlCheckCol[i / 5];
                this.isSelected[i] = set;
            }

        }
        private void SelectAllLED(int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                bool set = controlCheckAll;
                this.isSelected[i] = set;
            }

        }


        internal List<bool> initIsSelected()
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