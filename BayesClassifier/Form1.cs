using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private readonly TelegramMessageSender telegramSender;

        public Form1()
        {
            InitializeComponent();
            FillChart();
            TelegramStatusLabel.Text = "Telegram: configure TELEGRAM_BOT_TOKEN and TELEGRAM_CHAT_ID";

            if (TelegramMessageSender.TryCreate(out TelegramMessageSender sender, out string validationError))
            {
                telegramSender = sender;
                TelegramStatusLabel.Text = "Telegram: ready";
                SendTelegramButton.Enabled = true;
            }
            else
            {
                SendTelegramButton.Enabled = false;
                TelegramStatusLabel.Text = $"Telegram: {validationError}";
            }
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
            await SendMessageToTelegramAsync();
        }

        private async Task SendMessageToTelegramAsync()
        {
            if (telegramSender == null)
            {
                TelegramStatusLabel.Text = "Telegram: sender not configured";
                return;
            }

            string messageText = Message.Text?.Trim();
            if (String.IsNullOrWhiteSpace(messageText))
            {
                TelegramStatusLabel.Text = "Telegram: message text is empty";
                return;
            }

            SendTelegramButton.Enabled = false;
            TelegramStatusLabel.Text = "Telegram: sending...";

            try
            {
                await telegramSender.SendTextAsync(messageText);
                TelegramStatusLabel.Text = "Telegram: sent";
            }
            catch (Exception ex)
            {
                TelegramStatusLabel.Text = $"Telegram: failed ({ex.Message})";
            }
            finally
            {
                SendTelegramButton.Enabled = true;
            }
        }
    }
}
