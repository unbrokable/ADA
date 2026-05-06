using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private readonly TelegramTextMessageSender telegramSender = new TelegramTextMessageSender();

        public Form1()
        {
            InitializeComponent();
            FillChart();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Message.Text))
            {
                return;
            }

            (double spam, double notSpam) = Classifier.Determine(Reader.ReadData(), Message.Text);
            string prediction = spam > notSpam ? "Spam!!!" : "Not spam";
            string result = $"Spam = {spam}  Not spam = {notSpam} \n{prediction}";

            PredictionLabel.Text = result;

            await SendPredictionToTelegramAsync(
                $"Message:\n{Message.Text}\n\nPrediction:\n{result}");
        }

        private async Task SendPredictionToTelegramAsync(string text)
        {
            if (!telegramSender.IsConfigured)
            {
                PredictionLabel.Text += "\nTelegram is not configured.";
                return;
            }

            try
            {
                await telegramSender.SendTextAsync(text);
                PredictionLabel.Text += "\nSent to Telegram.";
            }
            catch (Exception ex)
            {
                PredictionLabel.Text += $"\nTelegram send failed: {ex.Message}";
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
