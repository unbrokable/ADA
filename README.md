# BayesClassifier

BayesClassifier is a .NET Framework WinForms application for classifying spam and non-spam text.

## Send Telegram text

The application can also send a plain text Telegram message from the command line:

```powershell
BayesClassifier.exe --send-telegram --text "text"
```

Credentials can be supplied in this order:

1. Command-line arguments: `--bot-token TOKEN --chat-id CHAT_ID`
2. Environment variables:
   - `TELEGRAM_BOT_TOKEN`, `TELEGRAM_TOKEN`, or `BOT_TOKEN`
   - `TELEGRAM_CHAT_ID` or `CHAT_ID`
3. `App.config` keys: `TelegramBotToken` and `TelegramChatId`

If `--text` or `--message` is omitted, the message defaults to `text`.

For Linux/macOS automation, use the helper script:

```bash
TELEGRAM_BOT_TOKEN="123:abc" TELEGRAM_CHAT_ID="123456" ./send_telegram_text.sh "text"
```

Use `--dry-run` to validate the message without calling Telegram:

```bash
./send_telegram_text.sh --dry-run "text"
```
