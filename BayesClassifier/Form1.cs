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

        private async void SendTelegramButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Message.Text))
            {
                return;
            }

            if (!TelegramMessageSender.IsConfigured)
            {
                MessageBox.Show(
                    "Configure TelegramBotToken and TelegramChatId in App.config before sending.",
                    "Telegram is not configured",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SendTelegramButton.Enabled = false;

            try
            {
                await TelegramMessageSender.SendTextAsync(BuildTelegramMessage());
                MessageBox.Show("Text was sent to Telegram.", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Telegram send failed: {exception.Message}",
                    "Telegram",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SendTelegramButton.Enabled = true;
            }
        }

        private string BuildTelegramMessage()
        {
            return String.IsNullOrWhiteSpace(PredictionLabel.Text) || PredictionLabel.Text == "Prediction:"
                ? Message.Text
                : $"{Message.Text}{Environment.NewLine}{Environment.NewLine}{PredictionLabel.Text}";
        }

        private void FillChart()
        {
            for (int i = 0; i <= 10; i++)
            {
                Chart.Series[0].Points.AddXY(i*10 + "%", Classifier.CalculatePercentageRigthAnswers(i * 10));
            }
        }
    }
}
