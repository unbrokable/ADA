using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramService
    {
        private static readonly HttpClient Client = new HttpClient();

        public static async Task SendMessageAsync(string botToken, string chatId, string text)
        {
            if (String.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("Telegram bot token is required.", nameof(botToken));
            }

            if (String.IsNullOrWhiteSpace(chatId))
            {
                throw new ArgumentException("Telegram chat id is required.", nameof(chatId));
            }

            string endpoint = $"https://api.telegram.org/bot{botToken}/sendMessage";
            using (var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", text)
            }))
            {
                HttpResponseMessage response = await Client.PostAsync(endpoint, content).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new InvalidOperationException(
                        $"Telegram API request failed with status {(int)response.StatusCode}: {responseText}");
                }
            }
        }
    }
}
