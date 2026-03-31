using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramNotifier
    {
        private static readonly HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.telegram.org/")
        };

        public static async Task<(bool isSuccess, string error)> SendTextAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return (false, "Message is empty.");
            }

            string botToken = GetConfigValue("TELEGRAM_BOT_TOKEN", "TelegramBotToken");
            string chatId = GetConfigValue("TELEGRAM_CHAT_ID", "TelegramChatId");

            if (string.IsNullOrWhiteSpace(botToken) || string.IsNullOrWhiteSpace(chatId))
            {
                return (false, "Telegram configuration is missing. Set TELEGRAM_BOT_TOKEN and TELEGRAM_CHAT_ID (or App.config values).");
            }

            using (var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", text)
            }))
            using (HttpResponseMessage response = await HttpClient.PostAsync($"bot{botToken}/sendMessage", requestBody).ConfigureAwait(false))
            {
                string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    return (true, string.Empty);
                }

                return (false, $"Telegram API error {(int)response.StatusCode}: {responseContent}");
            }
        }

        private static string GetConfigValue(string environmentVariableName, string appSettingKey)
        {
            string environmentValue = Environment.GetEnvironmentVariable(environmentVariableName);
            if (!string.IsNullOrWhiteSpace(environmentValue))
            {
                return environmentValue;
            }

            return ConfigurationManager.AppSettings[appSettingKey];
        }
    }
}
