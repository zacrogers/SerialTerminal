using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialTerminal
{
    public partial class SerialTerminal : Form
    {
        static readonly int[] BaudRates = {300, 600, 1200, 2400, 4800, 9600, 14400, 
                                           19200, 28800, 31250, 38400, 57600, 115200};

        static string[] comPorts = SerialPort.GetPortNames();
        private static SerialPort _serialPort;

        bool connected = false;
        int baudRate = 0;
        string comPort = string.Empty;

        private delegate void SafeCallDelegate(string text);

        public SerialTerminal()
        {
            InitializeComponent();
            _serialPort = new SerialPort();

            /* Add valid baud rates */
            for (int i = 0; i < BaudRates.Length; i++)
            {
                baudComboBox.Items.Add(BaudRates[i]);
            }

            GetAvailableComPorts();
        }

        #region Serial port methods

        private void GetAvailableComPorts()
        {
            Array.Clear(comPorts, 0, comPorts.Length);

            comPorts = SerialPort.GetPortNames();

            if (comPorts.Length == 0)
                return;

            for (int i = 0; i < comPorts.Length; i++)
            {
                comPortComboBox.Items.Add(comPorts[i]);
            }
        }

        private void SerialDataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) 
        {
            if(connected)
            {
                string lineReceived = _serialPort.ReadLine();
                ReceivedTextBoxWrite(lineReceived);
            }

        }

        #endregion

        #region GUI methods

        private void ReceivedTextBoxWrite(string text)
        {
            if(receivedTextBox.InvokeRequired)
            {
                var d = new SafeCallDelegate(ReceivedTextBoxWrite);
                receivedTextBox.Invoke(d, new object[] { text });
            }
            else
            {
                receivedTextBox.AppendText(text);
            }
        }

        private void ConnectButtonClick(object sender, EventArgs e)
        {
            if(BaudRates.Contains(baudRate) && comPort.Contains("COM") && !connected)
            {
                _serialPort.PortName = comPort;
                _serialPort.BaudRate = baudRate;
                _serialPort.Open();
                _serialPort.DataReceived += SerialDataReceived;

                connected = true;

                connectionButton.Text = "Disconnect";
            }
            else if(connected)
            {
                _serialPort.Close();
                connected = false;
                connectionButton.Text = "Connect";
            }


        }

        private void BaudRateComboBoxChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            string sli = cmb.SelectedItem.ToString();
            baudRate = Int32.Parse(sli);
        }

        private void ComPortComboBoxChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            comPort = cmb.SelectedItem.ToString();
        }
        #endregion
    }
}
