using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;

namespace BayesClassifier
{
    internal static class TelegramTextSender
    {
        private const string TelegramApiBaseUrl = "https://api.telegram.org";

        public static void Send(string message, string botTokenOverride = null, string chatIdOverride = null)
        {
            string botToken = FirstNonEmpty(
                botTokenOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"),
                Environment.GetEnvironmentVariable("TG_BOT_TOKEN"),
                Environment.GetEnvironmentVariable("BOT_TOKEN"),
                Environment.GetEnvironmentVariable("TELEGRAM_TOKEN"),
                ConfigurationManager.AppSettings["TelegramBotToken"]);

            string chatId = FirstNonEmpty(
                chatIdOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID"),
                Environment.GetEnvironmentVariable("TG_CHAT_ID"),
                Environment.GetEnvironmentVariable("CHAT_ID"),
                ConfigurationManager.AppSettings["TelegramChatId"]);

            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new InvalidOperationException("Telegram bot token is required. Set TELEGRAM_BOT_TOKEN or pass --bot-token.");
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException("Telegram chat id is required. Set TELEGRAM_CHAT_ID or pass --chat-id.");
            }

            if (message == null)
            {
                message = string.Empty;
            }

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            using (var httpClient = new HttpClient())
            {
                var uri = TelegramApiBaseUrl + "/bot" + botToken + "/sendMessage";
                using (var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("chat_id", chatId),
                    new KeyValuePair<string, string>("text", message)
                }))
                {
                    var response = httpClient.PostAsync(uri, content).GetAwaiter().GetResult();
                    string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException(
                            "Telegram API returned " + (int)response.StatusCode + " " + response.ReasonPhrase + ": " + responseBody);
                    }
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
