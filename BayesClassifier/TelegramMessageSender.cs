using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramMessageSender
    {
        private static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        public static async Task SendTextAsync(string text)
        {
            string botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
            string chatId = Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID");

            if (String.IsNullOrWhiteSpace(botToken) || String.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException("Missing TELEGRAM_BOT_TOKEN or TELEGRAM_CHAT_ID environment variables.");
            }

            string endpoint = $"https://api.telegram.org/bot{botToken}/sendMessage";
            using (var payload = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", text }
            }))
            {
                using (HttpResponseMessage response = await HttpClient.PostAsync(endpoint, payload).ConfigureAwait(false))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        throw new InvalidOperationException(
                            $"Telegram API error {(int)response.StatusCode} ({response.ReasonPhrase}): {responseBody}");
                    }
                }
            }
        }
    }
}
