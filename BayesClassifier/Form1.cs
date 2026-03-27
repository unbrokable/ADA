using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient TelegramHttpClient = new HttpClient();

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

            ValidateButton.Enabled = false;

            try
            {
                string text = Message.Text.Trim();
                (double spam, double notSpam) = Classifier.Determine(Reader.ReadData(), text);
                string prediction = spam > notSpam ? "Spam!!!" : "Not spam";
                string telegramStatus = await SendTextToTelegramAsync(text);
                PredictionLabel.Text = $"Spam = {spam}  Not spam = {notSpam} \n{prediction}\nTelegram: {telegramStatus}";
            }
            finally
            {
                ValidateButton.Enabled = true;
            }
        }

        private void FillChart()
        {
            for (int i = 0; i <= 10; i++)
            {
                Chart.Series[0].Points.AddXY(i*10 + "%", Classifier.CalculatePercentageRigthAnswers(i * 10));
            }
        }

        private static async Task<string> SendTextToTelegramAsync(string text)
        {
            string botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
            string chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");
            if (String.IsNullOrWhiteSpace(botToken) || String.IsNullOrWhiteSpace(chatId))
            {
                return "skipped (set TELEGRAM_BOT_TOKEN and TELEGRAM_CHAT_ID)";
            }

            string endpoint = $"https://api.telegram.org/bot{botToken}/sendMessage";
            var payload = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", text }
            });

            try
            {
                using (HttpResponseMessage response = await TelegramHttpClient.PostAsync(endpoint, payload))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return "sent";
                    }

                    string responseText = await response.Content.ReadAsStringAsync();
                    return $"failed ({(int)response.StatusCode}): {responseText}";
                }
            }
            catch (Exception ex)
            {
                return $"error: {ex.Message}";
            }
        }
    }
}
