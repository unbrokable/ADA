# Telegram text message sender

Use the helper script from the repository root:

```bash
TELEGRAM_BOT_TOKEN="123456:ABCDEF" \
TELEGRAM_CHAT_ID="123456789" \
./send_telegram_text.sh "Hello from automation"
```

Optional formatting mode:

```bash
TELEGRAM_PARSE_MODE="MarkdownV2"
```

You can also pipe the message:

```bash
echo "Daily report complete." | TELEGRAM_BOT_TOKEN="..." TELEGRAM_CHAT_ID="..." ./send_telegram_text.sh
```
