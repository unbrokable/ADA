using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal sealed class TelegramClient
    {
        private static readonly HttpClient HttpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };

        private readonly string _botToken;
        private readonly string _chatId;

        public TelegramClient(string botToken, string chatId)
        {
            if (String.IsNullOrWhiteSpace(botToken))
            {
                throw new ArgumentException("Telegram bot token is required.", nameof(botToken));
            }

            if (String.IsNullOrWhiteSpace(chatId))
            {
                throw new ArgumentException("Telegram chat id is required.", nameof(chatId));
            }

            _botToken = botToken.Trim();
            _chatId = chatId.Trim();
        }

        public async Task SendTextAsync(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Telegram message text is required.", nameof(text));
            }

            string endpoint = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            string payload = $"chat_id={Uri.EscapeDataString(_chatId)}&text={Uri.EscapeDataString(text)}";

            using (var body = new StringContent(payload, Encoding.UTF8, "application/x-www-form-urlencoded"))
            using (HttpResponseMessage response = await HttpClient.PostAsync(endpoint, body).ConfigureAwait(false))
            {
                string responsePayload = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                bool hasSuccessFlag = responsePayload.IndexOf("\"ok\":true", StringComparison.OrdinalIgnoreCase) >= 0;

                if (!response.IsSuccessStatusCode || !hasSuccessFlag)
                {
                    throw new InvalidOperationException(
                        $"Telegram API request failed with status {(int)response.StatusCode} ({response.ReasonPhrase}).");
                }
            }
        }
    }
}
