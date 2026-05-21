using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal sealed class TelegramTextSender : IDisposable
    {
        private readonly string botToken;
        private readonly HttpClient httpClient;

        public TelegramTextSender(string botToken)
            : this(botToken, new HttpClient())
        {
        }

        internal TelegramTextSender(string botToken, HttpClient httpClient)
        {
            if (String.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("Telegram bot token is required.", nameof(botToken));
            }

            this.botToken = botToken;
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task SendAsync(string chatId, string text)
        {
            if (String.IsNullOrWhiteSpace(chatId))
            {
                throw new ArgumentException("Telegram chat ID is required.", nameof(chatId));
            }

            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Telegram message text is required.", nameof(text));
            }

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            var endpoint = new Uri("https://api.telegram.org/bot" + botToken + "/sendMessage");
            var formValues = new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", text }
            };

            using (var content = new FormUrlEncodedContent(formValues))
            using (HttpResponseMessage response = await httpClient.PostAsync(endpoint, content).ConfigureAwait(false))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        "Telegram API returned " + (int)response.StatusCode + " " + response.ReasonPhrase + ": " + responseBody);
                }
            }
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }

    internal sealed class TelegramTextMessage
    {
        private TelegramTextMessage(string botToken, string chatId, string text)
        {
            BotToken = botToken;
            ChatId = chatId;
            Text = text;
        }

        public string BotToken { get; }
        public string ChatId { get; }
        public string Text { get; }

        public static TelegramTextMessage Resolve(string botToken, string chatId, string text)
        {
            string resolvedToken = FirstNonEmpty(
                botToken,
                Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"),
                Environment.GetEnvironmentVariable("TG_BOT_TOKEN"),
                AppSetting("TelegramBotToken"));

            string resolvedChatId = FirstNonEmpty(
                chatId,
                Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID"),
                Environment.GetEnvironmentVariable("TG_CHAT_ID"),
                AppSetting("TelegramChatId"));

            string resolvedText = FirstNonEmpty(
                text,
                Environment.GetEnvironmentVariable("TELEGRAM_TEXT"),
                Environment.GetEnvironmentVariable("TG_TEXT"),
                AppSetting("TelegramText"),
                "text");

            if (String.IsNullOrWhiteSpace(resolvedToken))
            {
                throw new InvalidOperationException(
                    "Telegram bot token is required. Set --telegram-token, TELEGRAM_BOT_TOKEN, TG_BOT_TOKEN, or App.config TelegramBotToken.");
            }

            if (String.IsNullOrWhiteSpace(resolvedChatId))
            {
                throw new InvalidOperationException(
                    "Telegram chat ID is required. Set --telegram-chat-id, TELEGRAM_CHAT_ID, TG_CHAT_ID, or App.config TelegramChatId.");
            }

            return new TelegramTextMessage(resolvedToken, resolvedChatId, resolvedText);
        }

        private static string AppSetting(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (string value in values)
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
