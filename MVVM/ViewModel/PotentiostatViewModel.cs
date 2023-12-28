using C490_App.Core;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace C490_App.MVVM.ViewModel
{
    public class PotentiostatViewModel
    {
        //public List<int> intsy = (List<int>)Enumerable.Range(0, 49);

        public ObservableCollection<String> potsInactive { get; set; }

        public ObservableCollection<String> _potsActive { get; set; }
        public ObservableCollection<String> potsActive { get; set; }
        public RelayCommand switchL { get; set; }
        public String SelectedPot;
        public String SelectedPotName
        {
            get
            {
                return SelectedPot;
            }
            set
            {
                SelectedPot = value;
                //potsActive.Add(SelectedPot);
                //OnPropertyChanged();
            }
        }

        public PotentiostatViewModel()
        {
            potsInactive = new ObservableCollection<String>();
            potsActive = new ObservableCollection<String>();
            switchL = new RelayCommand(
                o => SwitchList(o, potsActive, potsInactive),
                o => true
            );
            populateInactiveList(potsInactive);

        }

        public void populateInactiveList(ObservableCollection<string> o)
        {
            for (int i = 0; i < 50; i++)
            {
                o.Add($"Potetionstat {i}");
            }

        }

        public void SwitchList(Object o, ObservableCollection<String> potsActive, ObservableCollection<String> potsInactive)
        {
            Trace.WriteLine("[x0]");
            Trace.WriteLine("SelectedPot : " + SelectedPotName);

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
        }

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
