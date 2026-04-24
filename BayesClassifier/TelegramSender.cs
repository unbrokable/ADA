using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramSender
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendTextAsync(string botToken, string chatId, string text, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("Telegram bot token is not configured.", nameof(botToken));
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                throw new ArgumentException("Telegram chat id is not configured.", nameof(chatId));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Message text cannot be empty.", nameof(text));
            }

            string requestUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";
            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", text }
            }))
            using (HttpResponseMessage response = await HttpClient.PostAsync(requestUrl, content, cancellationToken).ConfigureAwait(false))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode || !responseBody.Contains("\"ok\":true"))
                {
                    throw new InvalidOperationException(
                        $"Telegram API request failed with status code {(int)response.StatusCode}: {responseBody}");
                }
            }
        }
    }
}
