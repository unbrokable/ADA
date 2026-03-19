using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramSender
    {
        private static readonly Uri TelegramApiBaseUri = new Uri("https://api.telegram.org/");

        public static async Task SendTextMessageAsync(string botToken, string chatId, string message)
        {
            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("Telegram bot token is required.");
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                throw new ArgumentException("Telegram chat id is required.");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Telegram message text is required.");
            }

            Uri endpoint = new Uri(TelegramApiBaseUri, "bot" + botToken + "/sendMessage");
            var payload = new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", message }
            };

            using (var client = new HttpClient())
            using (var content = new FormUrlEncodedContent(payload))
            using (HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        "Telegram API call failed with status " + (int)response.StatusCode + ". Response: " + responseBody);
                }
            }
        }
    }
}
