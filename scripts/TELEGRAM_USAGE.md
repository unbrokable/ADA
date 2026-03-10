# Telegram text sender

Use `send_telegram_text.py` to send messages through the Telegram Bot API.

## Required

- `TELEGRAM_BOT_TOKEN` (or `--token`)
- `TELEGRAM_CHAT_ID` (or `--chat-id`)

## Optional

- `TELEGRAM_TEXT` (or `--text`)
- `--parse-mode MarkdownV2|HTML`
- `--disable-notification`
- `--protect-content`
- `--message-thread-id <id>`
- `--disable-link-preview`
- `--message-effect-id <id>`

## Example

```bash
TELEGRAM_BOT_TOKEN="123456:ABC..." \
TELEGRAM_CHAT_ID="-1001234567890" \
TELEGRAM_TEXT="Daily cron check: OK" \
python3 scripts/send_telegram_text.py
```
