using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramTextSender
    {
        private const string SendCommand = "--send-telegram";
        private static readonly HttpClient HttpClient = new HttpClient();

        public static bool IsTelegramSendRequest(string[] args)
        {
            return args != null
                && args.Length > 0
                && string.Equals(args[0], SendCommand, StringComparison.OrdinalIgnoreCase);
        }

        public static int Run(string[] args)
        {
            try
            {
                TelegramSendOptions options = TelegramSendOptions.Parse(args);

                if (options.DryRun)
                {
                    Console.WriteLine("Dry run: would send Telegram message to chat '{0}'.", Mask(options.ChatId));
                    Console.WriteLine(options.Message);
                    return 0;
                }

                SendMessageAsync(options).GetAwaiter().GetResult();
                Console.WriteLine("Telegram message sent.");
                return 0;
            }
            catch (TelegramUsageException ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine();
                Console.Error.WriteLine(GetUsage());
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Telegram send failed: {0}", ex.Message);
                return 1;
            }
        }

        private static async Task SendMessageAsync(TelegramSendOptions options)
        {
            string endpoint = string.Format(
                "https://api.telegram.org/bot{0}/sendMessage",
                options.BotToken);

            using (FormUrlEncodedContent content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("chat_id", options.ChatId),
                new KeyValuePair<string, string>("text", options.Message)
            }))
            {
                HttpResponseMessage response = await HttpClient.PostAsync(endpoint, content).ConfigureAwait(false);
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(string.Format(
                        "Telegram API returned HTTP {0}: {1}",
                        (int)response.StatusCode,
                        responseBody));
                }

                if (responseBody.IndexOf("\"ok\":true", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    throw new InvalidOperationException("Telegram API did not confirm success: " + responseBody);
                }
            }
        }

        private static string GetUsage()
        {
            return "Usage: BayesClassifier.exe --send-telegram [--token TOKEN] [--chat-id CHAT_ID] [--message MESSAGE] [--dry-run]\n"
                + "Credentials can also be provided by TELEGRAM_BOT_TOKEN / TELEGRAM_CHAT_ID or App.config keys TelegramBotToken / TelegramChatId.";
        }

        private static string Mask(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "<missing>";
            }

            if (value.Length <= 4)
            {
                return "****";
            }

            return new string('*', value.Length - 4) + value.Substring(value.Length - 4);
        }

        private sealed class TelegramSendOptions
        {
            public string BotToken { get; private set; }
            public string ChatId { get; private set; }
            public string Message { get; private set; }
            public bool DryRun { get; private set; }

            public static TelegramSendOptions Parse(string[] args)
            {
                string token = null;
                string chatId = null;
                string message = null;
                bool dryRun = false;
                List<string> messageParts = new List<string>();

                for (int i = 1; i < args.Length; i++)
                {
                    string arg = args[i];

                    if (IsOption(arg, "--help") || IsOption(arg, "-h"))
                    {
                        throw new TelegramUsageException("Telegram text sending command.");
                    }

                    if (IsOption(arg, "--token") || IsOption(arg, "--bot-token") || IsOption(arg, "--telegram-token"))
                    {
                        token = ReadValue(args, ref i, arg);
                        continue;
                    }

                    if (IsOption(arg, "--chat-id") || IsOption(arg, "--telegram-chat-id"))
                    {
                        chatId = ReadValue(args, ref i, arg);
                        continue;
                    }

                    if (IsOption(arg, "--message") || IsOption(arg, "--text"))
                    {
                        message = ReadValue(args, ref i, arg);
                        continue;
                    }

                    if (IsOption(arg, "--dry-run"))
                    {
                        dryRun = true;
                        continue;
                    }

                    if (arg.StartsWith("--", StringComparison.Ordinal))
                    {
                        throw new TelegramUsageException("Unknown option: " + arg);
                    }

                    messageParts.Add(arg);
                }

                if (string.IsNullOrWhiteSpace(message) && messageParts.Count > 0)
                {
                    message = string.Join(" ", messageParts.ToArray());
                }

                message = FirstNonEmpty(message, Environment.GetEnvironmentVariable("TELEGRAM_MESSAGE"), "text");

                if (!dryRun)
                {
                    token = FirstNonEmpty(
                        token,
                        Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN"),
                        ConfigurationManager.AppSettings["TelegramBotToken"]);

                    chatId = FirstNonEmpty(
                        chatId,
                        Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID"),
                        ConfigurationManager.AppSettings["TelegramChatId"]);

                    if (string.IsNullOrWhiteSpace(token))
                    {
                        throw new TelegramUsageException("Missing Telegram bot token.");
                    }

                    if (string.IsNullOrWhiteSpace(chatId))
                    {
                        throw new TelegramUsageException("Missing Telegram chat ID.");
                    }
                }
                else
                {
                    chatId = FirstNonEmpty(
                        chatId,
                        Environment.GetEnvironmentVariable("TELEGRAM_CHAT_ID"),
                        ConfigurationManager.AppSettings["TelegramChatId"]);
                }

                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new TelegramUsageException("Missing Telegram message text.");
                }

                return new TelegramSendOptions
                {
                    BotToken = token,
                    ChatId = chatId,
                    Message = message,
                    DryRun = dryRun
                };
            }

            private static bool IsOption(string actual, string expected)
            {
                return string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase);
            }

            private static string ReadValue(string[] args, ref int index, string optionName)
            {
                if (index + 1 >= args.Length || args[index + 1].StartsWith("--", StringComparison.Ordinal))
                {
                    throw new TelegramUsageException("Missing value for " + optionName + ".");
                }

                index++;
                return args[index];
            }

            private static string FirstNonEmpty(params string[] values)
            {
                foreach (string value in values)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return value.Trim();
                    }
                }

                return null;
            }
        }

        private sealed class TelegramUsageException : Exception
        {
            public TelegramUsageException(string message)
                : base(message)
            {
            }
        }
    }
}
