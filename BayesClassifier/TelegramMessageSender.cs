using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal sealed class TelegramMessageSender
    {
        private const string TelegramApiBaseUrl = "https://api.telegram.org";
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly string botToken;
        private readonly string chatId;

        private TelegramMessageSender(string botToken, string chatId)
        {
            this.botToken = botToken;
            this.chatId = chatId;
        }

        public static bool TryCreate(out TelegramMessageSender sender, out string validationError)
        {
            string botToken = ReadSetting("TELEGRAM_BOT_TOKEN", "TelegramBotToken");
            string chatId = ReadSetting("TELEGRAM_CHAT_ID", "TelegramChatId");

            if (String.IsNullOrWhiteSpace(botToken))
            {
                sender = null;
                validationError = "missing TELEGRAM_BOT_TOKEN";
                return false;
            }

            if (String.IsNullOrWhiteSpace(chatId))
            {
                sender = null;
                validationError = "missing TELEGRAM_CHAT_ID";
                return false;
            }

            sender = new TelegramMessageSender(botToken.Trim(), chatId.Trim());
            validationError = null;
            return true;
        }

        public async Task SendTextAsync(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Message text cannot be empty.", nameof(text));
            }

            string endpoint = $"{TelegramApiBaseUrl}/bot{botToken}/sendMessage";
            var payload = new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", text)
            };

            using (var content = new FormUrlEncodedContent(payload))
            using (HttpResponseMessage response = await HttpClient.PostAsync(endpoint, content).ConfigureAwait(false))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Telegram API call failed with status {(int)response.StatusCode}. Response: {responseBody}");
                }
            }
        }

        private static string ReadSetting(string environmentVariableName, string appConfigKey)
        {
            string envValue = Environment.GetEnvironmentVariable(environmentVariableName);
            if (!String.IsNullOrWhiteSpace(envValue))
            {
                return envValue;
            }

            return ConfigurationManager.AppSettings[appConfigKey];
        }
    }
}
