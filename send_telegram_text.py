#!/usr/bin/env python3
"""Send a text message using the Telegram Bot API."""

from __future__ import annotations

import argparse
import json
import os
import sys
from datetime import datetime, timezone
from typing import Any, Dict
from urllib import error, request


API_BASE = "https://api.telegram.org"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Send a text message to a Telegram chat using a bot token."
    )
    parser.add_argument(
        "--text",
        help="Message text to send. If omitted, a timestamp heartbeat is sent.",
    )
    parser.add_argument(
        "--bot-token",
        help="Telegram bot token. Falls back to TELEGRAM_BOT_TOKEN.",
    )
    parser.add_argument(
        "--chat-id",
        help="Target Telegram chat ID. Falls back to TELEGRAM_CHAT_ID.",
    )
    parser.add_argument(
        "--parse-mode",
        choices=("Markdown", "MarkdownV2", "HTML"),
        help="Optional Telegram parse mode for text formatting.",
    )
    parser.add_argument(
        "--message-thread-id",
        type=int,
        help="Optional topic thread ID for forum supergroups.",
    )
    parser.add_argument(
        "--disable-link-preview",
        action="store_true",
        help="Disable previews via link_preview_options (new Bot API field).",
    )
    parser.add_argument(
        "--disable-notification",
        action="store_true",
        help="Send silently without notification sound.",
    )
    return parser.parse_args()


def send_message(
    *,
    bot_token: str,
    chat_id: str,
    text: str,
    parse_mode: str | None,
    message_thread_id: int | None,
    disable_link_preview: bool,
    disable_notification: bool,
) -> Dict[str, Any]:
    payload: Dict[str, Any] = {
        "chat_id": chat_id,
        "text": text,
        "disable_notification": disable_notification,
    }
    if parse_mode:
        payload["parse_mode"] = parse_mode
    if message_thread_id is not None:
        payload["message_thread_id"] = message_thread_id
    if disable_link_preview:
        payload["link_preview_options"] = {"is_disabled": True}

    encoded_payload = json.dumps(payload).encode("utf-8")
    endpoint = f"{API_BASE}/bot{bot_token}/sendMessage"
    req = request.Request(
        endpoint,
        data=encoded_payload,
        method="POST",
        headers={"Content-Type": "application/json"},
    )

    try:
        with request.urlopen(req, timeout=30) as response:
            body = response.read().decode("utf-8")
    except error.HTTPError as exc:
        body = exc.read().decode("utf-8", errors="replace")
        raise RuntimeError(
            f"Telegram API HTTP {exc.code}. Response: {body}"
        ) from exc
    except error.URLError as exc:
        raise RuntimeError(f"Failed to reach Telegram API: {exc.reason}") from exc

    try:
        parsed_response = json.loads(body)
    except json.JSONDecodeError as exc:
        raise RuntimeError(f"Non-JSON Telegram response: {body}") from exc

    if not parsed_response.get("ok"):
        raise RuntimeError(f"Telegram API returned ok=false: {parsed_response}")

    return parsed_response


def main() -> int:
    args = parse_args()

    bot_token = args.bot_token or os.getenv("TELEGRAM_BOT_TOKEN")
    chat_id = args.chat_id or os.getenv("TELEGRAM_CHAT_ID")
    if not bot_token:
        print("Missing Telegram bot token. Use --bot-token or TELEGRAM_BOT_TOKEN.", file=sys.stderr)
        return 2
    if not chat_id:
        print("Missing Telegram chat ID. Use --chat-id or TELEGRAM_CHAT_ID.", file=sys.stderr)
        return 2

    default_text = f"Automation heartbeat: {datetime.now(timezone.utc).isoformat()}"
    text = args.text or os.getenv("TELEGRAM_TEXT") or default_text

    try:
        response = send_message(
            bot_token=bot_token,
            chat_id=chat_id,
            text=text,
            parse_mode=args.parse_mode,
            message_thread_id=args.message_thread_id,
            disable_link_preview=args.disable_link_preview,
            disable_notification=args.disable_notification,
        )
    except RuntimeError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    message_id = response.get("result", {}).get("message_id", "<unknown>")
    print(f"Message sent successfully. message_id={message_id}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
