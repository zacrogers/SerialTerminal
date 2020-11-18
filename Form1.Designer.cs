
namespace SerialTerminal
{
    partial class SerialTerminal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.baudComboBox = new System.Windows.Forms.ComboBox();
            this.comPortComboBox = new System.Windows.Forms.ComboBox();
            this.connectionButton = new System.Windows.Forms.Button();
            this.baudRateLabel = new System.Windows.Forms.Label();
            this.comPortLabel = new System.Windows.Forms.Label();
            this.receivedTextBox = new System.Windows.Forms.RichTextBox();
            this.sendTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // baudComboBox
            // 
            this.baudComboBox.FormattingEnabled = true;
            this.baudComboBox.Location = new System.Drawing.Point(586, 36);
            this.baudComboBox.Name = "baudComboBox";
            this.baudComboBox.Size = new System.Drawing.Size(121, 23);
            this.baudComboBox.TabIndex = 1;
            this.baudComboBox.SelectedIndexChanged += new System.EventHandler(this.BaudRateComboBoxChanged);
            // 
            // comPortComboBox
            // 
            this.comPortComboBox.FormattingEnabled = true;
            this.comPortComboBox.Location = new System.Drawing.Point(586, 102);
            this.comPortComboBox.Name = "comPortComboBox";
            this.comPortComboBox.Size = new System.Drawing.Size(121, 23);
            this.comPortComboBox.TabIndex = 2;
            this.comPortComboBox.SelectedIndexChanged += new System.EventHandler(this.ComPortComboBoxChanged);
            // 
            // connectionButton
            // 
            this.connectionButton.Location = new System.Drawing.Point(586, 153);
            this.connectionButton.Name = "connectionButton";
            this.connectionButton.Size = new System.Drawing.Size(121, 31);
            this.connectionButton.TabIndex = 3;
            this.connectionButton.Text = "Connect";
            this.connectionButton.UseVisualStyleBackColor = true;
            this.connectionButton.Click += new System.EventHandler(this.ConnectButtonClick);
            // 
            // baudRateLabel
            // 
            this.baudRateLabel.AutoSize = true;
            this.baudRateLabel.Location = new System.Drawing.Point(586, 18);
            this.baudRateLabel.Name = "baudRateLabel";
            this.baudRateLabel.Size = new System.Drawing.Size(60, 15);
            this.baudRateLabel.TabIndex = 4;
            this.baudRateLabel.Text = "Baud Rate";
            // 
            // comPortLabel
            // 
            this.comPortLabel.AutoSize = true;
            this.comPortLabel.Location = new System.Drawing.Point(586, 84);
            this.comPortLabel.Name = "comPortLabel";
            this.comPortLabel.Size = new System.Drawing.Size(60, 15);
            this.comPortLabel.TabIndex = 5;
            this.comPortLabel.Text = "COM Port";
            // 
            // receivedTextBox
            // 
            this.receivedTextBox.Location = new System.Drawing.Point(12, 12);
            this.receivedTextBox.Name = "receivedTextBox";
            this.receivedTextBox.Size = new System.Drawing.Size(560, 183);
            this.receivedTextBox.TabIndex = 6;
            this.receivedTextBox.Text = "";
            // 
            // sendTextBox
            // 
            this.sendTextBox.Location = new System.Drawing.Point(12, 214);
            this.sendTextBox.Name = "sendTextBox";
            this.sendTextBox.Size = new System.Drawing.Size(560, 23);
            this.sendTextBox.TabIndex = 7;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(586, 214);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(121, 23);
            this.sendButton.TabIndex = 8;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            // 
            // SerialTerminal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(729, 259);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.sendTextBox);
            this.Controls.Add(this.receivedTextBox);
            this.Controls.Add(this.comPortLabel);
            this.Controls.Add(this.baudRateLabel);
            this.Controls.Add(this.connectionButton);
            this.Controls.Add(this.comPortComboBox);
            this.Controls.Add(this.baudComboBox);
            this.Name = "SerialTerminal";
            this.Text = "SerialTerminal";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox baudComboBox;
        private System.Windows.Forms.ComboBox comPortComboBox;
        private System.Windows.Forms.Button connectionButton;
        private System.Windows.Forms.Label baudRateLabel;
        private System.Windows.Forms.Label comPortLabel;
        private System.Windows.Forms.RichTextBox receivedTextBox;
        private System.Windows.Forms.TextBox sendTextBox;
        private System.Windows.Forms.Button sendButton;
    }
}

