using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramTextSender
    {
        private const string BotTokenEnvironmentVariable = "TELEGRAM_BOT_TOKEN";
        private const string AlternateBotTokenEnvironmentVariable = "BOT_TOKEN";
        private const string ChatIdEnvironmentVariable = "TELEGRAM_CHAT_ID";
        private const string AlternateChatIdEnvironmentVariable = "CHAT_ID";
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendAsync(string message, string botTokenOverride, string chatIdOverride)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Telegram message text is required.", "message");
            }

            string botToken = FirstNonEmpty(
                botTokenOverride,
                Environment.GetEnvironmentVariable(BotTokenEnvironmentVariable),
                Environment.GetEnvironmentVariable(AlternateBotTokenEnvironmentVariable),
                ConfigurationManager.AppSettings["TelegramBotToken"]);

            string chatId = FirstNonEmpty(
                chatIdOverride,
                Environment.GetEnvironmentVariable(ChatIdEnvironmentVariable),
                Environment.GetEnvironmentVariable(AlternateChatIdEnvironmentVariable),
                ConfigurationManager.AppSettings["TelegramChatId"]);

            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new InvalidOperationException("Telegram bot token is missing. Set --bot-token, TELEGRAM_BOT_TOKEN, BOT_TOKEN, or TelegramBotToken in App.config.");
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException("Telegram chat ID is missing. Set --chat-id, TELEGRAM_CHAT_ID, CHAT_ID, or TelegramChatId in App.config.");
            }

            string requestUri = "https://api.telegram.org/bot" + botToken + "/sendMessage";
            using (FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", message)
            }))
            using (HttpResponseMessage response = await HttpClient.PostAsync(requestUri, content).ConfigureAwait(false))
            {
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException("Telegram API returned " + (int)response.StatusCode + ": " + responseBody);
                }
            }
        }

        private static string FirstNonEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value;
                }
            }

            return null;
        }
    }
}
