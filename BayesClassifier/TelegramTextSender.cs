using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramTextSender
    {
        public static async Task SendAsync(string message, string tokenOverride, string chatIdOverride)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be empty.", nameof(message));
            }

            string botToken = FirstNonEmpty(
                tokenOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"),
                ConfigurationManager.AppSettings["TelegramBotToken"]);

            string chatId = FirstNonEmpty(
                chatIdOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID"),
                ConfigurationManager.AppSettings["TelegramChatId"]);

            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new InvalidOperationException("Missing Telegram bot token. Provide --telegram-token, TELEGRAM_BOT_TOKEN, or App.config key TelegramBotToken.");
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException("Missing Telegram chat id. Provide --telegram-chat-id, TELEGRAM_CHAT_ID, or App.config key TelegramChatId.");
            }

            string endpoint = string.Format("https://api.telegram.org/bot{0}/sendMessage", botToken);
            string payload = string.Format("chat_id={0}&text={1}", Uri.EscapeDataString(chatId), Uri.EscapeDataString(message));

            using (var client = new HttpClient())
            using (var content = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded"))
            {
                HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Telegram API request failed with status {0}: {1}",
                            (int)response.StatusCode,
                            responseBody));
                }
            }
        }

        private static string FirstNonEmpty(params string[] values)
        {
            if (values == null)
            {
                return null;
            }

            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }
    }
}
