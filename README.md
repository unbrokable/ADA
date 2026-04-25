# BayesClassifier

WinForms Bayesian classifier sample with an optional Telegram text-sending entry point.

## Send text in Telegram

The application can send a Telegram Bot API text message without opening the UI:

```bash
BayesClassifier.exe --send-telegram --message "Hello from BayesClassifier"
```

Configuration is resolved in this order:

1. CLI options:
   - `--telegram-token` or `--bot-token`
   - `--telegram-chat-id` or `--chat-id`
2. Environment variables:
   - `TELEGRAM_BOT_TOKEN`, `BOT_TOKEN`, or `TELEGRAM_TOKEN`
   - `TELEGRAM_CHAT_ID`, `CHAT_ID`, or `TELEGRAM_TO`
3. `BayesClassifier/App.config` app settings:
   - `TelegramBotToken`
   - `TelegramChatId`

Optional values:

- `--parse-mode` or `TELEGRAM_PARSE_MODE` / `PARSE_MODE`
- `--message-thread-id` / `--thread-id` or `TELEGRAM_THREAD_ID` / `MESSAGE_THREAD_ID`

For shell automation, use:

```bash
./send_telegram_text.sh "Hello from automation"
```

Run `./send_telegram_text.sh --dry-run "Preview"` to validate message resolution without calling Telegram.
