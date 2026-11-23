# System Design Package

## Context view

```mermaid
flowchart LR
  Ward[Ward staff / ward PC] -->|Breakdown report or urgent alert| HBMS[HBMS]
  Eng[Biomedical engineers] <-->|Assignments, response, repairs| HBMS
  Vendor[External biomedical vendor] <-->|SLA and service response records| HBMS
  Admin[Hospital administration] <-->|Reference data and reports| HBMS
  Exec[Executive users] -->|Read-only KPI request| Portal[Executive portal]
  HBMS -->|Published aggregate snapshot| Portal
  HBMS <-->|Assets, jobs, costs, inventory, audit| DB[(Access database)]
  Ward -->|TCP JSON alert| Java[Java alert receiver]
  Java -->|Durable JSONL inbox| HBMS
```

## Level-1 data-flow view

```mermaid
flowchart TB
  Ward[Ward reporter] --> P1((1.0 Register and triage breakdown))
  P1 <--> D1[(D1 Asset and ward register)]
  P1 --> D2[(D2 Breakdown records)]
  P1 --> P2((2.0 Prioritise and assign work))
  P2 <--> D3[(D3 Users and engineer records)]
  P2 --> D4[(D4 Maintenance jobs)]
  P2 --> Eng[Biomedical engineer]
  Eng --> P3((3.0 Perform maintenance))
  P3 <--> D5[(D5 Parts and stock)]
  P3 --> D6[(D6 Repair costs)]
  P3 --> D4
  P3 --> D2
  P4((4.0 Monitor SLA and escalate)) <--> D2
  P4 <--> D7[(D7 SLA and escalation logs)]
  P4 --> Sup[Biomedical supervisor]
  P5((5.0 Publish executive metrics)) --> D8[(D8 Aggregate snapshot)]
  D1 --> P5
  D2 --> P5
  D4 --> P5
  D7 --> P5
  D8 --> Exec[Executive portal]
  Ward --> Java[Java TCP alert receiver]
  Java --> Inbox[(Durable alert inbox)]
  Inbox --> P1
```

## Deployment view

```mermaid
flowchart TB
 subgraph Ward_Network[Ward network]
  WardPC[Ward workstation]
  Sender[Java alert sender]
  WardPC --> Sender
 end
 subgraph Workshop_Network[Biomedical workshop]
  Receiver[Java TCP receiver :5055]
  Inbox[(JSONL alert inbox)]
  Desktop[VB.NET WinForms client]
  Access[(Microsoft Access / ACE database)]
  Receiver --> Inbox
  Desktop --> Inbox
  Desktop <--> Access
 end
 subgraph Executive_Network[Executive/admin network]
  Browser[Browser]
  Portal[Static HTML portal]
  Snapshot[(Published uptime JSON)]
  Browser --> Portal --> Snapshot
 end
 Sender -->|TCP / JSON line| Receiver
 Desktop -->|Publish aggregate snapshot| Snapshot
```

## Entity relationship view

```mermaid
erDiagram
 WARDS ||--o{ ASSETS : contains
 VENDORS ||--o{ ASSETS : services
 VENDORS ||--o{ VENDOR_SLAS : defines
 ASSETS ||--o{ BREAKDOWNS : experiences
 USERS ||--o{ BREAKDOWNS : reports
 USERS ||--o{ BREAKDOWNS : responds_to
 BREAKDOWNS ||--o{ MAINTENANCE_JOBS : creates
 USERS ||--o{ MAINTENANCE_JOBS : assigned_to
 MAINTENANCE_JOBS ||--o{ JOB_SPARE_PARTS : consumes
 SPARE_PARTS ||--o{ JOB_SPARE_PARTS : used_as
 MAINTENANCE_JOBS ||--o{ REPAIR_COSTS : incurs
 BREAKDOWNS ||--o{ ESCALATION_LOGS : triggers
 BREAKDOWNS ||--o{ NOTIFICATION_LOGS : notifies
 BREAKDOWNS ||--o{ IMPORTED_ALERTS : originates_from
```

## Use-case catalogue
| ID | Actor | Use case | Outcome |
|---|---|---|---|
| UC-01 | Ward reporter | Register breakdown | Unique report and response deadline |
| UC-02 | Supervisor | Review priority queue | Critical/highest-priority work visible first |
| UC-03 | Supervisor | Assign engineer | Maintenance job linked to breakdown |
| UC-04 | Engineer | Record response | Response timestamp captured; response SLA stops |
| UC-05 | Engineer | Record diagnosis and repair | Job history updated |
| UC-06 | Engineer/store officer | Record part usage | Stock decremented transactionally |
| UC-07 | Engineer | Record repair costs | Cost ledger updated |
| UC-08 | Engineer/supervisor | Verify and close | Repair verified and breakdown closed |
| UC-09 | Supervisor | Review/escalate overdue response | Unique escalation audit record |
| UC-10 | Ward workstation | Send urgent alert | Durable receiver acknowledgement |
| UC-11 | Supervisor | Import network alerts | Message converted to breakdown once |
| UC-12 | Executive | View department availability | Aggregate KPI snapshot displayed |
| UC-13 | Administrator | Manage reference data/accounts | Controlled system configuration |

## Key business rules
1. Every breakdown belongs to one registered asset.
2. Asset location is derived from the asset's current ward; report history remains attached to the breakdown.
3. Critical-care response target is at most four hours unless an approved effective vendor SLA has a stricter target.
4. Response timestamp and repair completion timestamp are independent.
5. Queue ordering is severity descending, then report timestamp ascending.
6. A stock issue is rejected if requested quantity exceeds available stock.
7. Part usage and stock decrement occur in one transaction.
8. A breakdown cannot be closed without verification notes.
9. Escalation uniqueness is per breakdown and escalation level.
10. A received TCP alert is not considered a database breakdown until imported successfully.
11. Imported alert message IDs are unique.
12. Portal data is an aggregate snapshot; it is not a transactional interface.
