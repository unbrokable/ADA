using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal sealed class TelegramTextSender
    {
        private const string BotTokenSettingName = "TelegramBotToken";
        private const string ChatIdSettingName = "TelegramChatId";
        private const string BotTokenEnvironmentName = "TELEGRAM_BOT_TOKEN";
        private const string ChatIdEnvironmentName = "TELEGRAM_CHAT_ID";

        private static readonly HttpClient HttpClient = new HttpClient();

        public async Task SendAsync(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Telegram message text cannot be empty.", nameof(text));
            }

            TelegramConfiguration configuration = TelegramConfiguration.Load();
            if (!configuration.IsConfigured)
            {
                throw new InvalidOperationException(
                    "Set TelegramBotToken and TelegramChatId in App.config, or set TELEGRAM_BOT_TOKEN and TELEGRAM_CHAT_ID environment variables.");
            }

            string endpoint = $"https://api.telegram.org/bot{configuration.BotToken}/sendMessage";
            using (FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", configuration.ChatId),
                new KeyValuePair<string, string>("text", text)
            }))
            {
                using (HttpResponseMessage response = await HttpClient.PostAsync(endpoint, content).ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }

                    string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new InvalidOperationException(
                        $"Telegram API returned {(int)response.StatusCode} ({response.ReasonPhrase}): {TrimResponse(responseText)}");
                }
            }
        }

        private static string ReadSetting(string settingName, string environmentName)
        {
            string environmentValue = Environment.GetEnvironmentVariable(environmentName);
            if (!String.IsNullOrWhiteSpace(environmentValue))
            {
                return environmentValue.Trim();
            }

            string configuredValue = ConfigurationManager.AppSettings[settingName];
            return String.IsNullOrWhiteSpace(configuredValue) ? String.Empty : configuredValue.Trim();
        }

        private static string TrimResponse(string responseText)
        {
            if (String.IsNullOrWhiteSpace(responseText))
            {
                return "empty response";
            }

            const int maxLength = 300;
            string trimmedResponse = responseText.Trim();
            return trimmedResponse.Length <= maxLength
                ? trimmedResponse
                : trimmedResponse.Substring(0, maxLength) + "...";
        }

        private sealed class TelegramConfiguration
        {
            private TelegramConfiguration(string botToken, string chatId)
            {
                BotToken = botToken;
                ChatId = chatId;
            }

            public string BotToken { get; }

            public string ChatId { get; }

            public bool IsConfigured => !String.IsNullOrWhiteSpace(BotToken) && !String.IsNullOrWhiteSpace(ChatId);

            public static TelegramConfiguration Load()
            {
                return new TelegramConfiguration(
                    ReadSetting(BotTokenSettingName, BotTokenEnvironmentName),
                    ReadSetting(ChatIdSettingName, ChatIdEnvironmentName));
            }
        }
    }
}
