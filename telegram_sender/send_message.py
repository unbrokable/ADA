#!/usr/bin/env python3
"""
Telegram Message Sender - TelegramPostAggregator
Sends messages via Telegram using MTProto API (api_id/api_hash).
"""

import asyncio
import os
import sys
from pathlib import Path
from typing import Optional

from telethon import TelegramClient


# App configuration - TelegramPostAggregator
API_ID = int(os.environ.get("TELEGRAM_API_ID", "13250555"))
API_HASH = os.environ.get("TELEGRAM_API_HASH", "7a3ffc385547658f91428f741fb59480")
SESSION_NAME = os.environ.get("TELEGRAM_SESSION", "telegram_post_aggregator")


async def send_message(
    recipient: str,
    message: str,
    *,
    parse_mode: Optional[str] = None,
) -> bool:
    """
    Send a message to a Telegram chat/channel/user.

    Args:
        recipient: Username (e.g. @channel), chat ID, or phone number
        message: Text message to send
        parse_mode: Optional parse mode ('md' for Markdown, 'html' for HTML)

    Returns:
        True if sent successfully, False otherwise
    """
    session_path = Path(__file__).parent / f"{SESSION_NAME}.session"
    client = TelegramClient(
        str(session_path),
        API_ID,
        API_HASH,
        system_version="4.16.30-vxCUSTOM",
    )

    try:
        await client.start()
        await client.send_message(recipient, message, parse_mode=parse_mode)
        return True
    except Exception as e:
        print(f"Error sending message: {e}", file=sys.stderr)
        return False
    finally:
        await client.disconnect()


def main() -> None:
    """CLI entry point."""
    import argparse

    parser = argparse.ArgumentParser(
        description="Send a message via Telegram (TelegramPostAggregator)"
    )
    parser.add_argument(
        "recipient",
        help="Recipient: @username, chat ID, or phone number (e.g. +1234567890)",
    )
    parser.add_argument(
        "message",
        nargs="?",
        default="",
        help="Message text to send (or read from stdin if omitted)",
    )
    parser.add_argument(
        "-m", "--parse-mode",
        choices=["md", "html"],
        help="Parse mode for formatting (Markdown or HTML)",
    )
    parser.add_argument(
        "-f", "--file",
        help="Read message from file instead of argument",
    )

    args = parser.parse_args()

    if args.file:
        with open(args.file, encoding="utf-8") as f:
            message = f.read()
    elif args.message:
        message = args.message
    else:
        message = sys.stdin.read().strip()

    if not message:
        print("Error: No message provided. Use argument, -f FILE, or stdin.", file=sys.stderr)
        sys.exit(1)

    success = asyncio.run(send_message(args.recipient, message, parse_mode=args.parse_mode))
    sys.exit(0 if success else 1)


if __name__ == "__main__":
    main()
