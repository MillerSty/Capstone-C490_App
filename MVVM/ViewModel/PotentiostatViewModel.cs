using C490_App.Core;
using C490_App.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace C490_App.MVVM.ViewModel
{
    public class PotentiostatViewModel : ViewModelBase
    {
        //public List<int> intsy = (List<int>)Enumerable.Range(0, 49);

        public ObservableCollection<String> potsInactive { get; set; }

        public ObservableCollection<String> _potsActive { get; set; }
        public ObservableCollection<String> potsActive
        {
            get;
            set;
        }
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
        ExperimentStore ExperimentLocal { get; set; }
        public PotentiostatViewModel(ExperimentStore ExperimentSingleton)
        {
            ExperimentLocal = ExperimentSingleton;
            potsInactive = new ObservableCollection<String>();
            potsActive = new ObservableCollection<String>();
            switchL = new RelayCommand(o => switchList(o, potsActive), o => true);

            populateInactiveList(potsInactive);

        }

        public void populateInactiveList(ObservableCollection<string> o)
        {
            for (int i = 0; i < 50; i++)
            {
                o.Add($"Potetionstat {i}");
            }

        }

        public void switchList(Object o, ObservableCollection<String> potsActive)
        {
            Trace.WriteLine("[x0]");
            Trace.WriteLine(potsActive.ToString());
            Trace.WriteLine("SelectedPot : " + SelectedPotName);
            //postActive=SelectedPotName.ToList();
            potsActive.Add(SelectedPotName);
            potsInactive.Remove(SelectedPotName);


        }


    }
}
