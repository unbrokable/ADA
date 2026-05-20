
namespace BayesClassifier
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.Message = new System.Windows.Forms.RichTextBox();
            this.ValidateButton = new System.Windows.Forms.Button();
            this.PredictionLabel = new System.Windows.Forms.Label();
            this.Chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.BotTokenLabel = new System.Windows.Forms.Label();
            this.BotTokenTextBox = new System.Windows.Forms.TextBox();
            this.ChatIdLabel = new System.Windows.Forms.Label();
            this.ChatIdTextBox = new System.Windows.Forms.TextBox();
            this.SendTelegramButton = new System.Windows.Forms.Button();
            this.TelegramStatusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).BeginInit();
            this.SuspendLayout();
            // 
            // Message
            // 
            this.Message.Location = new System.Drawing.Point(41, 31);
            this.Message.Name = "Message";
            this.Message.Size = new System.Drawing.Size(392, 300);
            this.Message.TabIndex = 0;
            this.Message.Text = "";
            // 
            // ValidateButton
            // 
            this.ValidateButton.Location = new System.Drawing.Point(46, 340);
            this.ValidateButton.Name = "ValidateButton";
            this.ValidateButton.Size = new System.Drawing.Size(126, 43);
            this.ValidateButton.TabIndex = 1;
            this.ValidateButton.Text = "Validate";
            this.ValidateButton.UseVisualStyleBackColor = true;
            this.ValidateButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // PredictionLabel
            // 
            this.PredictionLabel.AutoSize = true;
            this.PredictionLabel.Location = new System.Drawing.Point(292, 340);
            this.PredictionLabel.Name = "PredictionLabel";
            this.PredictionLabel.Size = new System.Drawing.Size(57, 13);
            this.PredictionLabel.TabIndex = 2;
            this.PredictionLabel.Text = "Prediction:";
            // 
            // Chart
            // 
            chartArea1.Name = "ChartArea1";
            this.Chart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.Chart.Legends.Add(legend1);
            this.Chart.Location = new System.Drawing.Point(493, 31);
            this.Chart.Name = "Chart";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Percentage of correct answers";
            this.Chart.Series.Add(series1);
            this.Chart.Size = new System.Drawing.Size(702, 300);
            this.Chart.TabIndex = 3;
            this.Chart.Text = "chart1";
            // 
            // BotTokenLabel
            // 
            this.BotTokenLabel.AutoSize = true;
            this.BotTokenLabel.Location = new System.Drawing.Point(493, 342);
            this.BotTokenLabel.Name = "BotTokenLabel";
            this.BotTokenLabel.Size = new System.Drawing.Size(59, 13);
            this.BotTokenLabel.TabIndex = 4;
            this.BotTokenLabel.Text = "Bot token:";
            // 
            // BotTokenTextBox
            // 
            this.BotTokenTextBox.Location = new System.Drawing.Point(558, 337);
            this.BotTokenTextBox.Name = "BotTokenTextBox";
            this.BotTokenTextBox.Size = new System.Drawing.Size(255, 20);
            this.BotTokenTextBox.TabIndex = 5;
            this.BotTokenTextBox.UseSystemPasswordChar = true;
            // 
            // ChatIdLabel
            // 
            this.ChatIdLabel.AutoSize = true;
            this.ChatIdLabel.Location = new System.Drawing.Point(830, 342);
            this.ChatIdLabel.Name = "ChatIdLabel";
            this.ChatIdLabel.Size = new System.Drawing.Size(46, 13);
            this.ChatIdLabel.TabIndex = 6;
            this.ChatIdLabel.Text = "Chat ID:";
            // 
            // ChatIdTextBox
            // 
            this.ChatIdTextBox.Location = new System.Drawing.Point(879, 337);
            this.ChatIdTextBox.Name = "ChatIdTextBox";
            this.ChatIdTextBox.Size = new System.Drawing.Size(140, 20);
            this.ChatIdTextBox.TabIndex = 7;
            // 
            // SendTelegramButton
            // 
            this.SendTelegramButton.Location = new System.Drawing.Point(1036, 335);
            this.SendTelegramButton.Name = "SendTelegramButton";
            this.SendTelegramButton.Size = new System.Drawing.Size(159, 26);
            this.SendTelegramButton.TabIndex = 8;
            this.SendTelegramButton.Text = "Send to Telegram";
            this.SendTelegramButton.UseVisualStyleBackColor = true;
            this.SendTelegramButton.Click += new System.EventHandler(this.SendTelegramButton_Click);
            // 
            // TelegramStatusLabel
            // 
            this.TelegramStatusLabel.AutoSize = true;
            this.TelegramStatusLabel.Location = new System.Drawing.Point(493, 372);
            this.TelegramStatusLabel.MaximumSize = new System.Drawing.Size(700, 0);
            this.TelegramStatusLabel.Name = "TelegramStatusLabel";
            this.TelegramStatusLabel.Size = new System.Drawing.Size(87, 13);
            this.TelegramStatusLabel.TabIndex = 9;
            this.TelegramStatusLabel.Text = "Telegram status:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1217, 407);
            this.Controls.Add(this.TelegramStatusLabel);
            this.Controls.Add(this.SendTelegramButton);
            this.Controls.Add(this.ChatIdTextBox);
            this.Controls.Add(this.ChatIdLabel);
            this.Controls.Add(this.BotTokenTextBox);
            this.Controls.Add(this.BotTokenLabel);
            this.Controls.Add(this.Chart);
            this.Controls.Add(this.PredictionLabel);
            this.Controls.Add(this.ValidateButton);
            this.Controls.Add(this.Message);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.Chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox Message;
        private System.Windows.Forms.Button ValidateButton;
        private System.Windows.Forms.Label PredictionLabel;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart;
        private System.Windows.Forms.Label BotTokenLabel;
        private System.Windows.Forms.TextBox BotTokenTextBox;
        private System.Windows.Forms.Label ChatIdLabel;
        private System.Windows.Forms.TextBox ChatIdTextBox;
        private System.Windows.Forms.Button SendTelegramButton;
        private System.Windows.Forms.Label TelegramStatusLabel;
    }
}

