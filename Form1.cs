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
        static SerialPort _serialPort;

        int baudRate = 0;
        string comPort = string.Empty;

        /* For remembering previously sent messages */
        Stack<string> previousSentMessages = new Stack<string>();
        Stack<string> previousPoppedMessages = new Stack<string>();

        /* For sending received serial data to text box safely */
        private delegate void SafeCallDelegate(string text);

        public SerialTerminal()
        {
            InitializeComponent();

            /* Init button colours */
            connectionButton.BackColor = Color.FromArgb(67, 176, 46);
            clearButton.BackColor = Color.FromArgb(67, 176, 46);
            sendButton.BackColor = Color.FromArgb(67, 176, 46);

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
            if(_serialPort.IsOpen)
            {
                try 
                {
                    string lineReceived = _serialPort.ReadLine();
                    ReceivedTextBoxWrite(lineReceived);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
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

        private void ReceivedTextBoxTextChanged(object sender, EventArgs e)
        {
            receivedTextBox.SelectionStart = receivedTextBox.Text.Length;
            receivedTextBox.ScrollToCaret();
        }

        private void ConnectButtonClick(object sender, EventArgs e)
        {
            if(BaudRates.Contains(baudRate) && comPort.Contains("COM") && !_serialPort.IsOpen)
            {
                _serialPort.PortName = comPort;
                _serialPort.BaudRate = baudRate;
                _serialPort.Open();
                _serialPort.DataReceived += SerialDataReceived;

                if(_serialPort.IsOpen)
                {
                    connectionButton.Text = "Disconnect";
                    connectionButton.BackColor = Color.FromArgb(181, 38, 38);
                }

            }
            else if(_serialPort.IsOpen)
            {
                _serialPort.Close();
                connectionButton.Text = "Connect";
                connectionButton.BackColor = Color.FromArgb(67, 176, 46);
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
      
        private void clearButton_Click(object sender, EventArgs e)
        {
            receivedTextBox.Clear();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                string msg = sendTextBox.Text;

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    if (carriageReturnCheckBox.Checked)
                    {
                        msg += '\r';
                    }

                    if (newlineCheckBox.Checked)
                    {
                        msg += '\n';
                    }
                    previousSentMessages.Push(msg);
                }

                _serialPort.Write(msg);
                sendTextBox.Clear();
            }
            else
            {
                connectionButton.Text = "Connect";
                connectionButton.BackColor = Color.FromArgb(67, 176, 46);
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (_serialPort.IsOpen)
                {
                    string msg = sendTextBox.Text;

                    if(!string.IsNullOrWhiteSpace(msg))
                    {
                        if (carriageReturnCheckBox.Checked)
                        {
                            msg += '\r';
                        }

                        if (newlineCheckBox.Checked)
                        {
                            msg += '\n';
                        }

                        previousSentMessages.Push(msg);
                    }
                    
                    _serialPort.Write(msg);            
                    sendTextBox.Clear();
                }
                else
                {
                    connectionButton.Text = "Connect";
                    connectionButton.BackColor = Color.FromArgb(67, 176, 46);
                }
            }
            else if(e.KeyCode == Keys.Up)
            {
                if (previousSentMessages.Count > 0)
                {
                    sendTextBox.Text = previousSentMessages.Pop();
                    previousPoppedMessages.Push(sendTextBox.Text);
                }
                else 
                {
                    sendTextBox.Clear();
                }
            }
            else if(e.KeyCode == Keys.Down)
            {
                if (previousPoppedMessages.Count > 0)
                {
                    sendTextBox.Text = previousPoppedMessages.Pop();
                    previousSentMessages.Push(sendTextBox.Text);
                }
                else
                {
                    sendTextBox.Clear();
                }
            }
        }
        #endregion
    }
}
