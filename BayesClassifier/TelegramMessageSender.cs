using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramMessageSender
    {
        private const string BotTokenEnvironmentVariable = "TELEGRAM_BOT_TOKEN";
        private const string ChatIdEnvironmentVariable = "TELEGRAM_CHAT_ID";
        private const string MessageEnvironmentVariable = "TELEGRAM_MESSAGE";
        private const string BotTokenConfigKey = "TelegramBotToken";
        private const string ChatIdConfigKey = "TelegramChatId";
        private const string MessageConfigKey = "TelegramMessage";

        public static int SendFromConfiguration(string argumentMessage)
        {
            var botToken = ReadSetting(BotTokenEnvironmentVariable, BotTokenConfigKey);
            var chatId = ReadSetting(ChatIdEnvironmentVariable, ChatIdConfigKey);
            var message = ResolveMessage(argumentMessage);

            if (string.IsNullOrWhiteSpace(botToken))
            {
                Console.Error.WriteLine("Missing Telegram bot token. Set TELEGRAM_BOT_TOKEN or AppSettings:TelegramBotToken.");
                return 1;
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                Console.Error.WriteLine("Missing Telegram chat id. Set TELEGRAM_CHAT_ID or AppSettings:TelegramChatId.");
                return 1;
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                Console.Error.WriteLine("Missing message text. Pass as CLI args or set TELEGRAM_MESSAGE/AppSettings:TelegramMessage.");
                return 1;
            }

            try
            {
                SendMessageAsync(botToken, chatId, message).GetAwaiter().GetResult();
                Console.WriteLine("Telegram message sent successfully.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to send Telegram message: " + ex.Message);
                return 1;
            }
        }

        private static async Task SendMessageAsync(string botToken, string chatId, string message)
        {
            var endpoint = $"https://api.telegram.org/bot{botToken}/sendMessage";
            using (var client = new HttpClient())
            using (var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", chatId),
                new KeyValuePair<string, string>("text", message)
            }))
            {
                var response = await client.PostAsync(endpoint, content).ConfigureAwait(false);
                var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Telegram API returned {(int)response.StatusCode}: {body}");
                }
            }
        }

        private static string ReadSetting(string environmentKey, string appSettingKey)
        {
            var fromEnvironment = Environment.GetEnvironmentVariable(environmentKey);
            if (!string.IsNullOrWhiteSpace(fromEnvironment))
            {
                return fromEnvironment;
            }

            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            return appSettings[appSettingKey];
        }

        private static string ResolveMessage(string argumentMessage)
        {
            if (!string.IsNullOrWhiteSpace(argumentMessage))
            {
                return argumentMessage;
            }

            return ReadSetting(MessageEnvironmentVariable, MessageConfigKey);
        }
    }
}
