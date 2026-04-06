using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal static class TelegramMessageSender
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        public static async Task SendTextMessageAsync(
            string botToken,
            string chatId,
            string text,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("Bot token is required.", nameof(botToken));
            }

            if (string.IsNullOrWhiteSpace(chatId))
            {
                throw new ArgumentException("Chat ID is required.", nameof(chatId));
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Message text is required.", nameof(text));
            }

            using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "chat_id", chatId },
                { "text", text }
            }))
            {
                string endpoint = $"https://api.telegram.org/bot{botToken}/sendMessage";
                HttpResponseMessage response = await HttpClient.PostAsync(endpoint, content, cancellationToken).ConfigureAwait(false);
                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException(
                        $"Telegram API error ({(int)response.StatusCode} {response.ReasonPhrase}): {responseBody}");
                }
            }
        }
    }
}
