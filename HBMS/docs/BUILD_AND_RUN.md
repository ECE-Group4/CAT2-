# Build and run guide

## Windows desktop prerequisites

- Windows 10/11 or Windows Server supported by .NET 8
- .NET 8 SDK
- Microsoft Access Database Engine 2016/365 ACE OLEDB provider
- Matching 32-bit/64-bit process and ACE provider architecture

Run database bootstrapper before launching the desktop client. Set `HBMS_DB_PATH` to override the default database path. The first seeded administrator credential is `admin` / `ChangeMe123!`; rotate it before shared use.

## Java workshop service

Use Java 17 or newer and Maven 3.9 or newer. The receiver listens on a configured interface and TCP port. On a multi-PC network, set the host to the workshop server's private LAN address and permit the port only from trusted ward PCs. Do not expose the listener directly to the public internet.

The receiver persists JSONL before acknowledging. Back up the inbox and database together. Import messages from the desktop UI; inspect and reconcile any messages that remain in the inbox after a failed import.

## Portal

Serve the `portal` directory through a static web server when local-file security restrictions prevent frames or JSON loading. Publish snapshots from the desktop application to the configured portal data path. The portal must be read-only for executive users.

## Deployment checklist

- Change seeded administrator credentials.
- Replace demo reference data with approved non-patient equipment and ward data.
- Set database and inbox paths with least-privilege filesystem permissions.
- Restrict Java TCP port using host firewall rules.
- Configure backup and restore procedures and test recovery.
- Synchronise workstation clocks; store timestamps with explicit offset or a documented common timezone.
- Keep the system out of clinical control loops; it is a maintenance coordination system.
