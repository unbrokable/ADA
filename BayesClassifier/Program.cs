using System;
using System.Linq;
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
            if (args != null && args.Length > 0 && string.Equals(args[0], "send-telegram", StringComparison.OrdinalIgnoreCase))
            {
                var message = args.Length > 1 ? string.Join(" ", args.Skip(1)) : null;
                Environment.ExitCode = TelegramMessageSender.SendFromConfiguration(message);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
