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
            string message = GetArgumentValue(args, "--message");
            string token = GetArgumentValue(args, "--telegram-token");
            if (string.IsNullOrWhiteSpace(token))
            {
                token = GetArgumentValue(args, "--bot-token");
            }

            string chatId = GetArgumentValue(args, "--telegram-chat-id");
            if (string.IsNullOrWhiteSpace(chatId))
            {
                chatId = GetArgumentValue(args, "--chat-id");
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException("Missing required argument: --message");
            }

            TelegramTextSender.SendAsync(message, token, chatId).GetAwaiter().GetResult();
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
    }
}
