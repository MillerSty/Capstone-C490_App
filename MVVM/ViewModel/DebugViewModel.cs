using C490_App.Core;
using System.Diagnostics;
using System.IO.Ports;


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
                _userEntryRead += value + "\n";
                OnPropertyChanged();

            }
        }

        SerialPort _port;
        public RelayCommand enter { get; set; }
        public DebugViewModel(ExperimentStore store)
        {

            int pause = 1000;
            enter = new RelayCommand(o => Enter(), o => true);
            initSerial(store);
        }
        public void initSerial(ExperimentStore store)
        {
            _port = store.mySerialPort;
            _port.DataReceived += new SerialDataReceivedEventHandler(OnDataRecievedHere);
            if (!_port.IsOpen) { _port.Open(); }
        }
        private void OnDataRecievedHere(object sender, SerialDataReceivedEventArgs e)
        {
            var serialDevice = sender as SerialPort;
            /*
             * Exception thrown: 'System.TimeoutException' in System.IO.Ports.dll
An unhandled exception of type 'System.TimeoutException' occurred in System.IO.Ports.dll
The operation has timed out. -> seems to be from multiple onDataReceived starving when other takes over?
             */
            var indata = serialDevice.ReadLine();
            if (indata.ToString().Equals("D"))
            {
                indata = serialDevice.ReadLine();
                UserEntryRead = "Reply: " + indata.ToString();
                Trace.WriteLine(" Herro" + indata.ToString());
                Thread.Sleep(50);
            }
        }
        public void Enter()
        {

            UserEntryRead = "User: " + UserEntry;
            if (!UserEntry.Equals(null))
            {
                _port.Write(UserEntry); //enter is happening twice
                Thread.Sleep(50);
                UserEntry = null;
            }
        }
    }
}
