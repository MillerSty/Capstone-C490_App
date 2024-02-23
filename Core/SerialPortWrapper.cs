﻿using Microsoft.Win32;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Windows;

namespace C490_App.Core
{
    public class SerialPortWrapper : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String e)
        {
            Trace.WriteLine("Changed property!! " + e);
            if (e.Equals("sendChar"))
            {
                Trace.WriteLine("We got it baby?");
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e));
        }
        private String _debugInfo;
        public String debugInfo
        {
            get { return _debugInfo; }
            set
            {
                _debugInfo = value;
                OnPropertyChanged("Text");
            }
        }
        private String _experimentData;
        public String ExperimentData
        {
            get { return _experimentData; }
            set
            {
                _experimentData = value;
                OnPropertyChanged("Text2");
            }
        }
        private String _ssendData;
        public String SSendData
        {
            get { return _ssendData; }
            set
            {
                _ssendData = value;
                OnPropertyChanged("sendChar");
            }
        }
        private List<char> _sendData = new List<char>();
        public List<char> SendData
        {
            get
            {
                // Trace.WriteLine("getting lc"); 
                return _sendData;
            }
            set
            {
                //Trace.WriteLine("setting lc");
                _sendData = value;
                OnPropertyChanged("");
            }
        }
        public void send()
        {
            //while (true)
            //{
            foreach (char c in SendData)
            {
                Trace.WriteLine("Writing " + c);
                this.writeChar(c);
                Thread.Sleep(20);
            }
            SendData.Clear();


            //}


        }

        private SerialPort _port;
        public SerialPort SerialPort
        {
            get { return _port; }
            set { _port = value; }
        }

        public SerialPortWrapper(SerialPort serialPort)
        {
            SerialPort = serialPort;

        }


        /// <summary>
        /// This is for initializing the serial port
        /// </summary>
        public void initSerial()
        {
            String comPort = "COM3";
            //if (!SerialPort.IsOpen)
            //{
            //will use ComPortNames[0] in future to set String comPort
            List<String> comPortEnum = ComPortNames(); //try catch for no return?
            foreach (var com in comPortEnum)
            {
                if (com != null)
                {
                    comPort = com;
                }
            }
            //"COM3";
            SerialPort.PortName = comPort;
            SerialPort.BaudRate = 9600;
            SerialPort.NewLine = "\r\n";
            SerialPort.StopBits = StopBits.One;
            SerialPort.Parity = Parity.None;
            SerialPort.ReceivedBytesThreshold = 50;
            SerialPort.DataBits = 8;
            //SerialPort.ReadTimeout = 10;
            SerialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataRecieved);
            SerialPort.ErrorReceived += new SerialErrorReceivedEventHandler(OnErrorRecieved);
            // }
        }
        public bool Open()
        {

            if (!isOpen())
            {
                try
                {
                    SerialPort.Open();
                    SerialPort.DiscardInBuffer();
                    SerialPort.DiscardOutBuffer();
                }
                catch (Exception ex)
                {
                    var e = ex;
                    MessageBox errorBox;
                    MessageBox.Show(ex.Message, ex.Source);

                    //ex source is source of error
                    //ex message is message of error
                    //TODO this becomes a message box
                    Trace.WriteLine("Error with serial");
                }
            }
            return isOpen();

        }
        public void Close()
        {
            try
            {
                SerialPort.DiscardInBuffer();
                SerialPort.DiscardOutBuffer();
                SerialPort.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message, ex.Source);
            }

        }

        public void writeChar(char data)
        {
            SerialPort.Write(data.ToString());

        }
        public void writeString(string data)
        {
            SerialPort.Write(data);
        }
        public bool isOpen()
        {
            return SerialPort.IsOpen;
        }

        public void assignEventHandler()
        {

        }
        /// <summary>
        /// This handles receiving data from the mCU
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SerialDataReceivedEventArgs</param>
        private void OnDataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            var serialDevice = sender as SerialPort;
            try
            {
                //{ char[] indata=new char[64];
                //     serialDevice.Read(indata,0,serialDevice.BytesToRead);
                var indata = serialDevice.ReadLine();
                Trace.Write("*Hw debug. Bytes to Read: ");
                Trace.WriteLine(serialDevice.BytesToRead.ToString());
                debugInfo = indata;
                //Trace.WriteLine(indata.ToString());
            }
            catch (Exception err)
            {
                Trace.WriteLine(err.ToString());
            }
        }
        private void OnErrorRecieved(object sender, SerialErrorReceivedEventArgs e)
        {
            //var serialDevice = sender as SerialPort;
            try
            {
                //{ char[] indata=new char[64];
                //     serialDevice.Read(indata,0,serialDevice.BytesToRead);
                // var indata = serialDevice.ReadLine();
                Trace.Write("*Hw debug. Error received " + e.ToString());
                //Trace.WriteLine(serialDevice.BytesToRead.ToString());
                //debugInfo = indata;
                //Trace.WriteLine(indata.ToString());
            }
            catch (Exception err)
            {
                Trace.WriteLine(err.ToString());
            }
        }

        /// <summary>
        /// Used for getting comports used by VID/PID
        /// </summary>
        /// <param name="VID"></param>
        /// <param name="PID"></param>
        /// <returns></returns>
        //private String vid = "2341";
        //private String pid = "0043";
        private String vid = "04D8";
        private String pid = "00DD";
        private String SerialNumber = "950323034373518061D1";

        static List<string> ComPortNames()
        {
            //String vid = "2341";
            //String pid = "0043";
            String vid = "04D8";
            String pid = "00DD";
            String SerialNumber = "950323034373518061D1";

            String pattern = String.Format("^VID_{0}.PID_{1}", vid, pid);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();

            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");

            foreach (String s3 in rk2.GetSubKeyNames())
            {

                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    //if subkey names= serial then we good
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }
    }
}
