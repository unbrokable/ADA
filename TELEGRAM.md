## Send text in Telegram

Use `send_telegram_text.py` from the repository root.

### 1) Configure credentials

Set these environment variables:

- `TELEGRAM_BOT_TOKEN`
- `TELEGRAM_CHAT_ID`

Optional:

- `TELEGRAM_TEXT` (default message text if `--text` is not passed)

### 2) Send a message

```bash
python3 send_telegram_text.py --text "send text in telegram"
```

You can also pass credentials directly:

```bash
python3 send_telegram_text.py \
  --bot-token "123456:ABCDEF..." \
  --chat-id "-1001234567890" \
  --text "send text in telegram"
```

### Optional flags

- `--parse-mode MarkdownV2|Markdown|HTML`
- `--message-thread-id <id>`
- `--disable-link-preview`
- `--disable-notification`
