# Telegram Message Sender (TelegramPostAggregator)

Send messages via Telegram using the MTProto API with your TelegramPostAggregator app credentials.

## App Configuration

- **App**: TelegramPostAggregator
- **api_id**: 13250555
- **api_hash**: 7a3ffc385547658f91428f741fb59480

## Setup

1. Install dependencies:

   ```bash
   pip install -r requirements.txt
   # or: pip3 install -r requirements.txt
   ```

2. On first run, you will be prompted for:
   - Your phone number (with country code, e.g. +1234567890)
   - The login code sent to your Telegram app

   A session file is saved locally so you won't need to authenticate again.

## Usage

```bash
# Send to username/channel
python3 send_message.py @channel_name "Hello from TelegramPostAggregator!"

# Send to chat ID
python3 send_message.py 123456789 "Your message here"

# Read message from file
python3 send_message.py @me -f message.txt

# Read message from stdin
echo "Hello" | python3 send_message.py @me

# With Markdown formatting
python3 send_message.py @channel -m md "**Bold** and *italic*"
```

## Environment Variables

Override credentials via environment variables:

- `TELEGRAM_API_ID` - API ID (default: 13250555)
- `TELEGRAM_API_HASH` - API hash
- `TELEGRAM_SESSION` - Session file name (default: telegram_post_aggregator)

## Security

For production, prefer environment variables over hardcoded credentials:

```bash
export TELEGRAM_API_ID=13250555
export TELEGRAM_API_HASH=your_api_hash
python3 send_message.py @recipient "Message"
```
