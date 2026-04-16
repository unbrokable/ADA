## BayesClassifier

### Send text in Telegram

This app supports a CLI mode for sending a Telegram text message:

```bash
BayesClassifier.exe --send-telegram --message "Hello from automation"
```

Credentials can be provided in this priority order:

1. CLI arguments:
   - `--telegram-token` (or `--bot-token`)
   - `--telegram-chat-id` (or `--chat-id`)
2. Environment variables:
   - `TELEGRAM_BOT_TOKEN`
   - `TELEGRAM_CHAT_ID`
3. `App.config` appSettings:
   - `TelegramBotToken`
   - `TelegramChatId`

Example with explicit credentials:

```bash
BayesClassifier.exe --send-telegram --message "Daily ping" --telegram-token "<token>" --telegram-chat-id "<chat_id>"
```
