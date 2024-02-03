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

            //if (!serialPort.IsOpen)
            //{
            //    serialPort.BaudRate = 9600;
            //    serialPort.PortName = "COM3";
            //    serialPort.NewLine = "\r\n";
            //    serialPort.ReadTimeout = 500;
            //    serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataRecieved);



            //}

        }
        private void OnDataRecievedHere(object sender, SerialDataReceivedEventArgs e)
        {
            var serialDevice = sender as SerialPort;
            var indata = serialDevice.ReadLine();
            UserEntryRead = "Reply: " + indata.ToString();
            Trace.WriteLine(" Herro" + indata.ToString());
            Thread.Sleep(50);
        }
        public void Enter()
        {

            UserEntryRead = "User: " + UserEntry;
            _port.Write(UserEntry); //enter is happening twice
            Thread.Sleep(50);
            UserEntry = null;
            //Trace.WriteLine(UserEntryRead);


        }
    }
}
