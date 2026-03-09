#!/usr/bin/env python3
import argparse
import asyncio
import json
from pathlib import Path

from telethon import TelegramClient


def load_config(config_path: Path) -> dict:
    with config_path.open("r", encoding="utf-8") as f:
        return json.load(f)


async def send_message(
    config: dict, target: str, text: str, otp_code: str | None, twofa_password: str | None
) -> None:
    api_id = int(config["api_id"])
    api_hash = config["api_hash"]
    phone = config["phone"]
    session_name = config.get("session_name", "telegram/account")

    client = TelegramClient(
        session_name,
        api_id,
        api_hash,
        device_model="Cursor Automation",
        app_version="TelegramPostAggregator",
        system_version="Linux",
    )

    start_kwargs = {"phone": phone}
    if otp_code:
        start_kwargs["code_callback"] = lambda: otp_code
    if twofa_password:
        start_kwargs["password"] = twofa_password

    await client.start(**start_kwargs)
    await client.send_message(target, text)
    await client.disconnect()


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(
        description="Send a Telegram message using your personal account."
    )
    parser.add_argument(
        "--config",
        default="telegram/account.local.json",
        help="Path to account config JSON file.",
    )
    parser.add_argument(
        "--target",
        required=True,
        help="Telegram username/chat/phone target (for example: @username).",
    )
    parser.add_argument("--text", required=True, help="Message text to send.")
    parser.add_argument(
        "--code",
        default=None,
        help="Telegram login code (OTP). Use this for non-interactive execution.",
    )
    parser.add_argument(
        "--password",
        default=None,
        help="Telegram 2FA password if your account requires it.",
    )
    return parser.parse_args()


def main() -> None:
    args = parse_args()
    config_path = Path(args.config)

    if not config_path.exists():
        raise FileNotFoundError(
            f"Config file not found: {config_path}. "
            "Create it from telegram/account.example.json."
        )

    config = load_config(config_path)
    asyncio.run(send_message(config, args.target, args.text, args.code, args.password))
    print("Message sent successfully.")


if __name__ == "__main__":
    main()
