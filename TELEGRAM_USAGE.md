## Telegram text sender

Use `send_telegram_text.sh` to send a plain text Telegram message through the Bot API.

### Required environment variables

- `TELEGRAM_BOT_TOKEN` (or `BOT_TOKEN` / `TELEGRAM_TOKEN`)
- `TELEGRAM_CHAT_ID` (or `CHAT_ID` / `TELEGRAM_TO`)

### Message input precedence

1. First positional argument
2. `TELEGRAM_TEXT`
3. `MESSAGE_TEXT`
4. `MESSAGE`
5. Auto-generated default message

### Examples

Dry run:

```bash
./send_telegram_text.sh --dry-run "Preview only"
```

Send a message:

```bash
TELEGRAM_BOT_TOKEN="123456:abc" \
TELEGRAM_CHAT_ID="-1001234567890" \
./send_telegram_text.sh "Hello from cron automation"
```

Use existing exported env vars:

```bash
./send_telegram_text.sh "Daily status update"
```
