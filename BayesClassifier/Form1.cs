using System;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            FillChart();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Message.Text))
            {
                return;
            }

            (double spam, double notSpam) = Classifier.Determine(Reader.ReadData(), Message.Text);
            string message = spam > notSpam ? "Spam!!!" : "Not spam";
            PredictionLabel.Text = $"Spam = {spam}  Not spam = {notSpam} \n{message}";
        }

        private void FillChart()
        {
            for (int i = 0; i <= 10; i++)
            {
                Chart.Series[0].Points.AddXY(i*10 + "%", Classifier.CalculatePercentageRigthAnswers(i * 10));
            }
        }

        private async void SendTelegramButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Message.Text))
            {
                PredictionLabel.Text = "Message is empty.";
                return;
            }

            ToggleButtons(isEnabled: false);
            try
            {
                await TelegramClient.SendTextMessageAsync(Message.Text.Trim());
                PredictionLabel.Text = "Message sent to Telegram.";
            }
            catch (Exception ex)
            {
                PredictionLabel.Text = $"Telegram send failed: {ex.Message}";
            }
            finally
            {
                ToggleButtons(isEnabled: true);
            }
        }

        private void ToggleButtons(bool isEnabled)
        {
            ValidateButton.Enabled = isEnabled;
            SendTelegramButton.Enabled = isEnabled;
        }
    }
}
