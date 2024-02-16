using C490_App.Core;
using System.ComponentModel;


namespace C490_App.MVVM.ViewModel
{
    internal class DebugViewModel : ViewModelBase
    {

        private string _userEntry;
        public string UserEntry
        {
            get { return _userEntry; }
            set
            {
                _userEntry = value;
                OnPropertyChanged();

            }
        }

        private string _userEntryRead;
        public string UserEntryRead
        {
            get { return _userEntryRead; }
            set
            {
                _userEntryRead += value;
                OnPropertyChanged();

            }
        }
        private void A_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UserEntryRead = _ExperimentStore.debugInfo + "\n";
        }
        private SerialPortWrapper _experimentStore;
        public SerialPortWrapper _ExperimentStore
        {
            get
            {
                return _experimentStore;
            }
            set
            {
                _experimentStore = value;
            }
        }


        public SerialPortWrapper _port { get; set; }
        public RelayCommand enter { get; set; }
        public DebugViewModel(ExperimentStore store)
        {
            _ExperimentStore = store.serialPortWrapper;
            _ExperimentStore.PropertyChanged += A_PropertyChanged;

            enter = new RelayCommand(o => Enter(), o => true);
        }
        public void Enter()
        {
            if (!UserEntry.Equals(null))
            {
                if (_ExperimentStore.Open())
                {
                    _ExperimentStore.SerialPort.Write(UserEntry);
                    UserEntryRead = "User: " + UserEntry + "\n";

                    Thread.Sleep(50);
                    UserEntry = null;
                }

            }
        }
    }
}
