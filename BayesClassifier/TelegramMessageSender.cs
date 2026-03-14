using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramMessageSender
    {
        public static async Task SendAsync(string message)
        {
            var token = GetConfigValue("TELEGRAM_BOT_TOKEN", "TelegramBotToken");
            var chatId = GetConfigValue("TELEGRAM_CHAT_ID", "TelegramChatId");
            var parseMode = GetConfigValue("TELEGRAM_PARSE_MODE", "TelegramParseMode");

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("Telegram bot token is not configured.");
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException("Telegram chat ID is not configured.");
            }

            using (var httpClient = new HttpClient())
            using (var requestBody = new FormUrlEncodedContent(BuildPayload(chatId, message, parseMode)))
            {
                var endpoint = "https://api.telegram.org/bot" + token + "/sendMessage";
                var response = await httpClient.PostAsync(endpoint, requestBody).ConfigureAwait(false);
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        "Telegram API returned " + (int)response.StatusCode + ": " + responseBody);
                }

                if (responseBody.IndexOf("\"ok\":true", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new InvalidOperationException("Telegram API response indicates failure: " + responseBody);
                }
            }
        }

        private static IDictionary<string, string> BuildPayload(string chatId, string message, string parseMode)
        {
            var payload = new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", message }
            };

            if (!string.IsNullOrWhiteSpace(parseMode))
            {
                payload["parse_mode"] = parseMode;
            }

            return payload;
        }

        private static string GetConfigValue(string envKey, string configKey)
        {
            var envValue = Environment.GetEnvironmentVariable(envKey);
            if (!string.IsNullOrWhiteSpace(envValue))
            {
                return envValue.Trim();
            }

            return ConfigurationManager.AppSettings[configKey];
        }
    }
}
