# BayesClassifier

## Send text in Telegram

This project now supports sending a Telegram text message from command line mode (without opening the WinForms UI).

### Required environment variables

- `TELEGRAM_BOT_TOKEN` - your Telegram bot token
- `TELEGRAM_CHAT_ID` - target chat ID (user, group, or channel)

### Optional environment variable

- `TELEGRAM_TEXT` - message text, used if message is not passed in CLI args

### Usage

Send text by passing the message after the flag:

```bash
BayesClassifier.exe --send-telegram "Hello from automation"
```

or

```bash
BayesClassifier.exe --telegram-text "Daily status: OK"
```

You can also use `TELEGRAM_TEXT` together with just the flag:

```bash
export TELEGRAM_TEXT="Cron notification"
BayesClassifier.exe --send-telegram
```

If Telegram sending mode is not requested, the application runs the original WinForms UI.
