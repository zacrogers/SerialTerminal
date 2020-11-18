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
        int baudRate;


        public SerialTerminal()
        {
            InitializeComponent();

            /* Add valid baud rates */
            for (int i = 0; i < BaudRates.Length; i++)
            {
                baudComboBox.Items.Add(BaudRates[i]);
            }

            GetAvailableComPorts();
        }

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


        private void ConnectButtonClick(object sender, EventArgs e)
        {

        }

        private void BaudRateComboBoxChanged(object sender, EventArgs e)
        {

        }

        private void ComPortComboBoxChanged(object sender, EventArgs e)
        {

        }
    }
}
