# BayesClassifier

WinForms spam classifier with an optional CLI mode to send plain text messages to Telegram.

## Send text in Telegram

Build the app, then run with:

```bash
BayesClassifier.exe --send-telegram "Hello from BayesClassifier"
```

Credentials are resolved in this order:

1. CLI overrides:
   - `--telegram-bot-token <token>`
   - `--telegram-chat-id <chatId>`
2. Environment variables:
   - `TELEGRAM_BOT_TOKEN`
   - `TELEGRAM_CHAT_ID`
3. `App.config` appSettings keys:
   - `TelegramBotToken`
   - `TelegramChatId`

Example:

```bash
TELEGRAM_BOT_TOKEN=123:abc TELEGRAM_CHAT_ID=123456 BayesClassifier.exe --send-telegram "Daily report is ready"
```

