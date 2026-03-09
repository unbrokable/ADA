# Telegram personal-account sender

This setup sends Telegram messages from your personal account (not a bot) using MTProto via Telethon.

## 1) Install dependency (latest)

```bash
pip install -U telethon
```

## 2) Create local config file

```bash
cp telegram/account.example.json telegram/account.local.json
```

Update `telegram/account.local.json` with your real `api_hash` and phone number.

## 3) Send a message

```bash
python telegram/send_from_account.py --target "@username" --text "hello"
```

On first run, Telegram will ask for the login code (and 2FA password if enabled).  
After successful login, the local session file is reused for future runs.
