#!/usr/bin/env bash
set -euo pipefail

print_usage() {
  echo "Usage:"
  echo "  TELEGRAM_BOT_TOKEN=... TELEGRAM_CHAT_ID=... ./send_telegram_text.sh \"message text\""
  echo "  echo \"message text\" | TELEGRAM_BOT_TOKEN=... TELEGRAM_CHAT_ID=... ./send_telegram_text.sh"
  echo
  echo "Optional env vars:"
  echo "  TELEGRAM_PARSE_MODE                (MarkdownV2, HTML, Markdown)"
  echo "  TELEGRAM_DISABLE_WEB_PAGE_PREVIEW (true/false)"
  echo "  TELEGRAM_MESSAGE_THREAD_ID         (for forum topics)"
}

if [[ "${1:-}" == "--help" || "${1:-}" == "-h" ]]; then
  print_usage
  exit 0
fi

BOT_TOKEN="${TELEGRAM_BOT_TOKEN:-${TELEGRAM_TOKEN:-}}"
CHAT_ID="${TELEGRAM_CHAT_ID:-}"
PARSE_MODE="${TELEGRAM_PARSE_MODE:-}"
DISABLE_PREVIEW="${TELEGRAM_DISABLE_WEB_PAGE_PREVIEW:-}"
THREAD_ID="${TELEGRAM_MESSAGE_THREAD_ID:-}"

if [[ -z "$BOT_TOKEN" ]]; then
  echo "TELEGRAM_BOT_TOKEN (or TELEGRAM_TOKEN) is required." >&2
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
  PAYLOAD+=(--data-urlencode "parse_mode=${PARSE_MODE}")
fi

if [[ -n "$DISABLE_PREVIEW" ]]; then
  PAYLOAD+=(--data-urlencode "disable_web_page_preview=${DISABLE_PREVIEW}")
fi

if [[ -n "$THREAD_ID" ]]; then
  PAYLOAD+=(--data-urlencode "message_thread_id=${THREAD_ID}")
fi

RESPONSE="$(
  curl -sS --retry 3 --retry-delay 2 --retry-all-errors -X POST "$URL" "${PAYLOAD[@]}"
)"

if [[ "$RESPONSE" != *'"ok":true'* ]]; then
  echo "Telegram API error: $RESPONSE" >&2
  exit 1
fi

echo "Telegram message sent."
