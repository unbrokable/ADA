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
            string text = Message.Text;
            if (String.IsNullOrWhiteSpace(text))
            {
                PredictionLabel.Text = "Please enter text before sending to Telegram.";
                return;
            }

            SendTelegramButton.Enabled = false;
            try
            {
                await TelegramClient.SendTextMessageAsync(text.Trim());
                PredictionLabel.Text = "Message sent to Telegram.";
            }
            catch (Exception ex)
            {
                PredictionLabel.Text = $"Telegram error: {ex.Message}";
            }
            finally
            {
                SendTelegramButton.Enabled = true;
            }
        }
    }
}
