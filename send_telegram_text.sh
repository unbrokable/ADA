#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'USAGE'
Usage: ./send_telegram_text.sh [--token TOKEN] [--chat-id CHAT_ID] [--message MESSAGE] [--dry-run] [TEXT...]

Sends a plain text message through the Telegram Bot API.

Credentials can be supplied with options or environment variables:
  TELEGRAM_BOT_TOKEN, BOT_TOKEN, or TG_BOT_TOKEN
  TELEGRAM_CHAT_ID, TELEGRAM_CHATID, or TG_CHAT_ID

If no message is supplied, TELEGRAM_MESSAGE is used, then "text".
USAGE
}

token="${TELEGRAM_BOT_TOKEN:-${BOT_TOKEN:-${TG_BOT_TOKEN:-}}}"
chat_id="${TELEGRAM_CHAT_ID:-${TELEGRAM_CHATID:-${TG_CHAT_ID:-}}}"
message="${TELEGRAM_MESSAGE:-}"
dry_run=0
message_parts=()

while (($#)); do
  case "$1" in
    --token|--bot-token|--telegram-token)
      if (($# < 2)); then
        echo "Missing value for $1." >&2
        usage >&2
        exit 2
      fi
      token="$2"
      shift 2
      ;;
    --chat-id|--telegram-chat-id)
      if (($# < 2)); then
        echo "Missing value for $1." >&2
        usage >&2
        exit 2
      fi
      chat_id="$2"
      shift 2
      ;;
    --message|--text)
      if (($# < 2)); then
        echo "Missing value for $1." >&2
        usage >&2
        exit 2
      fi
      message="$2"
      shift 2
      ;;
    --dry-run)
      dry_run=1
      shift
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    --*)
      echo "Unknown option: $1" >&2
      usage >&2
      exit 2
      ;;
    *)
      message_parts+=("$1")
      shift
      ;;
  esac
done

if ((${#message_parts[@]})); then
  message="${message_parts[*]}"
fi

if [[ -z "${message}" ]]; then
  message="text"
fi

if ((dry_run)); then
  if [[ -z "${chat_id}" ]]; then
    chat_label="<missing>"
  elif ((${#chat_id} <= 4)); then
    chat_label="****"
  else
    chat_label="****${chat_id: -4}"
  fi

  echo "Dry run: would send Telegram message to chat '${chat_label}'."
  printf '%s\n' "${message}"
  exit 0
fi

if [[ -z "${token}" ]]; then
  echo "Missing Telegram bot token. Set TELEGRAM_BOT_TOKEN or pass --token." >&2
  exit 2
fi

if [[ -z "${chat_id}" ]]; then
  echo "Missing Telegram chat ID. Set TELEGRAM_CHAT_ID or pass --chat-id." >&2
  exit 2
fi

if ! command -v curl >/dev/null 2>&1; then
  echo "curl is required to send Telegram messages." >&2
  exit 2
fi

response="$(
  curl -sS -w '\n%{http_code}' \
    --request POST \
    --url "https://api.telegram.org/bot${token}/sendMessage" \
    --data-urlencode "chat_id=${chat_id}" \
    --data-urlencode "text=${message}"
)"

body="${response%$'\n'*}"
status="${response##*$'\n'}"

if [[ "${status}" -lt 200 || "${status}" -ge 300 ]]; then
  echo "Telegram API returned HTTP ${status}: ${body}" >&2
  exit 1
fi

if [[ "${body}" != *'"ok":true'* ]]; then
  echo "Telegram API did not confirm success: ${body}" >&2
  exit 1
fi

echo "Telegram message sent."
