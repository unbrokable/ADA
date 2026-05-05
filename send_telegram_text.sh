#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'USAGE'
Usage: ./send_telegram_text.sh [message] [--dry-run]

Sends a text message through the Telegram Bot API.

Environment variables:
  TELEGRAM_BOT_TOKEN     Required bot token
  TELEGRAM_CHAT_ID       Required target chat ID
  TELEGRAM_MESSAGE       Optional message when no argument is supplied

Optional fallback variable names are also supported:
  TG_BOT_TOKEN, BOT_TOKEN, TELEGRAM_TOKEN
  TG_CHAT_ID, CHAT_ID

Options:
  --dry-run              Print the payload without contacting Telegram
  -h, --help             Show this help text
USAGE
}

message="${TELEGRAM_MESSAGE:-text}"
dry_run=0

while (($#)); do
  case "$1" in
    --dry-run)
      dry_run=1
      shift
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    *)
      message="$1"
      shift
      ;;
  esac
done

bot_token="${TELEGRAM_BOT_TOKEN:-${TG_BOT_TOKEN:-${BOT_TOKEN:-${TELEGRAM_TOKEN:-}}}}"
chat_id="${TELEGRAM_CHAT_ID:-${TG_CHAT_ID:-${CHAT_ID:-}}}"

if [[ -z "$bot_token" ]]; then
  echo "Missing Telegram bot token. Set TELEGRAM_BOT_TOKEN." >&2
  exit 1
fi

if [[ -z "$chat_id" ]]; then
  echo "Missing Telegram chat ID. Set TELEGRAM_CHAT_ID." >&2
  exit 1
fi

if [[ "$dry_run" == 1 ]]; then
  printf 'Would send Telegram message to chat %s: %s\n' "$chat_id" "$message"
  exit 0
fi

curl --fail --silent --show-error \
  --request POST \
  --data-urlencode "chat_id=${chat_id}" \
  --data-urlencode "text=${message}" \
  "https://api.telegram.org/bot${bot_token}/sendMessage" >/dev/null

echo "Telegram text message sent."
