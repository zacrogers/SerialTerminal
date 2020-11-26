using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
        static readonly string PATH = Directory.GetCurrentDirectory();
        static readonly int[] BaudRates = {300, 600, 1200, 2400, 4800, 9600, 14400,
                                           19200, 28800, 31250, 38400, 57600, 115200};

        static readonly Color _redBtn       = Color.FromArgb(181, 38, 38);
        static readonly Color _darkRedBtn   = Color.FromArgb(128, 24, 24);
        static readonly Color _greenBtn     = Color.FromArgb(67, 176, 46);
        static readonly Color _darkGreenBtn = Color.FromArgb(67, 176, 46);

        static List<string> comPorts = new List<string>();
        static SerialPort _serialPort;
        Timer serialCheckTimer;

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
            this.FormClosing += new FormClosingEventHandler(OnFormClosing);

            /* Init button colours */
            connectionButton.BackColor = _greenBtn;
            clearButton.BackColor      = _greenBtn;
            sendButton.BackColor       = _greenBtn;

            _serialPort = new SerialPort();

            IsConnected = false;

            // Add valid baud rates 
            for (int i = 0; i < BaudRates.Length; i++)
            {
                baudComboBox.Items.Add(BaudRates[i]);
            }

            GetAvailableComPorts();

            // Load previous messages
            StreamReader file = new StreamReader($"{PATH}//previous_commands.txt");
            string line = string.Empty;

            while((line = file.ReadLine()) != null)
            {
                previousSentMessages.Push(line);
            }
            
            serialCheckTimer = new Timer();
            serialCheckTimer.Interval = 1000;
            serialCheckTimer.Start();
            serialCheckTimer.Tick += new EventHandler(CheckComPortsConnected);           
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

        /// <summary>
        /// Takes data received from connected serial device and puts it in text box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SerialDataReceived(object sender, EventArgs e) 
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
                        DisconnectDevice();
                    }
                }
            }
        }
        /// <summary>
        /// Method for checking if a serial device has been removed while connected.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void CheckComPortsConnected(object source, EventArgs e)
        {
            if (IsConnected)
            {
                var currConnections = SerialPort.GetPortNames().OfType<string>().ToList();

                if(!currConnections.Contains(comPort))
                {
                    DisconnectDevice();
                }
            }
        }

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
                    DisconnectDevice();
                }
            }
        }

        private void DisconnectDevice()
        {
            connectionButton.Text = "Connect";
            connectionButton.BackColor = _greenBtn;
            IsConnected = false;
            comPort = string.Empty;
            comPortComboBox.Text = string.Empty;
            _serialPort.Close();
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
            if(BaudRates.Contains(baudRate) && comPort.Contains("COM") && !IsConnected && !_serialPort.IsOpen)
            {
                _serialPort.PortName = comPort;
                _serialPort.BaudRate = baudRate;
                _serialPort.Open();
                _serialPort.DataReceived += SerialDataReceived;

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
                DisconnectDevice();
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

        /// <summary>
        /// Refreshes the available com ports when combo box is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            }
            else
            {
                connectionButton.Text = "Connect";
                connectionButton.BackColor = _greenBtn;
                IsConnected = false;
            }
        }

        /// <summary>
        /// Handles key presses for sending message to serial device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (IsConnected)
                {
                    SendMessage();
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

        /// <summary>
        /// Saves all the previously sent messages to a text file.
        /// </summary>
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            using (StreamWriter file = new StreamWriter($"{PATH}//previous_commands.txt"))
            {
                // Push all previous messages back to stack
                while (previousPoppedMessages.Count > 0){
                    previousSentMessages.Push(previousPoppedMessages.Pop());
                }

                // Needs to be reversed to save in correct order
                Stack<string> reversed = new Stack<string>();

                while (previousSentMessages.Count > 0)
                {
                    reversed.Push(previousSentMessages.Pop());
                }

                // Write messages to file
                while (reversed.Count > 0){
                    file.WriteLine(reversed.Pop());
                }
            }      
        }
        #endregion
    }
}
