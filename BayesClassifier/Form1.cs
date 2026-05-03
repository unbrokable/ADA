using System;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private readonly TelegramMessageSender telegramMessageSender = new TelegramMessageSender();

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

        private async void SendToTelegramButton_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Message.Text))
            {
                MessageBox.Show("Enter a message before sending it to Telegram.", "Telegram");
                return;
            }

            try
            {
                SendTelegramButton.Enabled = false;
                await telegramMessageSender.SendAsync(Message.Text);
                MessageBox.Show("Message sent to Telegram.", "Telegram");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Telegram send failed");
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
