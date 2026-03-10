#!/usr/bin/env python3
"""Send a text message through the Telegram Bot API."""

from __future__ import annotations

import argparse
import json
import os
import sys
from datetime import datetime, timezone
from urllib.error import HTTPError, URLError
from urllib.request import Request, urlopen


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Send a Telegram text message using a bot token."
    )
    parser.add_argument(
        "--token",
        default=os.getenv("TELEGRAM_BOT_TOKEN", ""),
        help="Telegram bot token (or set TELEGRAM_BOT_TOKEN).",
    )
    parser.add_argument(
        "--chat-id",
        default=os.getenv("TELEGRAM_CHAT_ID", ""),
        help="Destination chat id (or set TELEGRAM_CHAT_ID).",
    )
    parser.add_argument(
        "--text",
        default=os.getenv("TELEGRAM_TEXT", ""),
        help="Text to send (or set TELEGRAM_TEXT).",
    )
    parser.add_argument(
        "--parse-mode",
        choices=["MarkdownV2", "HTML"],
        help="Optional Telegram parse mode.",
    )
    parser.add_argument(
        "--disable-notification",
        action="store_true",
        help="Send silently without sound/vibration.",
    )
    parser.add_argument(
        "--protect-content",
        action="store_true",
        help="Protect message from forwarding/saving.",
    )
    parser.add_argument(
        "--message-thread-id",
        type=int,
        help="Target topic thread id for forum supergroups.",
    )
    parser.add_argument(
        "--disable-link-preview",
        action="store_true",
        help="Disable link previews using link_preview_options.",
    )
    parser.add_argument(
        "--message-effect-id",
        help="Optional message effect id supported by Telegram clients.",
    )
    return parser.parse_args()


def build_payload(args: argparse.Namespace) -> dict[str, object]:
    text = args.text.strip()
    if not text:
        now = datetime.now(timezone.utc).strftime("%Y-%m-%d %H:%M:%S UTC")
        text = f"Automation ping at {now}"

    payload: dict[str, object] = {
        "chat_id": args.chat_id,
        "text": text,
        "disable_notification": args.disable_notification,
        "protect_content": args.protect_content,
    }

    if args.parse_mode:
        payload["parse_mode"] = args.parse_mode
    if args.message_thread_id is not None:
        payload["message_thread_id"] = args.message_thread_id
    if args.disable_link_preview:
        # Uses the modern Telegram Bot API field instead of legacy flags.
        payload["link_preview_options"] = {"is_disabled": True}
    if args.message_effect_id:
        payload["message_effect_id"] = args.message_effect_id

    return payload


def send_message(token: str, payload: dict[str, object]) -> dict[str, object]:
    endpoint = f"https://api.telegram.org/bot{token}/sendMessage"
    body = json.dumps(payload).encode("utf-8")
    request = Request(
        endpoint,
        data=body,
        headers={"Content-Type": "application/json"},
        method="POST",
    )

    try:
        with urlopen(request, timeout=20) as response:
            raw = response.read().decode("utf-8")
    except HTTPError as err:
        detail = err.read().decode("utf-8", errors="replace")
        raise RuntimeError(f"Telegram API HTTP {err.code}: {detail}") from err
    except URLError as err:
        raise RuntimeError(f"Telegram API request failed: {err}") from err

    try:
        data = json.loads(raw)
    except json.JSONDecodeError as err:
        raise RuntimeError(f"Invalid Telegram API response: {raw}") from err

    if not data.get("ok"):
        raise RuntimeError(f"Telegram API error: {data}")

    return data


def main() -> int:
    args = parse_args()
    if not args.token:
        print(
            "Missing bot token. Set TELEGRAM_BOT_TOKEN or pass --token.",
            file=sys.stderr,
        )
        return 2
    if not args.chat_id:
        print(
            "Missing chat id. Set TELEGRAM_CHAT_ID or pass --chat-id.",
            file=sys.stderr,
        )
        return 2

    payload = build_payload(args)
    try:
        response = send_message(args.token, payload)
    except RuntimeError as err:
        print(str(err), file=sys.stderr)
        return 1

    result = response.get("result", {})
    message_id = result.get("message_id", "unknown")
    chat = result.get("chat", {})
    destination = chat.get("id", args.chat_id)
    print(f"Sent Telegram message {message_id} to chat {destination}.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
