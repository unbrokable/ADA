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
            string botToken = ConfigurationManager.AppSettings["TelegramBotToken"];
            string chatId = ConfigurationManager.AppSettings["TelegramChatId"];

            if (string.IsNullOrWhiteSpace(botToken) || string.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException("Configure TelegramBotToken and TelegramChatId in App.config.");
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Message text is empty.", nameof(text));
            }

            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["chat_id"] = chatId,
                ["text"] = text
            }))
            {
                HttpResponseMessage response = await HttpClient.PostAsync(
                    $"https://api.telegram.org/bot{botToken}/sendMessage",
                    content).ConfigureAwait(false);

                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode || !responseBody.Contains("\"ok\":true"))
                {
                    throw new InvalidOperationException($"Telegram API returned an error: {responseBody}");
                }
            }
        }
    }
}
