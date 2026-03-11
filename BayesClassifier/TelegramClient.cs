using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendTextMessageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Message text cannot be empty.", nameof(text));
            }

            string botToken = ConfigurationManager.AppSettings["TelegramBotToken"];
            string chatId = ConfigurationManager.AppSettings["TelegramChatId"];

            if (string.IsNullOrWhiteSpace(botToken) || string.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException("TelegramBotToken and TelegramChatId must be configured in App.config.");
            }

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", text)
            });

            HttpResponseMessage response = await HttpClient
                .PostAsync($"https://api.telegram.org/bot{botToken}/sendMessage", content)
                .ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!response.IsSuccessStatusCode || !responseBody.Contains("\"ok\":true"))
            {
                throw new InvalidOperationException($"Telegram API request failed: {responseBody}");
            }
        }
    }
}
