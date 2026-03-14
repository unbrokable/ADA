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
        static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && IsTelegramCommand(args))
            {
                var message = BuildMessageFromArgs(args);
                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.Error.WriteLine("Telegram message is empty. Use: --send-telegram \"your text\"");
                    Environment.ExitCode = 1;
                    return;
                }

                try
                {
                    TelegramMessageSender.SendAsync(message).GetAwaiter().GetResult();
                    Console.WriteLine("Telegram message sent.");
                    Environment.ExitCode = 0;
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Telegram send failed: " + ex.Message);
                    Environment.ExitCode = 1;
                }

                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static bool IsTelegramCommand(string[] args)
        {
            return string.Equals(args[1], "--send-telegram", StringComparison.OrdinalIgnoreCase)
                || string.Equals(args[1], "-t", StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildMessageFromArgs(string[] args)
        {
            if (args.Length > 2)
            {
                return string.Join(" ", args, 2, args.Length - 2).Trim();
            }

            return Environment.GetEnvironmentVariable("TELEGRAM_MESSAGE");
        }
    }
}
