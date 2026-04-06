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

        private async void SendToTelegramButton_Click(object sender, EventArgs e)
        {
            string messageText = Message.Text;
            if (string.IsNullOrWhiteSpace(messageText))
            {
                MessageBox.Show(this, "Please enter text to send.", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string botToken = ConfigurationManager.AppSettings["TelegramBotToken"];
            string chatId = ConfigurationManager.AppSettings["TelegramChatId"];

            try
            {
                SetTelegramButtonState(false);
                await TelegramMessageSender.SendTextMessageAsync(botToken, chatId, messageText);
                MessageBox.Show(this, "Telegram text message sent successfully.", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"Failed to send message to Telegram.\n{ex.Message}", "Telegram", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetTelegramButtonState(true);
            }
        }

        private void SetTelegramButtonState(bool enabled)
        {
            SendToTelegramButton.Enabled = enabled;
            SendToTelegramButton.Text = enabled ? "Send to Telegram" : "Sending...";
        }
    }
}
