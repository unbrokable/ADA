using System;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private const string BotTokenEnvironmentVariable = "TELEGRAM_BOT_TOKEN";
        private const string ChatIdEnvironmentVariable = "TELEGRAM_CHAT_ID";

        public Form1()
        {
            InitializeComponent();
            BotTokenTextBox.Text = Environment.GetEnvironmentVariable(BotTokenEnvironmentVariable) ?? String.Empty;
            ChatIdTextBox.Text = Environment.GetEnvironmentVariable(ChatIdEnvironmentVariable) ?? String.Empty;
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
            SendTelegramButton.Enabled = false;
            TelegramStatusLabel.Text = "Sending...";

            try
            {
                await TelegramTextSender.SendTextAsync(BotTokenTextBox.Text, ChatIdTextBox.Text, Message.Text);
                TelegramStatusLabel.Text = "Text sent to Telegram.";
            }
            catch (Exception ex)
            {
                TelegramStatusLabel.Text = $"Telegram error: {ex.Message}";
            }
            finally
            {
                SendTelegramButton.Enabled = true;
            }
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
