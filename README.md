# BayesClassifier

BayesClassifier is a .NET Framework WinForms sample that classifies text as spam
or not spam.

## Send text in Telegram

The repository includes two ways to send a Telegram text message:

- `send_telegram_text.sh`, which sends from Linux or automation environments with
  `curl`.
- `BayesClassifier.exe --send-telegram`, which sends through the application CLI
  when the .NET Framework project is built.

Both commands use the Telegram Bot API `sendMessage` endpoint. Credentials are
resolved in this order:

1. Command-line arguments
2. Environment variables
3. `BayesClassifier/App.config` appSettings

Supported environment variables:

- `TELEGRAM_BOT_TOKEN` or `TG_BOT_TOKEN`
- `TELEGRAM_CHAT_ID` or `TG_CHAT_ID`
- `TELEGRAM_TEXT` or `TG_TEXT`

Example with the helper script:

```bash
TELEGRAM_BOT_TOKEN="123456:bot-token" \
TELEGRAM_CHAT_ID="123456789" \
./send_telegram_text.sh "text"
```

Example with the built application:

```powershell
BayesClassifier.exe --send-telegram --telegram-token "123456:bot-token" --telegram-chat-id "123456789" --text "text"
```

If no text is supplied, the commands use `TELEGRAM_TEXT`, `TG_TEXT`,
`TelegramText` from `App.config`, or the default message `text`.
