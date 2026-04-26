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
            if (TrySendTelegramMessage(args))
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static bool TrySendTelegramMessage(string[] args)
        {
            if (args == null || args.Length < 2 || !String.Equals(args[0], "--send-telegram", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            TelegramMessageSender.SendTextAsync(String.Join(" ", args, 1, args.Length - 1)).GetAwaiter().GetResult();
            return true;
        }
    }
}
