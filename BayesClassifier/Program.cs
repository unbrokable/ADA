using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BayesClassifier
{
    static class Program
    {
        private const string SendTelegramFlag = "--send-telegram";
        private const string TelegramTokenOption = "--telegram-token";
        private const string TelegramChatIdOption = "--telegram-chat-id";
        private const string TelegramTextOption = "--telegram-text";
        private const string TelegramParseModeOption = "--telegram-parse-mode";

        private const string TelegramTokenEnvironmentKey = "TELEGRAM_BOT_TOKEN";
        private const string TelegramChatIdEnvironmentKey = "TELEGRAM_CHAT_ID";
        private const string TelegramTextEnvironmentKey = "TELEGRAM_TEXT";
        private const string TelegramParseModeEnvironmentKey = "TELEGRAM_PARSE_MODE";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            TelegramMessageRequest telegramMessageRequest;
            string parseError;

            if (!TryBuildTelegramMessageRequest(args, out telegramMessageRequest, out parseError))
            {
                Trace.WriteLine(parseError);
                Environment.ExitCode = 1;
                return;
            }

            if (telegramMessageRequest != null)
            {
                bool isSent = TelegramMessageSender.SendTextMessageAsync(telegramMessageRequest).GetAwaiter().GetResult();
                Environment.ExitCode = isSent ? 0 : 1;
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static bool TryBuildTelegramMessageRequest(
            string[] args,
            out TelegramMessageRequest request,
            out string errorMessage)
        {
            request = null;
            errorMessage = null;

            string tokenFromArgs = null;
            string chatIdFromArgs = null;
            string textFromArgs = null;
            string parseModeFromArgs = null;
            bool sendTelegramRequested = false;
            bool sawTelegramSpecificOption = false;

            for (int i = 0; i < args.Length; i++)
            {
                string argument = args[i];

                switch (argument)
                {
                    case SendTelegramFlag:
                        sendTelegramRequested = true;
                        sawTelegramSpecificOption = true;

                        if (i + 1 < args.Length && !IsOption(args[i + 1]))
                        {
                            textFromArgs = args[i + 1];
                            i++;
                        }
                        break;

                    case TelegramTokenOption:
                        sawTelegramSpecificOption = true;
                        if (!TryReadOptionValue(args, ref i, TelegramTokenOption, out tokenFromArgs, out errorMessage))
                        {
                            return false;
                        }
                        break;

                    case TelegramChatIdOption:
                        sawTelegramSpecificOption = true;
                        if (!TryReadOptionValue(args, ref i, TelegramChatIdOption, out chatIdFromArgs, out errorMessage))
                        {
                            return false;
                        }
                        break;

                    case TelegramTextOption:
                        sawTelegramSpecificOption = true;
                        if (!TryReadOptionValue(args, ref i, TelegramTextOption, out textFromArgs, out errorMessage))
                        {
                            return false;
                        }
                        break;

                    case TelegramParseModeOption:
                        sawTelegramSpecificOption = true;
                        if (!TryReadOptionValue(args, ref i, TelegramParseModeOption, out parseModeFromArgs, out errorMessage))
                        {
                            return false;
                        }
                        break;

                    default:
                        if (argument.StartsWith("--telegram-", StringComparison.Ordinal))
                        {
                            errorMessage = "Unknown Telegram option: " + argument;
                            return false;
                        }
                        break;
                }
            }

            string tokenFromEnvironment = Environment.GetEnvironmentVariable(TelegramTokenEnvironmentKey);
            string chatIdFromEnvironment = Environment.GetEnvironmentVariable(TelegramChatIdEnvironmentKey);
            string textFromEnvironment = Environment.GetEnvironmentVariable(TelegramTextEnvironmentKey);
            string parseModeFromEnvironment = Environment.GetEnvironmentVariable(TelegramParseModeEnvironmentKey);

            bool hasFullTelegramEnvironmentConfiguration =
                !string.IsNullOrWhiteSpace(tokenFromEnvironment) &&
                !string.IsNullOrWhiteSpace(chatIdFromEnvironment) &&
                !string.IsNullOrWhiteSpace(textFromEnvironment);

            bool shouldSendTelegramMessage =
                sendTelegramRequested ||
                sawTelegramSpecificOption ||
                hasFullTelegramEnvironmentConfiguration;

            if (!shouldSendTelegramMessage)
            {
                return true;
            }

            string token = FirstNonEmpty(tokenFromArgs, tokenFromEnvironment);
            string chatId = FirstNonEmpty(chatIdFromArgs, chatIdFromEnvironment);
            string text = FirstNonEmpty(textFromArgs, textFromEnvironment);
            string parseMode = FirstNonEmpty(parseModeFromArgs, parseModeFromEnvironment);

            if (string.IsNullOrWhiteSpace(token))
            {
                errorMessage = "Telegram bot token is required. Set --telegram-token or TELEGRAM_BOT_TOKEN.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                errorMessage = "Telegram chat id is required. Set --telegram-chat-id or TELEGRAM_CHAT_ID.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                errorMessage = "Telegram text is required. Set --telegram-text or TELEGRAM_TEXT.";
                return false;
            }

            request = new TelegramMessageRequest(
                token.Trim(),
                chatId.Trim(),
                text,
                string.IsNullOrWhiteSpace(parseMode) ? null : parseMode.Trim());

            return true;
        }

        private static bool TryReadOptionValue(
            string[] args,
            ref int index,
            string optionName,
            out string optionValue,
            out string errorMessage)
        {
            optionValue = null;
            errorMessage = null;

            int valueIndex = index + 1;
            if (valueIndex >= args.Length || IsOption(args[valueIndex]))
            {
                errorMessage = "Missing value for option: " + optionName;
                return false;
            }

            optionValue = args[valueIndex];
            index = valueIndex;
            return true;
        }

        private static bool IsOption(string value)
        {
            return !string.IsNullOrEmpty(value) && value.StartsWith("--", StringComparison.Ordinal);
        }

        private static string FirstNonEmpty(string preferredValue, string fallbackValue)
        {
            return !string.IsNullOrWhiteSpace(preferredValue) ? preferredValue : fallbackValue;
        }
    }

    internal sealed class TelegramMessageRequest
    {
        public TelegramMessageRequest(string botToken, string chatId, string text, string parseMode)
        {
            BotToken = botToken;
            ChatId = chatId;
            Text = text;
            ParseMode = parseMode;
        }

        public string BotToken { get; private set; }

        public string ChatId { get; private set; }

        public string Text { get; private set; }

        public string ParseMode { get; private set; }
    }

    internal static class TelegramMessageSender
    {
        private static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public static async Task<bool> SendTextMessageAsync(TelegramMessageRequest request)
        {
            string endpoint = "https://api.telegram.org/bot" + request.BotToken + "/sendMessage";

            List<KeyValuePair<string, string>> formValues = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("chat_id", request.ChatId),
                new KeyValuePair<string, string>("text", request.Text)
            };

            if (!string.IsNullOrWhiteSpace(request.ParseMode))
            {
                formValues.Add(new KeyValuePair<string, string>("parse_mode", request.ParseMode));
            }

            using (FormUrlEncodedContent content = new FormUrlEncodedContent(formValues))
            {
                try
                {
                    using (HttpResponseMessage response = await HttpClient.PostAsync(endpoint, content).ConfigureAwait(false))
                    {
                        string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        if (!response.IsSuccessStatusCode)
                        {
                            Trace.WriteLine("Telegram send failed with HTTP " + (int)response.StatusCode + ". Response: " + responseBody);
                            return false;
                        }

                        if (responseBody.IndexOf("\"ok\":true", StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            Trace.WriteLine("Telegram send returned unsuccessful payload. Response: " + responseBody);
                            return false;
                        }

                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Telegram send failed with exception: " + ex);
                    return false;
                }
            }
        }
    }
}
