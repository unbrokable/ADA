using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public static class TelegramMessageSender
    {
        private const string BotTokenEnvironmentVariable = "TELEGRAM_BOT_TOKEN";
        private const string ChatIdEnvironmentVariable = "TELEGRAM_CHAT_ID";
        private const string BotTokenSetting = "TelegramBotToken";
        private const string ChatIdSetting = "TelegramChatId";

        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendTextAsync(string text, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Text message cannot be empty.", nameof(text));
            }

            string botToken = GetSetting(BotTokenEnvironmentVariable, BotTokenSetting);
            string chatId = GetSetting(ChatIdEnvironmentVariable, ChatIdSetting);

            if (String.IsNullOrWhiteSpace(botToken) || String.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException(
                    "Telegram bot token and chat id must be configured in environment variables or App.config.");
            }

            string requestUri = $"https://api.telegram.org/bot{botToken}/sendMessage";
            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", text }
            }))
            using (HttpResponseMessage response = await HttpClient.PostAsync(requestUri, content, cancellationToken).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    string responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new HttpRequestException(
                        $"Telegram sendMessage failed with status {(int)response.StatusCode}: {responseText}");
                }
            }
        }

        private static string GetSetting(string environmentVariable, string appSetting)
        {
            string value = Environment.GetEnvironmentVariable(environmentVariable);

            if (!String.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return ConfigurationManager.AppSettings[appSetting];
        }
    }
}
