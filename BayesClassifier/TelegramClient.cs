using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal class TelegramClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task SendTextAsync(string botToken, string chatId, string text)
        {
            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("Bot token is required.", nameof(botToken));
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                throw new ArgumentException("Chat ID is required.", nameof(chatId));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Message text is required.", nameof(text));
            }

            string endpoint = $"https://api.telegram.org/bot{botToken}/sendMessage";
            using (var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", text)
            }))
            using (HttpResponseMessage response = await HttpClient.PostAsync(endpoint, content).ConfigureAwait(false))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Telegram API returned {(int)response.StatusCode}: {responseBody}");
                }

                if (responseBody.Contains("\"ok\":false"))
                {
                    throw new InvalidOperationException($"Telegram API error: {responseBody}");
                }
            }
        }
    }
}
