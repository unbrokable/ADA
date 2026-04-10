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
            if (TryGetTelegramText(args, out string textToSend))
            {
                try
                {
                    TelegramSender.SendTextMessageAsync(textToSend).GetAwaiter().GetResult();
                    return;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to send Telegram text: {ex.Message}");
                    Environment.ExitCode = 1;
                    return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static bool TryGetTelegramText(string[] args, out string text)
        {
            text = null;

            if (args == null || args.Length == 0)
            {
                return TryGetTelegramTextFromEnvironment(out text);
            }

            int flagIndex = Array.FindIndex(args, arg =>
                string.Equals(arg, "--send-telegram", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(arg, "--telegram-text", StringComparison.OrdinalIgnoreCase));

            if (flagIndex < 0 || flagIndex + 1 >= args.Length)
            {
                return flagIndex >= 0 && TryGetTelegramTextFromEnvironment(out text);
            }

            string candidate = string.Join(" ", args, flagIndex + 1, args.Length - (flagIndex + 1));
            if (string.IsNullOrWhiteSpace(candidate))
            {
                return TryGetTelegramTextFromEnvironment(out text);
            }

            text = candidate.Trim();
            return true;
        }

        private static bool TryGetTelegramTextFromEnvironment(out string text)
        {
            text = Environment.GetEnvironmentVariable("TELEGRAM_TEXT");
            if (string.IsNullOrWhiteSpace(text))
            {
                text = null;
                return false;
            }

            text = text.Trim();
            return true;
        }
    }
}
