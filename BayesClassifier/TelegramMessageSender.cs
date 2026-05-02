using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramMessageSender
    {
        private static readonly HttpClient Client = new HttpClient();

        public static bool IsConfigured
        {
            get
            {
                return !String.IsNullOrWhiteSpace(BotToken)
                    && !String.IsNullOrWhiteSpace(ChatId);
            }
        }

        private static string BotToken
            => ConfigurationManager.AppSettings["TelegramBotToken"];

        private static string ChatId
            => ConfigurationManager.AppSettings["TelegramChatId"];

        public static async Task SendTextAsync(string text)
        {
            if (!IsConfigured)
            {
                throw new InvalidOperationException("Telegram bot token and chat id must be configured.");
            }

            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "chat_id", ChatId },
                { "text", text }
            }))
            using (HttpResponseMessage response = await Client.PostAsync(GetSendMessageUrl(), content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
            }
        }

        private static string GetSendMessageUrl()
            => $"https://api.telegram.org/bot{BotToken}/sendMessage";
    }
}
