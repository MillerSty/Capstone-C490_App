using C490_App.Core;
using C490_App.Services;
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
            initSerial(_port);
        }
        public void initSerial(SerialPort serialPort)
        {
            if (!serialPort.IsOpen)
            {
                serialPort.BaudRate = 9600;
                serialPort.PortName = "COM3";
                serialPort.NewLine = "\r\n";
                serialPort.ReadTimeout = 500;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataRecieved);

                serialPort.Open();


            }

        }
        private void OnDataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            var serialDevice = sender as SerialPort;
            var indata = serialDevice.ReadExisting();
            Trace.WriteLine(indata.ToString());
            Thread.Sleep(50);
        }
        public void Enter()
        {

            UserEntryRead = UserEntry;
            UserEntry = "";
            Trace.WriteLine(UserEntryRead);


        }
    }
}
