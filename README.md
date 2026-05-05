# BayesClassifier

## Sending a Telegram text message

The application can send a Telegram Bot API text message without launching the WinForms UI:

```bash
BayesClassifier.exe --send-telegram --message "text"
```

Credentials are read in this order:

1. CLI arguments: `--bot-token` / `--telegram-bot-token` and `--chat-id` / `--telegram-chat-id`
2. Environment variables: `TELEGRAM_BOT_TOKEN` / `TG_BOT_TOKEN` / `BOT_TOKEN` / `TELEGRAM_TOKEN` and `TELEGRAM_CHAT_ID` / `TG_CHAT_ID` / `CHAT_ID`
3. `App.config` keys: `TelegramBotToken` and `TelegramChatId`

Optional command arguments:

- `--message "hello"` or a positional first message argument
- `--bot-token` / `--telegram-bot-token`
- `--chat-id` / `--telegram-chat-id`

For automation on Linux/macOS, use the helper script:

```bash
TELEGRAM_BOT_TOKEN="123:abc" TELEGRAM_CHAT_ID="123456" ./send_telegram_text.sh "text"
```

Run `./send_telegram_text.sh --dry-run "text"` to validate configuration without calling Telegram.
