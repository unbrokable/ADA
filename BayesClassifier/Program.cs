using System;
using System.Windows.Forms;

namespace BayesClassifier
{
    static class Program
    {
        private const string SendTelegramSwitch = "--send-telegram";
        private const string TelegramTokenSwitch = "--telegram-token";
        private const string TelegramChatIdSwitch = "--telegram-chat-id";
        private const string TelegramMessageSwitch = "--telegram-message";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (HasSwitch(args, SendTelegramSwitch))
            {
                RunTelegramMode(args);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void RunTelegramMode(string[] args)
        {
            try
            {
                string botToken = ResolveSetting(args, TelegramTokenSwitch, "TELEGRAM_BOT_TOKEN");
                string chatId = ResolveSetting(args, TelegramChatIdSwitch, "TELEGRAM_CHAT_ID");
                string message = ResolveSetting(args, TelegramMessageSwitch, "TELEGRAM_MESSAGE");

                TelegramSender.SendTextMessageAsync(botToken, chatId, message).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Unable to send Telegram text message: " + ex.Message);
                Environment.ExitCode = 1;
            }
        }

        private static string ResolveSetting(string[] args, string argumentName, string environmentVariableName)
        {
            string value;
            if (TryGetArgumentValue(args, argumentName, out value))
            {
                return value;
            }

            return Environment.GetEnvironmentVariable(environmentVariableName);
        }

        private static bool HasSwitch(string[] args, string argumentName)
        {
            string prefix = argumentName + "=";
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], argumentName, StringComparison.OrdinalIgnoreCase) ||
                    args[i].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryGetArgumentValue(string[] args, string argumentName, out string value)
        {
            string prefix = argumentName + "=";
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    if (i + 1 < args.Length)
                    {
                        value = args[i + 1];
                        return true;
                    }

                    break;
                }

                if (args[i].StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    value = args[i].Substring(prefix.Length);
                    return true;
                }
            }

            value = null;
            return false;
        }
    }
}
