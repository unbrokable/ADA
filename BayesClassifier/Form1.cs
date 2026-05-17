using System;
using System.Configuration;
using System.Windows.Forms;

namespace BayesClassifier
{
    public partial class Form1 : Form
    {
        private const string TelegramBotTokenSettingName = "TelegramBotToken";
        private const string TelegramChatIdSettingName = "TelegramChatId";
        private const string TelegramBotTokenEnvironmentVariable = "TELEGRAM_BOT_TOKEN";
        private const string TelegramChatIdEnvironmentVariable = "TELEGRAM_CHAT_ID";

        public Form1()
        {
            InitializeComponent();
            FillChart();
            LoadTelegramSettings();
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
            TelegramStatusLabel.Text = "Telegram: sending...";
            SendTelegramButton.Enabled = false;

            try
            {
                await TelegramMessageSender.SendTextAsync(TelegramBotTokenTextBox.Text, TelegramChatIdTextBox.Text, Message.Text);
                TelegramStatusLabel.Text = "Telegram: message sent.";
            }
            catch (Exception ex)
            {
                TelegramStatusLabel.Text = "Telegram: failed to send.";
                MessageBox.Show(this, ex.Message, "Telegram send failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void LoadTelegramSettings()
        {
            TelegramBotTokenTextBox.Text = ReadTelegramSetting(TelegramBotTokenSettingName, TelegramBotTokenEnvironmentVariable);
            TelegramChatIdTextBox.Text = ReadTelegramSetting(TelegramChatIdSettingName, TelegramChatIdEnvironmentVariable);
        }

        private static string ReadTelegramSetting(string appSettingName, string environmentVariableName)
        {
            string environmentValue = Environment.GetEnvironmentVariable(environmentVariableName);
            if (!String.IsNullOrWhiteSpace(environmentValue))
            {
                return environmentValue;
            }

            return ConfigurationManager.AppSettings[appSettingName] ?? String.Empty;
        }
    }
}
