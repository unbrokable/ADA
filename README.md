# BayesClassifier

BayesClassifier is a .NET Framework WinForms application for classifying text.

## Send text in Telegram

The repository includes two ways to send a plain text Telegram message through the Telegram Bot API.

### Shell helper

```bash
TELEGRAM_BOT_TOKEN="123456:bot-token" \
TELEGRAM_CHAT_ID="123456789" \
./send_telegram_text.sh "text"
```

You can also pass credentials explicitly:

```bash
./send_telegram_text.sh --token "123456:bot-token" --chat-id "123456789" --message "text"
```

Use `--dry-run` to validate the command without contacting Telegram:

```bash
./send_telegram_text.sh --dry-run "text"
```

### Application CLI

The WinForms app keeps its normal UI startup when no Telegram argument is provided. To send text from the compiled executable:

```powershell
BayesClassifier.exe --send-telegram --token "123456:bot-token" --chat-id "123456789" --message "text"
```

The CLI reads credentials in this order:

1. Command-line options: `--token` / `--chat-id`
2. Environment variables: `TELEGRAM_BOT_TOKEN`, `BOT_TOKEN`, or `TG_BOT_TOKEN`; `TELEGRAM_CHAT_ID`, `TELEGRAM_CHATID`, or `TG_CHAT_ID`
3. `App.config` app settings: `TelegramBotToken` / `TelegramChatId`

If no message is supplied, `TELEGRAM_MESSAGE` is used, then the default message `text`.
