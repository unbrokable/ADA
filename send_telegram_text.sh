#!/usr/bin/env bash
set -euo pipefail

if [[ "${1:-}" == "--help" || "${1:-}" == "-h" ]]; then
  echo "Usage: TELEGRAM_BOT_TOKEN=... TELEGRAM_CHAT_ID=... ./send_telegram_text.sh \"message text\""
  echo "Or: echo \"message text\" | TELEGRAM_BOT_TOKEN=... TELEGRAM_CHAT_ID=... ./send_telegram_text.sh"
  exit 0
fi

BOT_TOKEN="${TELEGRAM_BOT_TOKEN:-}"
CHAT_ID="${TELEGRAM_CHAT_ID:-}"
PARSE_MODE="${TELEGRAM_PARSE_MODE:-}"

if [[ -z "$BOT_TOKEN" ]]; then
  echo "TELEGRAM_BOT_TOKEN is required." >&2
  exit 1
fi

if [[ -z "$CHAT_ID" ]]; then
  echo "TELEGRAM_CHAT_ID is required." >&2
  exit 1
fi

if [[ $# -gt 0 ]]; then
  MESSAGE="$*"
else
  MESSAGE="$(cat)"
fi

if [[ -z "${MESSAGE// }" ]]; then
  echo "Message is empty." >&2
  exit 1
fi

URL="https://api.telegram.org/bot${BOT_TOKEN}/sendMessage"
PAYLOAD=(
  --data-urlencode "chat_id=${CHAT_ID}"
  --data-urlencode "text=${MESSAGE}"
)

if [[ -n "$PARSE_MODE" ]]; then
  PAYLOAD+=( --data-urlencode "parse_mode=${PARSE_MODE}" )
fi

RESPONSE="$(
  curl -sS -X POST "$URL" "${PAYLOAD[@]}"
)"

if [[ "$RESPONSE" != *'"ok":true'* ]]; then
  echo "Telegram API error: $RESPONSE" >&2
  exit 1
fi

echo "Telegram message sent."
