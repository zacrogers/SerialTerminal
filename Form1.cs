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
        #region Fields
        static readonly int[] BaudRates = {300, 600, 1200, 2400, 4800, 9600, 14400,
                                           19200, 28800, 31250, 38400, 57600, 115200};

        static readonly Color _redBtn       = Color.FromArgb(181, 38, 38);
        static readonly Color _darkRedBtn   = Color.FromArgb(128, 24, 24);
        static readonly Color _greenBtn     = Color.FromArgb(67, 176, 46);
        static readonly Color _darkGreenBtn = Color.FromArgb(67, 176, 46);

        static List<string> comPorts = new List<string>();
        static SerialPort _serialPort;

        int baudRate = 0;
        string comPort = string.Empty;

        bool isConnected;

        /* For remembering previously sent messages */
        Stack<string> previousSentMessages   = new Stack<string>();
        Stack<string> previousPoppedMessages = new Stack<string>();
        #endregion

        /* For sending received serial data to text box safely */
        private delegate void SafeCallDelegate(string text);

        #region Constructor
        public SerialTerminal()
        {
            InitializeComponent();

            /* Init button colours */
            connectionButton.BackColor = _greenBtn;
            clearButton.BackColor      = _greenBtn;
            sendButton.BackColor       = _greenBtn;

            _serialPort = new SerialPort();

            IsConnected = false;

            /* Add valid baud rates */
            for (int i = 0; i < BaudRates.Length; i++)
            {
                baudComboBox.Items.Add(BaudRates[i]);
            }

            GetAvailableComPorts();
        }
        #endregion

        #region Properties
        private bool IsConnected
        {
            get { return isConnected; }
            set 
            {
                isConnected = value;
                
                if(isConnected)
                {
                    sendButton.Enabled = true;
                    sendTextBox.Enabled = true;
                    sendButton.BackColor = _greenBtn;
                }
                else
                {
                    sendButton.Enabled = false;
                    sendTextBox.Enabled = false;
                    sendButton.BackColor = _darkRedBtn;
                }
            }
        }

        #endregion

        #region Serial port methods

        private void GetAvailableComPorts()
        {
            comPorts.Clear();
            comPortComboBox.Items.Clear();

            comPorts = SerialPort.GetPortNames().OfType<string>().ToList();

            if (comPorts.Count == 0)
                return;

            foreach (string port in comPorts)
            {
                comPortComboBox.Items.Add(port);
            }
        }

        private void SerialDataReceived(object sender, SerialDataReceivedEventArgs e) 
        {
            if(IsConnected)
            {
                try 
                {
                    string lineReceived = _serialPort.ReadLine();
                    ReceivedTextBoxWrite(lineReceived);
                }
                catch (Exception ex)
                {
                    if (ex is InvalidOperationException)
                    {

                        connectionButton.Text = "Connect";
                        connectionButton.BackColor = _greenBtn;
                        IsConnected = false;
                        _serialPort.Close();
                    }
                }
            }
        }

        #endregion

        #region Other Methods
        private void SendMessage()
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

            //_serialPort.Write(msg);
            //sendTextBox.Clear();

            // Test to see if device has been removed
            try 
            {
                _serialPort.Write(msg);
                sendTextBox.Clear();
            }
            catch(Exception ex)
            { 
                if(ex is InvalidOperationException)
                {
                    
                    connectionButton.Text = "Connect";
                    connectionButton.BackColor = _greenBtn;
                    IsConnected = false;
                    _serialPort.Close();
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
            if(BaudRates.Contains(baudRate) && comPort.Contains("COM") && !IsConnected)
            {
                _serialPort.PortName = comPort;
                _serialPort.BaudRate = baudRate;
                _serialPort.Open();
                _serialPort.DataReceived += SerialDataReceived;

                // IsOpen should only be used here. IsConnected is used to manage the connection
                // status so the send button can be enabled/disabled
                if(_serialPort.IsOpen)
                {
                    connectionButton.Text = "Disconnect";
                    connectionButton.BackColor = _redBtn;
                    IsConnected = true;
                }
                else 
                {
                    IsConnected = false;
                }

            }
            else if(IsConnected)
            {
                _serialPort.Close();
                connectionButton.Text = "Connect";
                connectionButton.BackColor = _greenBtn;
                IsConnected = false;
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

        private void ComPortComboBoxClicked(object sender, EventArgs e)
        {
            GetAvailableComPorts();
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            receivedTextBox.Clear();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                SendMessage();
                /*
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
                */
            }
            else
            {
                connectionButton.Text = "Connect";
                connectionButton.BackColor = _greenBtn;

                IsConnected = false;
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (IsConnected)
                {
                    SendMessage();
                    /*
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
                    */
                }
                else
                {
                    connectionButton.Text = "Connect";
                    connectionButton.BackColor = _greenBtn;
                    IsConnected = false;
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
