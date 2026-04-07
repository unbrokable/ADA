# Telegram text sender

Use the script at repository root to send a message through the Telegram Bot API.

## Required variables

- `TELEGRAM_BOT_TOKEN` (or `TELEGRAM_TOKEN`)
- `TELEGRAM_CHAT_ID`

## Send a direct text message

```bash
TELEGRAM_BOT_TOKEN="123456:ABCDEF" \
TELEGRAM_CHAT_ID="123456789" \
./send_telegram_text.sh "Hello from automation"
```

## Send message from stdin

```bash
echo "Daily job completed." | TELEGRAM_BOT_TOKEN="..." TELEGRAM_CHAT_ID="..." ./send_telegram_text.sh
```

## Optional variables

- `TELEGRAM_PARSE_MODE` (`MarkdownV2`, `HTML`, or `Markdown`)
- `TELEGRAM_DISABLE_WEB_PAGE_PREVIEW` (`true`/`false`)
- `TELEGRAM_MESSAGE_THREAD_ID` (for forum topic messages)
