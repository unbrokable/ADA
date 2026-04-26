using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BayesClassifier
{
    public static class TelegramMessageSender
    {
        private const string BotTokenEnvironmentVariable = "TELEGRAM_BOT_TOKEN";
        private const string ChatIdEnvironmentVariable = "TELEGRAM_CHAT_ID";

        public static Task SendTextAsync(string text)
        {
            return SendTextAsync(text, CancellationToken.None);
        }

        public static Task SendTextAsync(string text, CancellationToken cancellationToken)
        {
            string botToken = Environment.GetEnvironmentVariable(BotTokenEnvironmentVariable);
            string chatId = Environment.GetEnvironmentVariable(ChatIdEnvironmentVariable);

            return SendTextAsync(botToken, chatId, text, cancellationToken);
        }

        public static async Task SendTextAsync(string botToken, string chatId, string text, CancellationToken cancellationToken)
        {
            if (String.IsNullOrWhiteSpace(botToken))
            {
                throw new InvalidOperationException($"Set {BotTokenEnvironmentVariable} before sending Telegram messages.");
            }

            if (String.IsNullOrWhiteSpace(chatId))
            {
                throw new InvalidOperationException($"Set {ChatIdEnvironmentVariable} before sending Telegram messages.");
            }

            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Telegram message text cannot be empty.", nameof(text));
            }

            using (HttpClient client = new HttpClient())
            using (FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["chat_id"] = chatId,
                ["text"] = text
            }))
            {
                string url = $"https://api.telegram.org/bot{botToken}/sendMessage";
                HttpResponseMessage response = await client.PostAsync(url, content, cancellationToken).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
