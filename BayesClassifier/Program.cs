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
            if (ShouldSendTelegram(args))
            {
                SendTelegramMessage(args);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static bool ShouldSendTelegram(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return false;
            }

            foreach (string arg in args)
            {
                if (string.Equals(arg, "--send-telegram", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static void SendTelegramMessage(string[] args)
        {
            try
            {
                string message = GetArgumentValue(args, "--message");
                string token = FirstNonEmpty(
                    GetArgumentValue(args, "--telegram-token"),
                    GetArgumentValue(args, "--bot-token"));
                string chatId = FirstNonEmpty(
                    GetArgumentValue(args, "--telegram-chat-id"),
                    GetArgumentValue(args, "--chat-id"));
                string parseMode = GetArgumentValue(args, "--parse-mode");
                string threadId = FirstNonEmpty(
                    GetArgumentValue(args, "--message-thread-id"),
                    GetArgumentValue(args, "--thread-id"));

                if (string.IsNullOrWhiteSpace(message))
                {
                    throw new ArgumentException("Missing required argument: --message");
                }

                TelegramTextSender.SendAsync(message, token, chatId, parseMode, threadId).GetAwaiter().GetResult();
                Console.WriteLine("Telegram message sent successfully.");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Environment.ExitCode = 1;
            }
        }

        private static string GetArgumentValue(string[] args, string key)
        {
            if (args == null || args.Length == 0)
            {
                return null;
            }

            string keyWithEquals = key + "=";
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                if (arg == null)
                {
                    continue;
                }

                if (arg.StartsWith(keyWithEquals, StringComparison.OrdinalIgnoreCase))
                {
                    return arg.Substring(keyWithEquals.Length);
                }

                if (string.Equals(arg, key, StringComparison.OrdinalIgnoreCase))
                {
                    int nextIndex = i + 1;
                    if (nextIndex < args.Length)
                    {
                        return args[nextIndex];
                    }
                }
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
