using System;
using System.Windows.Forms;

namespace BayesClassifier
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            if (HasOption(args, "--send-telegram"))
            {
                return SendTelegramMessage(args);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            return 0;
        }

        private static int SendTelegramMessage(string[] args)
        {
            try
            {
                string message = FirstNonEmpty(
                    GetOptionValue(args, "--message"),
                    GetOptionValue(args, "--text"),
                    GetFirstPositionalValue(args),
                    Environment.GetEnvironmentVariable("TELEGRAM_MESSAGE"),
                    Environment.GetEnvironmentVariable("TELEGRAM_TEXT"),
                    Environment.GetEnvironmentVariable("MESSAGE_TEXT"),
                    Environment.GetEnvironmentVariable("MESSAGE"));

                string token = FirstNonEmpty(
                    GetOptionValue(args, "--telegram-token"),
                    GetOptionValue(args, "--bot-token"));

                string chatId = FirstNonEmpty(
                    GetOptionValue(args, "--telegram-chat-id"),
                    GetOptionValue(args, "--chat-id"));

                TelegramTextSender.SendAsync(message, token, chatId).GetAwaiter().GetResult();
                Console.WriteLine("Telegram text message sent.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }

        private static bool HasOption(string[] args, string option)
        {
            if (args == null)
            {
                return false;
            }

            foreach (string arg in args)
            {
                if (string.Equals(arg, option, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetOptionValue(string[] args, string option)
        {
            if (args == null)
            {
                return null;
            }

            string optionWithEquals = option + "=";
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg == null)
                {
                    continue;
                }

                if (arg.StartsWith(optionWithEquals, StringComparison.OrdinalIgnoreCase))
                {
                    return arg.Substring(optionWithEquals.Length);
                }

                if (string.Equals(arg, option, StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
                {
                    return args[i + 1];
                }
            }

            return null;
        }

        private static string GetFirstPositionalValue(string[] args)
        {
            if (args == null)
            {
                return null;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (string.IsNullOrWhiteSpace(arg) || string.Equals(arg, "--send-telegram", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (arg.StartsWith("--", StringComparison.Ordinal))
                {
                    if (!arg.Contains("=") && i + 1 < args.Length)
                    {
                        i++;
                    }

                    continue;
                }

                return arg;
            }

            return null;
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
