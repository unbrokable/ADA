using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public sealed class TelegramTextMessageSender
    {
        private static readonly HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.telegram.org/")
        };

        private readonly string botToken;
        private readonly string chatId;

        public TelegramTextMessageSender()
            : this(
                GetSetting("Telegram.BotToken", "TELEGRAM_BOT_TOKEN"),
                GetSetting("Telegram.ChatId", "TELEGRAM_CHAT_ID"))
        {
        }

        public TelegramTextMessageSender(string botToken, string chatId)
        {
            this.botToken = botToken;
            this.chatId = chatId;
        }

        public bool IsConfigured
            => !String.IsNullOrWhiteSpace(botToken) && !String.IsNullOrWhiteSpace(chatId);

        public async Task SendTextAsync(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Telegram message text cannot be empty.", nameof(text));
            }

            if (!IsConfigured)
            {
                return;
            }

            var values = new Dictionary<string, string>
            {
                ["chat_id"] = chatId,
                ["text"] = text
            };

            using (var content = new FormUrlEncodedContent(values))
            using (HttpResponseMessage response = await HttpClient
                .PostAsync($"bot{botToken}/sendMessage", content)
                .ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        private static string GetSetting(string appSettingsKey, string environmentVariable)
        {
            string value = ConfigurationManager.AppSettings[appSettingsKey];

            return !String.IsNullOrWhiteSpace(value)
                ? value
                : Environment.GetEnvironmentVariable(environmentVariable);
        }
    }
}
