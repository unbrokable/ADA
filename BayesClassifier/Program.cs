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
            if (args.Length > 0)
            {
                return RunCommandLine(args);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            return 0;
        }

        private static int RunCommandLine(string[] args)
        {
            if (HasFlag(args, "--help") || HasFlag(args, "-h") || HasFlag(args, "/?"))
            {
                PrintUsage();
                return 0;
            }

            if (!HasFlag(args, "--send-telegram"))
            {
                Console.Error.WriteLine("Unknown command.");
                PrintUsage();
                return 1;
            }

            string message = GetOptionValue(args, "--text") ?? GetOptionValue(args, "--message");
            if (string.IsNullOrWhiteSpace(message))
            {
                message = "text";
            }

            string botToken = GetOptionValue(args, "--bot-token");
            string chatId = GetOptionValue(args, "--chat-id");

            try
            {
                TelegramTextSender.SendAsync(message, botToken, chatId).GetAwaiter().GetResult();
                Console.WriteLine("Telegram message sent.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to send Telegram message: " + ex.Message);
                return 1;
            }
        }

        private static bool HasFlag(string[] args, string flag)
        {
            foreach (string arg in args)
            {
                if (string.Equals(arg, flag, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetOptionValue(string[] args, string optionName)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (!string.Equals(args[i], optionName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (i + 1 >= args.Length)
                {
                    throw new ArgumentException("Missing value for " + optionName + ".");
                }

                return args[i + 1];
            }

            return null;
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  BayesClassifier.exe --send-telegram [--text \"message\"] [--bot-token TOKEN] [--chat-id CHAT_ID]");
            Console.WriteLine();
            Console.WriteLine("Telegram credentials can be supplied by arguments, TELEGRAM_BOT_TOKEN/TELEGRAM_CHAT_ID, or App.config.");
        }
    }
}
