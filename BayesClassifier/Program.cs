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
        static void Main(string[] args)
        {
            if (args.Length > 0 && IsTelegramCommand(args[0]))
            {
                SendTelegramMessage(args);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static bool IsTelegramCommand(string value)
        {
            return string.Equals(value, "--send-telegram", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "send-telegram", StringComparison.OrdinalIgnoreCase);
        }

        private static void SendTelegramMessage(string[] args)
        {
            try
            {
                string message = GetOption(args, "--message") ?? GetFirstMessageArgument(args) ?? Environment.GetEnvironmentVariable("TELEGRAM_MESSAGE") ?? "text";
                string botToken = GetOption(args, "--bot-token") ?? GetOption(args, "--telegram-bot-token");
                string chatId = GetOption(args, "--chat-id") ?? GetOption(args, "--telegram-chat-id");

                TelegramTextSender.Send(message, botToken, chatId);
                Console.WriteLine("Telegram text message sent.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Telegram text message failed: " + ex.Message);
                Environment.ExitCode = 1;
            }
        }

        private static string GetFirstMessageArgument(string[] args)
        {
            for (int i = 1; i < args.Length; i++)
            {
                if (IsOptionWithSeparateValue(args[i]))
                {
                    i++;
                    continue;
                }

                if (!args[i].StartsWith("--", StringComparison.Ordinal))
                {
                    return args[i];
                }
            }

            return null;
        }

        private static string GetOption(string[] args, string name)
        {
            for (int i = 1; i < args.Length; i++)
            {
                if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 >= args.Length || args[i + 1].StartsWith("--", StringComparison.Ordinal))
                    {
                        return null;
                    }

                    return args[i + 1];
                }

                string prefix = name + "=";
                if (args[i].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i].Substring(prefix.Length);
                }
            }

            return null;
        }

        private static bool IsOptionWithSeparateValue(string value)
        {
            return string.Equals(value, "--message", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "--bot-token", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "--telegram-bot-token", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "--chat-id", StringComparison.OrdinalIgnoreCase)
                || string.Equals(value, "--telegram-chat-id", StringComparison.OrdinalIgnoreCase);
        }
    }
}
