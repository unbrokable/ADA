using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public static class TelegramTextSender
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendTextAsync(string botToken, string chatId, string text)
        {
            if (String.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("Telegram bot token is required.", nameof(botToken));
            }

            if (String.IsNullOrWhiteSpace(chatId))
            {
                throw new ArgumentException("Telegram chat id is required.", nameof(chatId));
            }

            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Message text is required.", nameof(text));
            }

            var endpoint = new Uri($"https://api.telegram.org/bot{botToken.Trim()}/sendMessage");
            var values = new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId.Trim()),
                new KeyValuePair<string, string>("text", text)
            };

            using (var content = new FormUrlEncodedContent(values))
            using (var response = await HttpClient.PostAsync(endpoint, content).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return;
                }

                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new InvalidOperationException(
                    $"Telegram sendMessage failed: {(int)response.StatusCode} {response.ReasonPhrase}. {responseBody}");
            }
        }
    }
}
