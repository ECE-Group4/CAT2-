# Operations and Support

## Accounts
The database bootstrapper creates a synthetic administrator account (`admin`). The initial password is `ChangeMe123!`. Change it before any shared demonstration. Passwords are stored as salted PBKDF2 hashes by the desktop client; never store plaintext passwords.

## Routine operations
- Review critical queue at the start of each shift.
- Review overdue response escalations and vendor SLA exceptions.
- Reconcile spare-parts receipts and consumption.
- Confirm every repair has verification notes before closure.
- Publish a fresh uptime snapshot for the executive portal.
- Back up the database and alert inbox.

## Recovery
1. Stop desktop clients and the Java receiver.
2. Copy the last known-good database backup into the deployment location.
3. Preserve the existing alert inbox before restoring; do not overwrite pending alerts.
4. Start the receiver and desktop client, import pending alerts, and reconcile message IDs.
5. Record the restore time and any records re-entered manually.

## Configuration
- `HBMS_DB_PATH`: full path to `HBMS.accdb`.
- `HBMS_ALERT_INBOX`: full path to `integration/inbox/alerts.jsonl`.
- Java receiver: `--server --host 0.0.0.0 --port 5055 --inbox <path>`.
- Java sender: `--send --host <workshop-host> --port 5055 --asset <asset-code> --ward <ward> --severity <severity> --description <text>`.

## Production hardening checklist
Use managed identities where possible, enforce workstation access controls, encrypt network traffic (TLS/VPN), firewall the alert port, implement centralised logging, monitor disk capacity, rotate inbox logs, test database backups, review password policy, and obtain institutional safety/security approval.
