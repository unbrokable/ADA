using System;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private readonly TelegramTextSender telegramTextSender = new TelegramTextSender();

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

            SetTelegramSendingState(true);

            try
            {
                await telegramTextSender.SendAsync(Message.Text);
                TelegramStatusLabel.Text = "Telegram: sent";
            }
            catch (Exception exception)
            {
                TelegramStatusLabel.Text = "Telegram: not sent";
                MessageBox.Show(
                    exception.Message,
                    "Telegram send failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SetTelegramSendingState(false);
            }
        }

        private void SetTelegramSendingState(bool isSending)
        {
            SendTelegramButton.Enabled = !isSending;
            ValidateButton.Enabled = !isSending;
            TelegramStatusLabel.Text = isSending ? "Telegram: sending..." : TelegramStatusLabel.Text;
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
