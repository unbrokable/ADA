using System;
using System.Configuration;
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
            string text = Message.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("Enter text before sending it to Telegram.", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string botToken = ConfigurationManager.AppSettings["TelegramBotToken"];
            string chatId = ConfigurationManager.AppSettings["TelegramChatId"];

            try
            {
                SendTelegramButton.Enabled = false;
                await TelegramSender.SendTextAsync(botToken, chatId, text);
                MessageBox.Show("Text was sent to Telegram.", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send Telegram message.\n{ex.Message}", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SendTelegramButton.Enabled = true;
            }
        }
    }
}
