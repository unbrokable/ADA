using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public sealed class TelegramMessageSender
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task SendAsync(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Message text is required.", nameof(text));
            }

            string botToken = GetRequiredSetting("TelegramBotToken", "TELEGRAM_BOT_TOKEN");
            string chatId = GetRequiredSetting("TelegramChatId", "TELEGRAM_CHAT_ID");

            using (var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", text)
            }))
            {
                HttpResponseMessage response = await Client
                    .PostAsync($"https://api.telegram.org/bot{botToken}/sendMessage", content)
                    .ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new InvalidOperationException(
                        $"Telegram send failed with status {(int)response.StatusCode}: {responseBody}");
                }
            }
        }

        private static string GetRequiredSetting(string appSettingKey, string environmentKey)
        {
            string value = ConfigurationManager.AppSettings[appSettingKey];

            if (String.IsNullOrWhiteSpace(value))
            {
                value = Environment.GetEnvironmentVariable(environmentKey);
            }

            if (String.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"Configure {appSettingKey} in App.config or set the {environmentKey} environment variable.");
            }

            return value;
        }
    }
}
