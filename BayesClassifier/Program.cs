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
            if (TryHandleTelegramSend(args))
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static bool TryHandleTelegramSend(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return false;
            }

            var message = GetArgumentValue(args, "--send-telegram");
            if (message == null)
            {
                return false;
            }

            if (String.IsNullOrWhiteSpace(message))
            {
                Console.Error.WriteLine("The --send-telegram argument must include non-empty text.");
                Environment.ExitCode = 1;
                return true;
            }

            var botTokenOverride = GetArgumentValue(args, "--telegram-bot-token");
            var chatIdOverride = GetArgumentValue(args, "--telegram-chat-id");

            try
            {
                var sender = new TelegramTextSender(botTokenOverride, chatIdOverride);
                sender.SendTextAsync(message).GetAwaiter().GetResult();
                Console.WriteLine("Telegram text message sent.");
                Environment.ExitCode = 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"Failed to send Telegram text message: {exception.Message}");
                Environment.ExitCode = 1;
            }

            return true;
        }

        private static string GetArgumentValue(string[] args, string key)
        {
            var prefix = key + "=";
            for (var index = 0; index < args.Length; index++)
            {
                var argument = args[index];
                if (argument.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    if (index + 1 >= args.Length)
                    {
                        return String.Empty;
                    }

                    return args[index + 1];
                }

                if (argument.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return argument.Substring(prefix.Length);
                }
            }

            return null;
        }
    }
}
