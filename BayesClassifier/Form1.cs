using System;
using System.Configuration;
using System.Threading.Tasks;
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
            PredictionLabel.Text = $"Spam = {spam}  Not spam = {notSpam} \n{message}";

            await TrySendPredictionToTelegramAsync(Message.Text, spam, notSpam, message);
        }

        private void FillChart()
        {
            for (int i = 0; i <= 10; i++)
            {
                Chart.Series[0].Points.AddXY(i*10 + "%", Classifier.CalculatePercentageRigthAnswers(i * 10));
            }
        }

        private async Task TrySendPredictionToTelegramAsync(string sourceMessage, double spam, double notSpam, string prediction)
        {
            string botToken = GetAppSettingOrEnv("TelegramBotToken", "TELEGRAM_BOT_TOKEN");
            string chatId = GetAppSettingOrEnv("TelegramChatId", "TELEGRAM_CHAT_ID");
            if (String.IsNullOrWhiteSpace(botToken) || String.IsNullOrWhiteSpace(chatId))
            {
                PredictionLabel.Text += "\nTelegram: skipped (configure token and chat id).";
                return;
            }

            string telegramText =
                $"Incoming text:\n{sourceMessage}\n\nPrediction: {prediction}\nSpam={spam:F6}\nNotSpam={notSpam:F6}";

            try
            {
                await TelegramService.SendMessageAsync(botToken, chatId, telegramText);
                PredictionLabel.Text += "\nTelegram: sent.";
            }
            catch (Exception ex)
            {
                PredictionLabel.Text += $"\nTelegram: failed ({ex.Message}).";
            }
        }

        private static string GetAppSettingOrEnv(string appSettingKey, string envKey)
        {
            string environmentValue = Environment.GetEnvironmentVariable(envKey);
            if (!String.IsNullOrWhiteSpace(environmentValue))
            {
                return environmentValue;
            }

            return ConfigurationManager.AppSettings[appSettingKey];
        }
    }
}
