using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace BayesClassifier
{
    internal sealed class TelegramSender
    {
        private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(15);
        private readonly string botToken;
        private readonly string chatId;
        private readonly HttpClient httpClient;

        private TelegramSender(string botToken, string chatId, HttpClient httpClient)
        {
            this.botToken = botToken;
            this.chatId = chatId;
            this.httpClient = httpClient;
        }

        public static TelegramSender CreateFromEnvironmentOrConfig()
        {
            string token = ReadSetting("TELEGRAM_BOT_TOKEN", "TelegramBotToken");
            string targetChatId = ReadSetting("TELEGRAM_CHAT_ID", "TelegramChatId");
            if (String.IsNullOrWhiteSpace(token) || String.IsNullOrWhiteSpace(targetChatId))
            {
                return null;
            }

            var client = new HttpClient
            {
                Timeout = RequestTimeout
            };

            return new TelegramSender(token.Trim(), targetChatId.Trim(), client);
        }

        public async Task<TelegramSendResult> SendTextAsync(string text)
        {
            try
            {
                string endpoint = $"https://api.telegram.org/bot{botToken}/sendMessage";
                using (var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("chat_id", chatId),
                    new KeyValuePair<string, string>("text", text ?? String.Empty)
                }))
                {
                    HttpResponseMessage response = await httpClient.PostAsync(endpoint, content).ConfigureAwait(false);
                    if (response.IsSuccessStatusCode)
                    {
                        return TelegramSendResult.Ok();
                    }

                    string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return TelegramSendResult.Fail($"send failed ({(int)response.StatusCode}): {TrimForUi(responseBody)}");
                }
            }
            catch (Exception ex)
            {
                return TelegramSendResult.Fail($"send failed: {ex.Message}");
            }
        }

        private static string ReadSetting(string envName, string configName)
        {
            string envValue = Environment.GetEnvironmentVariable(envName);
            if (!String.IsNullOrWhiteSpace(envValue))
            {
                return envValue;
            }

            return ConfigurationManager.AppSettings[configName];
        }

        private static string TrimForUi(string value)
        {
            const int maxLength = 120;
            if (String.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(0, maxLength) + "...";
        }
    }

    internal sealed class TelegramSendResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }

        private TelegramSendResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public static TelegramSendResult Ok()
        {
            return new TelegramSendResult(true, String.Empty);
        }

        public static TelegramSendResult Fail(string errorMessage)
        {
            return new TelegramSendResult(false, errorMessage);
        }
    }
}
