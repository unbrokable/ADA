using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramSender
    {
        private const string BotTokenEnvironmentVariable = "TELEGRAM_BOT_TOKEN";
        private const string ChatIdEnvironmentVariable = "TELEGRAM_CHAT_ID";

        public static async Task SendTextMessageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Telegram text cannot be empty.", nameof(text));
            }

            string botToken = GetRequiredEnvironmentVariable(BotTokenEnvironmentVariable);
            string chatId = GetRequiredEnvironmentVariable(ChatIdEnvironmentVariable);

            using (var httpClient = new HttpClient())
            using (var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", text)
            }))
            {
                string url = $"https://api.telegram.org/bot{botToken}/sendMessage";
                HttpResponseMessage response = await httpClient.PostAsync(url, content).ConfigureAwait(false);
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Telegram API returned {(int)response.StatusCode}: {responseBody}");
                }
            }
        }

        private static string GetRequiredEnvironmentVariable(string name)
        {
            string value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"Missing required environment variable: {name}");
            }

            return value.Trim();
        }
    }
}
