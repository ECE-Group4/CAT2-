# Architecture and Deployment

## Logical architecture

```text
Ward PCs / Engineering PCs / Admin PC
        |                         |
        | TCP alert protocol      | HTTPS/local OLEDB
        v                         v
Java Alert Receiver          VB.NET Desktop Client
        |                         |
        | durable JSONL inbox     | ADO.NET / OleDb
        v                         v
integration/inbox/          Microsoft Access (ACE)
alerts.jsonl                     |
                                +--> Assets, Wards, Vendors, SLAs
                                +--> Breakdowns, Jobs, Parts, Costs
                                +--> Escalations, Audit, Notifications

Executive Browser --> Static HTML Portal --> uptime snapshot JSON
```

## Three-tier deployment model
1. **Presentation tier:** ward reporting interface, VB desktop forms and HTML executive portal.
2. **Application tier:** VB domain services (triage, queue, SLA, maintenance, stock and reporting) and Java TCP alert receiver.
3. **Data tier:** Access database for structured operational records; append-only JSON Lines inbox for received network messages pending controlled import; JSON snapshot for portal reporting.

## Deployment zones
- Ward network: ward-side Java alert sender and authorised workstation.
- Biomedical engineering workshop: Java receiver, VB client and Access database on a secured workstation/share appropriate to a small prototype.
- Executive/admin network: read-only portal access to published uptime snapshot.

## Trust boundaries
- Java receiver validates packet shape, field lengths, timestamp and message ID before writing to the inbox.
- The VB importer validates the same fields and performs the authoritative database insert.
- The portal consumes a published aggregate snapshot; it does not expose database credentials or write access.
- Do not expose the TCP listener to public networks. Restrict it to the hospital LAN and firewall allow-listed ward clients.

## Availability and backup
Back up the Access database daily and before schema upgrades. Keep the alert inbox on persistent storage and rotate it only after imported messages are confirmed. Test restore procedures. Access is suitable for a small controlled deployment, not a multi-site high-concurrency production system.
