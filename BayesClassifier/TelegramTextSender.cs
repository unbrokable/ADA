using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace BayesClassifier
{
    internal sealed class TelegramMessageOptions
    {
        public const string Usage =
            "Usage: BayesClassifier.exe --send-telegram [message] " +
            "[--telegram-token <token>] [--telegram-chat-id <chat-id>] " +
            "[--parse-mode <mode>] [--disable-web-page-preview]";

        public string BotToken { get; private set; }
        public string ChatId { get; private set; }
        public string Text { get; private set; }
        public string ParseMode { get; private set; }
        public bool DisableWebPagePreview { get; private set; }

        public static TelegramMessageOptions FromCommandLine(string[] args)
        {
            string token = null;
            string chatId = null;
            string text = null;
            string parseMode = null;
            bool disableWebPagePreview = false;
            var positional = new List<string>();

            for (int i = 1; i < args.Length; i++)
            {
                string argument = args[i];
                switch (argument)
                {
                    case "--telegram-token":
                    case "--token":
                        token = ReadValue(args, ref i, argument);
                        break;
                    case "--telegram-chat-id":
                    case "--chat-id":
                        chatId = ReadValue(args, ref i, argument);
                        break;
                    case "--message":
                    case "-m":
                        text = ReadValue(args, ref i, argument);
                        break;
                    case "--parse-mode":
                        parseMode = ReadValue(args, ref i, argument);
                        break;
                    case "--disable-web-page-preview":
                        disableWebPagePreview = true;
                        break;
                    case "--help":
                    case "-h":
                        throw new ArgumentException(Usage);
                    default:
                        if (argument.StartsWith("--", StringComparison.Ordinal))
                        {
                            throw new ArgumentException("Unknown Telegram option: " + argument);
                        }

                        positional.Add(argument);
                        break;
                }
            }

            if (String.IsNullOrWhiteSpace(text) && positional.Count > 0)
            {
                text = String.Join(" ", positional);
            }

            return Create(
                Resolve(token, "TELEGRAM_BOT_TOKEN", "TelegramBotToken", null),
                Resolve(chatId, "TELEGRAM_CHAT_ID", "TelegramChatId", null),
                Resolve(text, "TELEGRAM_TEXT", "TelegramText", "text"),
                Resolve(parseMode, "TELEGRAM_PARSE_MODE", "TelegramParseMode", null),
                disableWebPagePreview);
        }

        private static TelegramMessageOptions Create(
            string token,
            string chatId,
            string text,
            string parseMode,
            bool disableWebPagePreview)
        {
            if (String.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(
                    "Telegram bot token is required. Set TELEGRAM_BOT_TOKEN, App.config TelegramBotToken, or pass --telegram-token.");
            }

            if (String.IsNullOrWhiteSpace(chatId))
            {
                throw new ArgumentException(
                    "Telegram chat ID is required. Set TELEGRAM_CHAT_ID, App.config TelegramChatId, or pass --telegram-chat-id.");
            }

            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Telegram message text is required.");
            }

            return new TelegramMessageOptions
            {
                BotToken = token,
                ChatId = chatId,
                Text = text,
                ParseMode = parseMode,
                DisableWebPagePreview = disableWebPagePreview
            };
        }

        private static string ReadValue(string[] args, ref int index, string option)
        {
            if (index + 1 >= args.Length || String.IsNullOrWhiteSpace(args[index + 1]))
            {
                throw new ArgumentException("Missing value for " + option + ".");
            }

            index++;
            return args[index];
        }

        private static string Resolve(string explicitValue, string environmentKey, string appSettingKey, string fallback)
        {
            if (!String.IsNullOrWhiteSpace(explicitValue))
            {
                return explicitValue;
            }

            string environmentValue = Environment.GetEnvironmentVariable(environmentKey);
            if (!String.IsNullOrWhiteSpace(environmentValue))
            {
                return environmentValue;
            }

            string appSettingValue = ConfigurationManager.AppSettings[appSettingKey];
            if (!String.IsNullOrWhiteSpace(appSettingValue))
            {
                return appSettingValue;
            }

            return fallback;
        }
    }

    internal static class TelegramTextSender
    {
        private const string SendMessageEndpointFormat = "https://api.telegram.org/bot{0}/sendMessage";

        public static void Send(TelegramMessageOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "chat_id", options.ChatId },
                    { "text", options.Text }
                };

                if (!String.IsNullOrWhiteSpace(options.ParseMode))
                {
                    values.Add("parse_mode", options.ParseMode);
                }

                if (options.DisableWebPagePreview)
                {
                    values.Add("disable_web_page_preview", "true");
                }

                using (var content = new FormUrlEncodedContent(values))
                {
                    var endpoint = String.Format(SendMessageEndpointFormat, options.BotToken);
                    var response = client.PostAsync(endpoint, content).GetAwaiter().GetResult();
                    var responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new InvalidOperationException(
                            "Telegram API returned " + (int)response.StatusCode + ": " + responseBody);
                    }
                }
            }
        }
    }
}
