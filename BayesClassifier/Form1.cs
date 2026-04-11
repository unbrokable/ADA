using System;
using System.Configuration;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private readonly TelegramClient telegramClient = new TelegramClient();

        public Form1()
        {
            InitializeComponent();
            FillChart();

            BotTokenTextBox.Text = ConfigurationManager.AppSettings["TelegramBotToken"] ?? string.Empty;
            ChatIdTextBox.Text = ConfigurationManager.AppSettings["TelegramChatId"] ?? string.Empty;
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
            string botToken = BotTokenTextBox.Text?.Trim();
            string chatId = ChatIdTextBox.Text?.Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                PredictionLabel.Text = "Message text is required.";
                return;
            }

            SetTelegramControlsEnabled(false);
            PredictionLabel.Text = "Sending message to Telegram...";

            try
            {
                await telegramClient.SendTextAsync(botToken, chatId, text);
                PredictionLabel.Text = "Message sent to Telegram.";
            }
            catch (Exception ex)
            {
                PredictionLabel.Text = $"Telegram send failed: {ex.Message}";
            }
            finally
            {
                SetTelegramControlsEnabled(true);
            }
        }

        private void SetTelegramControlsEnabled(bool isEnabled)
        {
            SendTelegramButton.Enabled = isEnabled;
            BotTokenTextBox.Enabled = isEnabled;
            ChatIdTextBox.Enabled = isEnabled;
        }
    }
}
