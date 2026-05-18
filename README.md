# BayesClassifier

Legacy WinForms Bayesian spam classifier.

## Send Telegram text

The repository includes two ways to send a Telegram text message through the
Telegram Bot API:

- `send_telegram_text.sh` for automation environments that do not have .NET
  Framework tooling available.
- `BayesClassifier.exe --send-telegram` when the Windows application is built.

With no message argument, both entry points send the text `text`.

```bash
./send_telegram_text.sh
./send_telegram_text.sh "hello from automation"
./send_telegram_text.sh --message "hello" --token "$TELEGRAM_BOT_TOKEN" --chat-id "$TELEGRAM_CHAT_ID"
```

Credentials can be provided with CLI options, environment variables, or
`BayesClassifier/App.config` app settings. CLI options take precedence over
environment variables, which take precedence over config values.

Required values:

- `TELEGRAM_BOT_TOKEN` or `TelegramBotToken`
- `TELEGRAM_CHAT_ID` or `TelegramChatId`

Optional values:

- `TELEGRAM_TEXT` or `TelegramText`
- `TELEGRAM_PARSE_MODE` or `TelegramParseMode`

Use `./send_telegram_text.sh --dry-run` to validate configuration without
sending a message.
