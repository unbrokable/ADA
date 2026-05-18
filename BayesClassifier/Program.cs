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
            if (args != null && args.Length > 0 && IsTelegramCommand(args[0]))
            {
                return SendTelegramText(args);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            return 0;
        }

        private static bool IsTelegramCommand(string argument)
        {
            return String.Equals(argument, "--send-telegram", StringComparison.OrdinalIgnoreCase)
                || String.Equals(argument, "send-telegram", StringComparison.OrdinalIgnoreCase);
        }

        private static int SendTelegramText(string[] args)
        {
            try
            {
                TelegramTextSender.Send(TelegramMessageOptions.FromCommandLine(args));
                return 0;
            }
            catch (ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine();
                Console.Error.WriteLine(TelegramMessageOptions.Usage);
                return 2;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Failed to send Telegram message: " + ex.Message);
                return 1;
            }
        }
    }
}
