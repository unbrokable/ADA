#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'EOF'
Usage:
  send_telegram.sh --text "message text"
  send_telegram.sh --text "message text" --token "<bot_token>" --chat-id "<chat_id>"

Environment variables (used when flags are not provided):
  TELEGRAM_BOT_TOKEN
  TELEGRAM_CHAT_ID

Examples:
  TELEGRAM_BOT_TOKEN=123:abc TELEGRAM_CHAT_ID=123456 ./send_telegram.sh --text "Daily job finished"
  ./send_telegram.sh --text "Hi" --token "123:abc" --chat-id "123456"
EOF
}

TEXT=""
TOKEN="${TELEGRAM_BOT_TOKEN:-}"
CHAT_ID="${TELEGRAM_CHAT_ID:-}"

while [[ $# -gt 0 ]]; do
  case "$1" in
    --text)
      TEXT="${2:-}"
      shift 2
      ;;
    --token)
      TOKEN="${2:-}"
      shift 2
      ;;
    --chat-id)
      CHAT_ID="${2:-}"
      shift 2
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      echo "Unknown argument: $1" >&2
      usage >&2
      exit 1
      ;;
  esac
done

if [[ -z "$TEXT" ]]; then
  echo "Error: --text is required." >&2
  usage >&2
  exit 1
fi

if [[ -z "$TOKEN" ]]; then
  echo "Error: TELEGRAM_BOT_TOKEN (or --token) is required." >&2
  exit 1
fi

if [[ -z "$CHAT_ID" ]]; then
  echo "Error: TELEGRAM_CHAT_ID (or --chat-id) is required." >&2
  exit 1
fi

API_URL="https://api.telegram.org/bot${TOKEN}/sendMessage"

RESPONSE="$(
  curl --silent --show-error --fail \
    --request POST \
    --url "$API_URL" \
    --data-urlencode "chat_id=${CHAT_ID}" \
    --data-urlencode "text=${TEXT}"
)"

if [[ "$RESPONSE" == *'"ok":true'* ]]; then
  echo "Telegram message sent successfully."
else
  echo "Telegram API returned a non-success response:" >&2
  echo "$RESPONSE" >&2
  exit 1
fi
