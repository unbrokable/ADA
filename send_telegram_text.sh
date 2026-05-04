#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'USAGE'
Usage:
  ./send_telegram_text.sh [message]
  ./send_telegram_text.sh --message "hello" [--bot-token TOKEN] [--chat-id CHAT_ID]
  ./send_telegram_text.sh --dry-run --message "hello"

Environment variables:
  TELEGRAM_BOT_TOKEN, TELEGRAM_TOKEN, BOT_TOKEN
  TELEGRAM_CHAT_ID, CHAT_ID

If no message is supplied, the script sends: text
USAGE
}

message=""
bot_token="${TELEGRAM_BOT_TOKEN:-${TELEGRAM_TOKEN:-${BOT_TOKEN:-}}}"
chat_id="${TELEGRAM_CHAT_ID:-${CHAT_ID:-}}"
dry_run=0

while [[ $# -gt 0 ]]; do
  case "$1" in
    --message|--text)
      if [[ $# -lt 2 ]]; then
        echo "Missing value for $1." >&2
        exit 2
      fi
      message="$2"
      shift 2
      ;;
    --bot-token|--token)
      if [[ $# -lt 2 ]]; then
        echo "Missing value for $1." >&2
        exit 2
      fi
      bot_token="$2"
      shift 2
      ;;
    --chat-id)
      if [[ $# -lt 2 ]]; then
        echo "Missing value for $1." >&2
        exit 2
      fi
      chat_id="$2"
      shift 2
      ;;
    --dry-run)
      dry_run=1
      shift
      ;;
    --help|-h)
      usage
      exit 0
      ;;
    --*)
      echo "Unknown option: $1" >&2
      usage >&2
      exit 2
      ;;
    *)
      if [[ -n "$message" ]]; then
        message+=" "
      fi
      message+="$1"
      shift
      ;;
  esac
done

if [[ -z "$message" ]]; then
  message="text"
fi

if [[ "$dry_run" -eq 1 ]]; then
  echo "Dry run: would send Telegram message:"
  echo "$message"
  exit 0
fi

if [[ -z "$bot_token" ]]; then
  echo "Telegram bot token is required. Set TELEGRAM_BOT_TOKEN, TELEGRAM_TOKEN, BOT_TOKEN, or pass --bot-token." >&2
  exit 1
fi

if [[ -z "$chat_id" ]]; then
  echo "Telegram chat ID is required. Set TELEGRAM_CHAT_ID, CHAT_ID, or pass --chat-id." >&2
  exit 1
fi

response=$(
  curl --silent --show-error --fail-with-body \
    --request POST "https://api.telegram.org/bot${bot_token}/sendMessage" \
    --data-urlencode "chat_id=${chat_id}" \
    --data-urlencode "text=${message}"
)

echo "Telegram message sent."
echo "$response"
