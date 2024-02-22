using C490_App.Core;
using System.ComponentModel;


namespace C490_App.MVVM.ViewModel
{
    internal class DebugViewModel : ViewModelBase
    {

        private string _userEntry = "";
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
            UserEntryRead = _ExperimentStoreSerialWrapper.debugInfo + "\n";
        }
        private SerialPortWrapper _experimentStore;
        public SerialPortWrapper _ExperimentStoreSerialWrapper
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
            _ExperimentStoreSerialWrapper = store.serialPortWrapper;
            //_ExperimentStoreSerialWrapper.PropertyChanged += A_PropertyChanged;

            enter = new RelayCommand(o => Enter(), o => true);
        }
        public void Enter()
        {
            bool check = UserEntry.Equals("");
            if (!UserEntry.Equals(""))
            {
                if (_ExperimentStoreSerialWrapper.Open())
                {
                    char[] c = UserEntry.ToCharArray();
                    foreach (char c2 in c)
                    {
                        _ExperimentStoreSerialWrapper.SendData.Add(c2);

                    }
                    _ExperimentStoreSerialWrapper.send();
                    UserEntryRead = "User: " + UserEntry + "\n";

                    Thread.Sleep(50);
                    UserEntry = "";
                }

            }
        }
    }
}
