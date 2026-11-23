# Project Folder Map

- `README.md` — project overview and quick start.
- `docs/` — product, architecture, workflows, data dictionary, user guide, operations and verification plan.
- `database/` — Access DDL and Windows bootstrapper that creates the `.accdb` and synthetic seed records.
- `desktop/HBMS.Desktop/` — Visual Basic .NET WinForms source and solution.
- `java/` — Java 17 TCP alert receiver/sender Maven project.
- `portal/` — HTML frameset portal, CSS, JavaScript, illustrative map and synthetic KPI snapshot.
- `integration/` — TCP protocol and durable inbox contracts.
- `config/` — example configuration.
- `scripts/` — build and run scripts.
- `tests/` — reserved for deployment-specific automated integration tests; see `docs/TEST_PLAN.md` for required manual verification cases.
- `evidence/` — place real screenshots and run logs here after building on Windows.

The database binary, compiled application binaries and screenshots are generated during deployment; they are not fabricated in this source package.
