using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal sealed class TelegramTextSender
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        private readonly string _botToken;
        private readonly string _chatId;

        public TelegramTextSender(string botTokenOverride = null, string chatIdOverride = null)
        {
            _botToken = FirstNonEmpty(
                botTokenOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"),
                ConfigurationManager.AppSettings["TelegramBotToken"]);

            _chatId = FirstNonEmpty(
                chatIdOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID"),
                ConfigurationManager.AppSettings["TelegramChatId"]);

            if (String.IsNullOrWhiteSpace(_botToken))
            {
                throw new InvalidOperationException(
                    "Telegram bot token is missing. Set --telegram-bot-token, TELEGRAM_BOT_TOKEN, or App.config key TelegramBotToken.");
            }

            if (String.IsNullOrWhiteSpace(_chatId))
            {
                throw new InvalidOperationException(
                    "Telegram chat id is missing. Set --telegram-chat-id, TELEGRAM_CHAT_ID, or App.config key TelegramChatId.");
            }
        }

        public async Task SendTextAsync(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Telegram message text cannot be empty.", nameof(text));
            }

            var requestUrl = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            using (var payload = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", _chatId),
                new KeyValuePair<string, string>("text", text)
            }))
            {
                using (var response = await HttpClient.PostAsync(requestUrl, payload).ConfigureAwait(false))
                {
                    var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode || responseBody.IndexOf("\"ok\":true", StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        throw new InvalidOperationException(
                            $"Telegram API request failed with {(int)response.StatusCode} {response.ReasonPhrase}. Response: {responseBody}");
                    }
                }
            }
        }

        private static string FirstNonEmpty(params string[] values)
        {
            if (values == null)
            {
                return null;
            }

            foreach (var value in values)
            {
                if (!String.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }
    }
}
