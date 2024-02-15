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
            ledSelect = new RelayCommand(o => SelectLed(o), o => true);
        }

        //TODO add parameter list to this
        public void SelectLed(object commandParameter)
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
                    SelectColLED(selected.index, selected.index + 40);
                }
            }


            ExperimentLocal.UpdateLEDS(isSelected);
        }
        private void SelectRowLED(int startIndex)
        {
            int counter = startIndex;
            bool set = controlCheckRow[startIndex];
            for (int i = (counter * 10); i < (10 * counter + 10); i++)
            {
                this.isSelected[i] = set;
            }

        }
        private void SelectColLED(int startIndex, int endIndex)
        {
            bool set = controlCheckCol[startIndex];
            for (int i = startIndex; i <= endIndex; i += 10)
            {
                this.isSelected[i] = set;
            }

        }
        private void SelectAllLED(int startIndex, int endIndex)
        {
            bool set = controlCheckAll;
            for (int i = startIndex; i < endIndex; i++)
            {
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