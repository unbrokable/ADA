using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BayesClassifier
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (IsTelegramCommand(args))
            {
                Environment.ExitCode = RunTelegramCommand(args);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static bool IsTelegramCommand(string[] args)
        {
            if (args == null)
            {
                return false;
            }

            foreach (string arg in args)
            {
                if (IsOption(arg, "--send-telegram"))
                {
                    return true;
                }
            }

            return false;
        }

        private static int RunTelegramCommand(string[] args)
        {
            try
            {
                TelegramCommandOptions options = TelegramCommandOptions.Parse(args);

                if (options.ShowHelp)
                {
                    Console.WriteLine(TelegramCommandOptions.Usage);
                    return 0;
                }

                TelegramTextMessage message = TelegramTextMessage.Resolve(
                    options.BotToken,
                    options.ChatId,
                    options.Message);

                using (var sender = new TelegramTextSender(message.BotToken))
                {
                    sender.SendAsync(message.ChatId, message.Text).GetAwaiter().GetResult();
                }

                Console.WriteLine("Telegram text sent.");
                return 0;
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine();
                Console.Error.WriteLine(TelegramCommandOptions.Usage);
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to send Telegram text: " + ex.Message);
                return 1;
            }
        }

        private static bool IsOption(string value, string option)
        {
            return String.Equals(value, option, StringComparison.OrdinalIgnoreCase);
        }

        private sealed class TelegramCommandOptions
        {
            public const string Usage =
                "Usage: BayesClassifier.exe --send-telegram [--text MESSAGE] [--telegram-token TOKEN] [--telegram-chat-id CHAT_ID]\n" +
                "\n" +
                "Credentials are resolved in this order: CLI arguments, environment variables, then App.config appSettings.\n" +
                "Supported environment variables: TELEGRAM_BOT_TOKEN, TG_BOT_TOKEN, TELEGRAM_CHAT_ID, TG_CHAT_ID, TELEGRAM_TEXT, TG_TEXT.\n" +
                "Supported App.config keys: TelegramBotToken, TelegramChatId, TelegramText.";

            public string BotToken { get; private set; }
            public string ChatId { get; private set; }
            public string Message { get; private set; }
            public bool ShowHelp { get; private set; }

            public static TelegramCommandOptions Parse(string[] args)
            {
                var options = new TelegramCommandOptions();
                var messageParts = new List<string>();

                for (int i = 0; i < args.Length; i++)
                {
                    string arg = args[i];

                    if (IsOption(arg, "--send-telegram"))
                    {
                        continue;
                    }

                    if (IsOption(arg, "--help") || IsOption(arg, "-h"))
                    {
                        options.ShowHelp = true;
                        continue;
                    }

                    if (IsOption(arg, "--telegram-token") || IsOption(arg, "--bot-token"))
                    {
                        options.BotToken = ReadValue(args, ref i, arg);
                        continue;
                    }

                    if (IsOption(arg, "--telegram-chat-id") || IsOption(arg, "--chat-id"))
                    {
                        options.ChatId = ReadValue(args, ref i, arg);
                        continue;
                    }

                    if (IsOption(arg, "--text") || IsOption(arg, "--message") || IsOption(arg, "-m"))
                    {
                        options.Message = ReadValue(args, ref i, arg);
                        continue;
                    }

                    if (arg.StartsWith("--", StringComparison.Ordinal))
                    {
                        throw new ArgumentException("Unknown Telegram option: " + arg);
                    }

                    messageParts.Add(arg);
                }

                if (String.IsNullOrWhiteSpace(options.Message) && messageParts.Count > 0)
                {
                    options.Message = String.Join(" ", messageParts);
                }

                return options;
            }

            private static string ReadValue(string[] args, ref int index, string option)
            {
                if (index + 1 >= args.Length)
                {
                    throw new ArgumentException("Missing value for " + option + ".");
                }

                index++;
                return args[index];
            }
        }
    }
}
