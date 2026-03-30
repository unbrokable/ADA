using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private readonly TelegramSender telegramSender;

        public Form1()
        {
            InitializeComponent();
            telegramSender = TelegramSender.CreateFromEnvironmentOrConfig();
            FillChart();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Message.Text))
            {
                return;
            }

            (double spam, double notSpam) = Classifier.Determine(Reader.ReadData(), Message.Text);
            string message = spam > notSpam ? "Spam!!!" : "Not spam";
            string prediction = $"Spam = {spam}  Not spam = {notSpam} \n{message}";
            PredictionLabel.Text = prediction;

            await SendToTelegramAsync(prediction);
        }

        private void FillChart()
        {
            for (int i = 0; i <= 10; i++)
            {
                Chart.Series[0].Points.AddXY(i*10 + "%", Classifier.CalculatePercentageRigthAnswers(i * 10));
            }
        }

        private async Task SendToTelegramAsync(string prediction)
        {
            if (telegramSender == null)
            {
                return;
            }

            string telegramText = $"Text: {Message.Text}\n{prediction}";
            TelegramSendResult result = await telegramSender.SendTextAsync(telegramText);
            PredictionLabel.Text += result.Success
                ? "\nTelegram: sent"
                : $"\nTelegram: {result.ErrorMessage}";
        }
    }
}
