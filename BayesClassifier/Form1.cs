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

            ValidateButton.Enabled = false;
            string telegramText = $"Classifier result:{Environment.NewLine}{prediction}{Environment.NewLine}{Environment.NewLine}Input:{Environment.NewLine}{Message.Text}";
            (bool sent, string error) = await TelegramNotifier.SendTextAsync(telegramText);
            if (!sent)
            {
                PredictionLabel.Text = $"{prediction}{Environment.NewLine}Telegram send failed: {error}";
            }

            ValidateButton.Enabled = true;
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
