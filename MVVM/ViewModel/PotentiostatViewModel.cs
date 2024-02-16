using C490_App.Core;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;


namespace C490_App.MVVM.ViewModel
{
    public class PotentiostatViewModel : ViewModelBase
    {
        public ObservableCollection<String> potsInactive { get; set; } = new ObservableCollection<String>();

        //public ObservableCollection<String> _potsActive { get; set; }
        public ObservableCollection<String> potsActive { get; set; } = new ObservableCollection<String>();
        public RelayCommand switchL { get; set; }

        private String _selectedPot;
        public String SelectedPotName
        {
            get
            {
                return _selectedPot;
            }
            set
            {
                _selectedPot = value;
                OnPropertyChanged();
            }
        }
        ExperimentStore ExperimentLocal { get; set; }
        public PotentiostatViewModel(ExperimentStore ExperimentSingleton)
        {
            ExperimentLocal = ExperimentSingleton;
            //potsInactive = new ObservableCollection<String>();
            //potsActive = new ObservableCollection<String>();
            switchL = new RelayCommand(
                o => SwitchList(o, potsActive, potsInactive),
                o => true
            );
            populateInactiveList(potsInactive);

        }

        /// <summary>
        /// Initializes our List Selection UI with 50 pots
        /// </summary>
        /// <param name="o"></param>
        public void populateInactiveList(ObservableCollection<string> uiList)
        {
            for (int i = 0; i < 50; i++)
            {
                uiList.Add($"{i}");
            }

        }

        /// <summary>
        /// Switches a potentiostat from inactive/active to other list
        /// </summary>
        /// <param name="o">unused</param>
        /// <param name="potsActive">Active Potentiostats List</param>
        /// <param name="potsInactive">Inactive Potentiostats List</param>
        public void SwitchList(Object o, ObservableCollection<String> potsActive, ObservableCollection<String> potsInactive)
        {

            if (potsActive.Contains(SelectedPotName))
            {
                potsInactive.Add(SelectedPotName);
                potsActive.Remove(SelectedPotName);
            }
            else if (potsInactive.Contains(SelectedPotName))
            {
                potsActive.Add(SelectedPotName);
                potsInactive.Remove(SelectedPotName);
            }
            // Sort both lists
            SortObservableCollection(potsActive);
            SortObservableCollection(potsInactive);
            ExperimentLocal.UpdatePots(potsActive);
        }

        /// <summary>
        /// Sorts Active/Inactive Lists
        /// </summary>
        /// <param name="collection"> Active/Inactive List</param>
        private void SortObservableCollection(ObservableCollection<string> collection)
        {
            List<string> sortedList = new List<string>(collection);

            // Custom sort logic to handle numeric part
            sortedList.Sort((s1, s2) =>
            {
                int num1 = int.Parse(Regex.Match(s1, @"\d+").Value);
                int num2 = int.Parse(Regex.Match(s2, @"\d+").Value);

                return num1.CompareTo(num2);
            });

            collection.Clear();
            foreach (var item in sortedList)
            {
                collection.Add(item);
            }
        }

    }
}
