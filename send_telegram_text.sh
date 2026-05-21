#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'USAGE'
Usage: ./send_telegram_text.sh [options] [message text]

Options:
  --text, --message MESSAGE      Text to send
  --token, --bot-token TOKEN     Telegram bot token
  --chat-id CHAT_ID              Telegram chat ID
  --config FILE                  App.config path (default: BayesClassifier/App.config)
  --dry-run                      Validate inputs without sending
  -h, --help                     Show this help

Credentials are resolved in this order: CLI arguments, environment variables,
then App.config appSettings. Supported environment variables:
TELEGRAM_BOT_TOKEN, TG_BOT_TOKEN, TELEGRAM_CHAT_ID, TG_CHAT_ID, TELEGRAM_TEXT,
and TG_TEXT.
USAGE
}

read_app_setting() {
  local config_file="$1"
  local key="$2"
  local python_bin

  [[ -f "$config_file" ]] || return 0

  python_bin="$(command -v python3 || command -v python || true)"
  [[ -n "$python_bin" ]] || return 0

  "$python_bin" - "$config_file" "$key" <<'PY'
import sys
import xml.etree.ElementTree as ET

config_path, setting_key = sys.argv[1], sys.argv[2]
root = ET.parse(config_path).getroot()

for element in root.findall("./appSettings/add"):
    if element.get("key") == setting_key:
        print(element.get("value", ""))
        break
PY
}

token="${TELEGRAM_BOT_TOKEN:-${TG_BOT_TOKEN:-}}"
chat_id="${TELEGRAM_CHAT_ID:-${TG_CHAT_ID:-}}"
message="${TELEGRAM_TEXT:-${TG_TEXT:-}}"
config_file="${TELEGRAM_CONFIG_FILE:-BayesClassifier/App.config}"
dry_run=false
message_parts=()

while [[ $# -gt 0 ]]; do
  case "$1" in
    --token|--bot-token)
      [[ $# -ge 2 ]] || { echo "Missing value for $1." >&2; exit 2; }
      token="$2"
      shift 2
      ;;
    --chat-id)
      [[ $# -ge 2 ]] || { echo "Missing value for $1." >&2; exit 2; }
      chat_id="$2"
      shift 2
      ;;
    --text|--message|-m)
      [[ $# -ge 2 ]] || { echo "Missing value for $1." >&2; exit 2; }
      message="$2"
      shift 2
      ;;
    --config)
      [[ $# -ge 2 ]] || { echo "Missing value for $1." >&2; exit 2; }
      config_file="$2"
      shift 2
      ;;
    --dry-run)
      dry_run=true
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
      message_parts+=("$1")
      shift
      ;;
  esac
done

if [[ ${#message_parts[@]} -gt 0 ]]; then
  message="${message_parts[*]}"
fi

if [[ -z "$token" ]]; then
  token="$(read_app_setting "$config_file" TelegramBotToken)"
fi

if [[ -z "$chat_id" ]]; then
  chat_id="$(read_app_setting "$config_file" TelegramChatId)"
fi

if [[ -z "$message" ]]; then
  message="$(read_app_setting "$config_file" TelegramText)"
fi

if [[ -z "$message" ]]; then
  message="text"
fi

if [[ -z "$token" ]]; then
  echo "Telegram bot token is required. Set --token, TELEGRAM_BOT_TOKEN, TG_BOT_TOKEN, or App.config TelegramBotToken." >&2
  exit 2
fi

if [[ -z "$chat_id" ]]; then
  echo "Telegram chat ID is required. Set --chat-id, TELEGRAM_CHAT_ID, TG_CHAT_ID, or App.config TelegramChatId." >&2
  exit 2
fi

if [[ "$dry_run" == true ]]; then
  echo "Dry run: Telegram text is ready to send."
  echo "Chat ID: $chat_id"
  echo "Message: $message"
  exit 0
fi

curl --silent --show-error --fail \
  --request POST \
  --data-urlencode "chat_id=$chat_id" \
  --data-urlencode "text=$message" \
  "https://api.telegram.org/bot${token}/sendMessage" >/dev/null

echo "Telegram text sent."
