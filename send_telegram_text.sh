#!/usr/bin/env bash
set -euo pipefail

usage() {
  cat <<'USAGE'
Usage: ./send_telegram_text.sh [message] [options]

Sends a Telegram text message through the Bot API. With no message argument,
the script sends "text".

Options:
  -m, --message <text>       Message text to send
      --token <token>        Telegram bot token
      --chat-id <chat_id>    Telegram chat ID
      --parse-mode <mode>    Optional parse mode, such as MarkdownV2 or HTML
      --dry-run              Validate inputs without sending
  -h, --help                 Show this help

Configuration precedence:
  CLI options, environment variables, BayesClassifier/App.config, defaults.

Environment variables:
  TELEGRAM_BOT_TOKEN, TELEGRAM_CHAT_ID, TELEGRAM_TEXT, TELEGRAM_PARSE_MODE
USAGE
}

read_config_key() {
  local key="$1"
  local config_path="$2"

  if ! command -v python3 >/dev/null 2>&1 || [[ ! -f "$config_path" ]]; then
    return 0
  fi

  python3 - "$key" "$config_path" <<'PY'
import sys
import xml.etree.ElementTree as ET

key, config_path = sys.argv[1], sys.argv[2]

try:
    root = ET.parse(config_path).getroot()
except ET.ParseError:
    sys.exit(0)

for item in root.findall("./appSettings/add"):
    if item.attrib.get("key") == key:
        value = item.attrib.get("value", "")
        if value:
            print(value)
        break
PY
}

send_message() {
  TELEGRAM_BOT_TOKEN="$token" \
  TELEGRAM_CHAT_ID="$chat_id" \
  TELEGRAM_TEXT="$message" \
  TELEGRAM_PARSE_MODE="$parse_mode" \
  python3 <<'PY'
import json
import os
import sys
import urllib.error
import urllib.parse
import urllib.request

token = os.environ["TELEGRAM_BOT_TOKEN"]
chat_id = os.environ["TELEGRAM_CHAT_ID"]
text = os.environ["TELEGRAM_TEXT"]
parse_mode = os.environ.get("TELEGRAM_PARSE_MODE", "")

payload = {
    "chat_id": chat_id,
    "text": text,
}

if parse_mode:
    payload["parse_mode"] = parse_mode

request = urllib.request.Request(
    f"https://api.telegram.org/bot{token}/sendMessage",
    data=urllib.parse.urlencode(payload).encode("utf-8"),
    method="POST",
)

try:
    with urllib.request.urlopen(request, timeout=30) as response:
        body = response.read().decode("utf-8")
except urllib.error.HTTPError as exc:
    body = exc.read().decode("utf-8", errors="replace")
    print(f"Telegram API returned HTTP {exc.code}: {body}", file=sys.stderr)
    sys.exit(1)
except urllib.error.URLError as exc:
    print(f"Failed to reach Telegram API: {exc}", file=sys.stderr)
    sys.exit(1)

try:
    data = json.loads(body)
except json.JSONDecodeError:
    print("Telegram API returned a non-JSON response.", file=sys.stderr)
    sys.exit(1)

if not data.get("ok"):
    print(f"Telegram API rejected the message: {body}", file=sys.stderr)
    sys.exit(1)

print("Telegram message sent.")
PY
}

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
config_path="$script_dir/BayesClassifier/App.config"

token="${TELEGRAM_BOT_TOKEN:-${TELEGRAM_TOKEN:-${BOT_TOKEN:-}}}"
chat_id="${TELEGRAM_CHAT_ID:-${CHAT_ID:-}}"
message="${TELEGRAM_TEXT:-text}"
parse_mode="${TELEGRAM_PARSE_MODE:-}"
dry_run=false
positionals=()

while [[ $# -gt 0 ]]; do
  case "$1" in
    -m|--message)
      [[ $# -ge 2 ]] || { echo "Missing value for $1." >&2; exit 2; }
      message="$2"
      shift 2
      ;;
    --token|--telegram-token)
      [[ $# -ge 2 ]] || { echo "Missing value for $1." >&2; exit 2; }
      token="$2"
      shift 2
      ;;
    --chat-id|--telegram-chat-id)
      [[ $# -ge 2 ]] || { echo "Missing value for $1." >&2; exit 2; }
      chat_id="$2"
      shift 2
      ;;
    --parse-mode)
      [[ $# -ge 2 ]] || { echo "Missing value for $1." >&2; exit 2; }
      parse_mode="$2"
      shift 2
      ;;
    --dry-run)
      dry_run=true
      shift
      ;;
    -h|--help)
      usage
      exit 0
      ;;
    --)
      shift
      while [[ $# -gt 0 ]]; do
        positionals+=("$1")
        shift
      done
      ;;
    -*)
      echo "Unknown option: $1" >&2
      usage >&2
      exit 2
      ;;
    *)
      positionals+=("$1")
      shift
      ;;
  esac
done

if [[ ${#positionals[@]} -gt 0 ]]; then
  message="${positionals[*]}"
fi

token="${token:-$(read_config_key TelegramBotToken "$config_path")}"
chat_id="${chat_id:-$(read_config_key TelegramChatId "$config_path")}"
message="${message:-$(read_config_key TelegramText "$config_path")}"
parse_mode="${parse_mode:-$(read_config_key TelegramParseMode "$config_path")}"
message="${message:-text}"

if [[ "$dry_run" == true ]]; then
  echo "Telegram dry run:"
  echo "  token: $([[ -n "$token" ]] && echo set || echo missing)"
  echo "  chat_id: $([[ -n "$chat_id" ]] && echo set || echo missing)"
  echo "  parse_mode: ${parse_mode:-none}"
  echo "  message: $message"
  exit 0
fi

if [[ -z "$token" ]]; then
  echo "Telegram bot token is required. Set TELEGRAM_BOT_TOKEN or pass --token." >&2
  exit 2
fi

if [[ -z "$chat_id" ]]; then
  echo "Telegram chat ID is required. Set TELEGRAM_CHAT_ID or pass --chat-id." >&2
  exit 2
fi

if ! command -v python3 >/dev/null 2>&1; then
  echo "python3 is required to send the Telegram message." >&2
  exit 2
fi

send_message
