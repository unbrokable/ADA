using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public static class TelegramBotClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendTextAsync(string text, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Message text cannot be empty.", nameof(text));
            }

            (string token, string chatId) = GetSettings();
            string endpoint = $"https://api.telegram.org/bot{token}/sendMessage";

            using (var payload = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["chat_id"] = chatId,
                ["text"] = text
            }))
            {
                using (HttpResponseMessage response = await HttpClient.PostAsync(endpoint, payload, cancellationToken))
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException(
                            $"Telegram API returned {(int)response.StatusCode}: {responseBody}");
                    }
                }
            }
        }

        private static (string token, string chatId) GetSettings()
        {
            string token = ReadRequiredSetting("TELEGRAM_BOT_TOKEN", "TelegramBotToken");
            string chatId = ReadRequiredSetting("TELEGRAM_CHAT_ID", "TelegramChatId");
            return (token, chatId);
        }

        private static string ReadRequiredSetting(string envName, string appSettingName)
        {
            string fromEnv = Environment.GetEnvironmentVariable(envName);
            if (!String.IsNullOrWhiteSpace(fromEnv))
            {
                return fromEnv.Trim();
            }

            string fromConfig = ConfigurationManager.AppSettings[appSettingName];
            if (!String.IsNullOrWhiteSpace(fromConfig))
            {
                return fromConfig.Trim();
            }

            throw new InvalidOperationException(
                $"Missing Telegram setting '{envName}' (env) or '{appSettingName}' (App.config).");
        }
    }
}
