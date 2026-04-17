#!/usr/bin/env bash

set -euo pipefail

usage() {
  cat <<'EOF'
Usage:
  send_telegram_text.sh [--dry-run] [message]

Environment variables:
  TELEGRAM_BOT_TOKEN   (required unless --dry-run)
  TELEGRAM_CHAT_ID     (required unless --dry-run)
  TELEGRAM_TEXT        (optional fallback when message arg is omitted)
  TELEGRAM_PARSE_MODE  (optional: MarkdownV2, Markdown, HTML)
  TELEGRAM_THREAD_ID   (optional: message_thread_id for forum topics)

Examples:
  TELEGRAM_BOT_TOKEN=... TELEGRAM_CHAT_ID=... ./send_telegram_text.sh "Hello from automation"
  TELEGRAM_TEXT="Daily report ready" ./send_telegram_text.sh --dry-run
EOF
}

DRY_RUN=0
if [[ "${1:-}" == "--help" || "${1:-}" == "-h" ]]; then
  usage
  exit 0
fi

if [[ "${1:-}" == "--dry-run" ]]; then
  DRY_RUN=1
  shift
fi

MESSAGE="${1:-${TELEGRAM_TEXT:-}}"
if [[ -z "$MESSAGE" ]]; then
  echo "Error: message is required (arg or TELEGRAM_TEXT)." >&2
  usage >&2
  exit 1
fi

if [[ "$DRY_RUN" -eq 1 ]]; then
  echo "Dry run: Telegram message would be sent."
  echo "Message: $MESSAGE"
  exit 0
fi

: "${TELEGRAM_BOT_TOKEN:?Error: TELEGRAM_BOT_TOKEN is required.}"
: "${TELEGRAM_CHAT_ID:?Error: TELEGRAM_CHAT_ID is required.}"

API_URL="https://api.telegram.org/bot${TELEGRAM_BOT_TOKEN}/sendMessage"

CURL_ARGS=(
  --silent
  --show-error
  --fail-with-body
  --request POST
  --data-urlencode "chat_id=${TELEGRAM_CHAT_ID}"
  --data-urlencode "text=${MESSAGE}"
  --data-urlencode "disable_web_page_preview=true"
)

if [[ -n "${TELEGRAM_PARSE_MODE:-}" ]]; then
  CURL_ARGS+=(--data-urlencode "parse_mode=${TELEGRAM_PARSE_MODE}")
fi

if [[ -n "${TELEGRAM_THREAD_ID:-}" ]]; then
  CURL_ARGS+=(--data-urlencode "message_thread_id=${TELEGRAM_THREAD_ID}")
fi

RESPONSE="$(curl "${CURL_ARGS[@]}" "$API_URL")"

if [[ "$RESPONSE" == *'"ok":true'* ]]; then
  echo "Telegram message sent successfully."
else
  echo "Telegram API returned an unexpected response:" >&2
  echo "$RESPONSE" >&2
  exit 1
fi
