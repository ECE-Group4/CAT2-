# HBMS — Hospital Biomedical Maintenance System

This folder contains the **actual source code and project assets** for an independent biomedical equipment maintenance management application. It is not a document-only bundle. The code is separated by executable component and includes database creation, Windows desktop source, Java networking source, browser portal, integration contracts, tests, build scripts and operator documentation.

## Source tree

```text
HBMS/
├── database/                 Access schema, seed data and database bootstrapper
├── desktop/HBMS.Desktop/     VB.NET Windows Forms application (.vb, .vbproj, .sln)
│   ├── Data/                 OleDb database access
│   ├── Domain/               Domain models
│   ├── Forms/                Login, operations, breakdown, registers, maintenance,
│   │                         spare parts, repair costs and confirmation dialogs
│   └── Services/             Application services, SLA escalation, priority queue,
│                             password hashing and alert inbox import
├── java/                     Java 17 TCP alert sender/receiver and Maven build
│   └── src/                  Production Java source and JUnit tests
├── portal/                   HTML frameset executive portal, CSS, JS, SVG map and data
├── integration/              Alert wire protocol and durable inbox contract
├── docs/                      Architecture, product spec, workflows, data dictionary,
│                              lifecycle diagrams, user guide and operations
├── tests/                     Runnable portal and Java validation smoke tests
├── scripts/                   Build and run scripts
└── config/                    Example configuration
```

## Components

### 1. VB.NET desktop application

Windows Forms application targeting .NET 8 for Windows. The source includes sign-in, operational dashboard, breakdown logging and queue, equipment/ward/vendor/SLA register management, response recording, engineer assignment, maintenance progression, spare-part usage, repair-cost capture, alert inbox import, priority ordering and executive snapshot publishing. Database access is parameterised through OleDb.

### 2. Microsoft Access database

`database/schema.sql` defines the relational model. `database/Create-HBMSDatabase.ps1` creates the `.accdb` database on Windows using Access Database Engine/ACE and seeds synthetic reference records. The native database is generated locally by the bootstrapper; it is not a binary file fabricated on Linux.

### 3. Java network alert service

Java 17 sender and concurrent TCP receiver. The receiver validates bounded JSON messages, appends each accepted message to a JSON Lines inbox and forces the write before acknowledging. The desktop importer consumes that inbox and creates the corresponding breakdown record. A `RECEIVED` acknowledgement means durable inbox receipt, not completed Access import.

Build with Java 17+ and Maven 3.9+:

```bash
cd java
mvn clean test package
```

### 4. HTML executive portal

Classic HTML frameset layout with navigation, executive overview, equipment list, ward map, maintenance view, SLA view and department uptime. The bundled data is synthetic. Use the desktop application's portal publishing function to create an updated snapshot; the portal is a read-only executive view, not a second operational database.

## Build and run

### Windows workstation (desktop + Access)

1. Install .NET 8 SDK and Microsoft Access Database Engine (ACE OLEDB 12.0). Keep application and ACE bitness aligned.
2. From the project root, run:
   `powershell -ExecutionPolicy Bypass -File .\database\Create-HBMSDatabase.ps1`
3. Build:
   `powershell -ExecutionPolicy Bypass -File .\scripts\build-desktop.ps1`
4. Run the executable from `desktop\HBMS.Desktop\bin\Release\net8.0-windows\`.
5. Seeded local account: `admin` / `ChangeMe123!`. Change the password before shared use.

### Java alert service

```bash
cd java && mvn clean test package
java -jar target/hbms-alerts-1.0.0.jar --server --host 0.0.0.0 --port 5055 --inbox ../integration/inbox/alerts.jsonl
```

From another ward workstation:

```bash
java -jar target/hbms-alerts-1.0.0.jar --send --host WORKSHOP_SERVER --port 5055 \
  --asset VENT-ICU-001 --ward ICU --severity CRITICAL \
  --description "Ventilator failed power-on self-test"
```

### Portal

Open `portal/index.html` in a browser. For browsers that restrict local-file frame/data access, serve the project root with a local static server and open `/portal/index.html`.

## Automated checks

From the project root:

```bash
./tests/run-portal-tests.sh
./tests/run-java-smoke.sh
```

Full Java unit tests and shaded executable build:

```bash
cd java && mvn clean test package
```

Windows desktop build:

```powershell
.\scripts\build-desktop.ps1
```

The current authoring environment is Linux and does not have .NET SDK, Maven or Access Database Engine installed. Therefore the Windows WinForms/Access build and full Maven build must be run on a correctly provisioned target machine. The runnable Node and dependency-free Java smoke checks are provided separately.

## Operational semantics

- Critical-care SLA response deadline: reported time + four hours unless an applicable asset/vendor SLA overrides the default according to the configured rule.
- Response/acknowledgement is not repair completion. Both timestamps are retained separately.
- Duplicate escalation records are prevented by checking existing escalation events for the same breakdown and level.
- Priority order is severity first (CRITICAL, HIGH, MEDIUM, LOW), then oldest report first within a severity.
- The Java receiver and Access application have separate persistence boundaries. The inbox importer is the explicit bridge; the TCP ACK is not an Access transaction acknowledgement.
- Uptime in the portal is an asset availability snapshot unless outage intervals are explicitly captured for a time-weighted period. Do not label a current snapshot as historical uptime.
- All supplied data is synthetic. Do not enter patient-identifiable data or rely on this prototype for clinical safety decisions.
