using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public static class TelegramMessageSender
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendTextAsync(string botToken, string chatId, string text, CancellationToken cancellationToken = default(CancellationToken))
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

            string requestUri = $"https://api.telegram.org/bot{botToken.Trim()}/sendMessage";
            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["chat_id"] = chatId.Trim(),
                ["text"] = text
            }))
            using (HttpResponseMessage response = await HttpClient.PostAsync(requestUri, content, cancellationToken))
            {
                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    throw new InvalidOperationException($"Telegram sendMessage failed with status {(int)response.StatusCode}: {responseBody}");
                }
            }
        }
    }
}
