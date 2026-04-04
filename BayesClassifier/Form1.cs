using System;
using System.Configuration;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private readonly TelegramClient _telegramClient;

        public Form1()
        {
            InitializeComponent();
            FillChart();
            _telegramClient = CreateTelegramClient();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Message.Text))
            {
                return;
            }

            (double spam, double notSpam, string message) = Predict(Message.Text);
            PredictionLabel.Text = $"Spam = {spam}  Not spam = {notSpam} \n{message}";
        }

        private void FillChart()
        {
            for (int i = 0; i <= 10; i++)
            {
                Chart.Series[0].Points.AddXY(i*10 + "%", Classifier.CalculatePercentageRigthAnswers(i * 10));
            }
        }

        private static TelegramClient CreateTelegramClient()
        {
            string botToken = ConfigurationManager.AppSettings["TelegramBotToken"];
            string chatId = ConfigurationManager.AppSettings["TelegramChatId"];

            if (String.IsNullOrWhiteSpace(botToken) || String.IsNullOrWhiteSpace(chatId))
            {
                return null;
            }

            return new TelegramClient(botToken, chatId);
        }

        private static (double spam, double notSpam, string message) Predict(string messageText)
        {
            (double spam, double notSpam) = Classifier.Determine(Reader.ReadData(), messageText);
            string message = spam > notSpam ? "Spam!!!" : "Not spam";
            return (spam, notSpam, message);
        }

        private async void SendTelegramButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Message.Text))
            {
                MessageBox.Show("Please enter text to send.", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_telegramClient == null)
            {
                MessageBox.Show(
                    "Telegram is not configured. Fill TelegramBotToken and TelegramChatId in App.config.",
                    "Telegram",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            (double spam, double notSpam, string resultMessage) = Predict(Message.Text);
            PredictionLabel.Text = $"Spam = {spam}  Not spam = {notSpam} \n{resultMessage}";

            string telegramMessage =
                $"Message:\n{Message.Text}\n\nPrediction:\nSpam={spam}\nNotSpam={notSpam}\nResult={resultMessage}";

            SendTelegramButton.Enabled = false;
            try
            {
                await _telegramClient.SendTextAsync(telegramMessage);
                MessageBox.Show("Message sent to Telegram.", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send message to Telegram.\n{ex.Message}", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SendTelegramButton.Enabled = true;
            }
        }
    }
}
