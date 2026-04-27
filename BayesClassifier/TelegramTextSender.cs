using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramTextSender
    {
        public static async Task SendAsync(string message, string tokenOverride, string chatIdOverride, string parseModeOverride, string threadIdOverride)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Message cannot be empty.", nameof(message));
            }

            string botToken = FirstNonEmpty(
                tokenOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"),
                Environment.GetEnvironmentVariable("BOT_TOKEN"),
                Environment.GetEnvironmentVariable("TELEGRAM_TOKEN"),
                ConfigurationManager.AppSettings["TelegramBotToken"]);
            string chatId = FirstNonEmpty(
                chatIdOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID"),
                Environment.GetEnvironmentVariable("CHAT_ID"),
                Environment.GetEnvironmentVariable("TELEGRAM_TO"),
                ConfigurationManager.AppSettings["TelegramChatId"]);
            string parseMode = FirstNonEmpty(
                parseModeOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_PARSE_MODE"),
                Environment.GetEnvironmentVariable("PARSE_MODE"),
                ConfigurationManager.AppSettings["TelegramParseMode"]);
            string threadId = FirstNonEmpty(
                threadIdOverride,
                Environment.GetEnvironmentVariable("TELEGRAM_THREAD_ID"),
                Environment.GetEnvironmentVariable("MESSAGE_THREAD_ID"),
                ConfigurationManager.AppSettings["TelegramMessageThreadId"]);

            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new InvalidOperationException("Missing Telegram bot token. Provide --telegram-token, TELEGRAM_BOT_TOKEN, BOT_TOKEN, TELEGRAM_TOKEN, or App.config key TelegramBotToken.");
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException("Missing Telegram chat id. Provide --telegram-chat-id, TELEGRAM_CHAT_ID, CHAT_ID, TELEGRAM_TO, or App.config key TelegramChatId.");
            }

            using (var client = new HttpClient())
            using (var content = new FormUrlEncodedContent(BuildPayload(chatId, message, parseMode, threadId)))
            {
                string endpoint = string.Format("https://api.telegram.org/bot{0}/sendMessage", botToken);
                HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Telegram API request failed with status {0}: {1}",
                            (int)response.StatusCode,
                            responseBody));
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> BuildPayload(string chatId, string message, string parseMode, string threadId)
        {
            var payload = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", message),
                new KeyValuePair<string, string>("disable_web_page_preview", "true")
            };

            if (!string.IsNullOrWhiteSpace(parseMode))
            {
                payload.Add(new KeyValuePair<string, string>("parse_mode", parseMode));
            }

            if (!string.IsNullOrWhiteSpace(threadId))
            {
                payload.Add(new KeyValuePair<string, string>("message_thread_id", threadId));
            }

            return payload;
        }

        private static string FirstNonEmpty(params string[] values)
        {
            if (values == null)
            {
                return null;
            }

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
