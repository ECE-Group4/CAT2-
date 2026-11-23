# Operational Workflows

## Breakdown report
1. Reporter selects an asset and confirms its ward.
2. Reporter chooses severity and describes the fault.
3. System captures report time, derives the applicable response and resolution deadlines, creates a unique breakdown reference and records an audit event.
4. Critical/high reports enter the front of the priority queue; equal-priority items are FIFO.
5. Supervisor assigns an engineer/vendor as appropriate.

## Response and maintenance
1. Engineer records acknowledgement/response time; this stops response escalation.
2. Engineer starts work and records diagnosis.
3. Parts used are selected from stock; usage is transactional and cannot make stock negative.
4. Costs are recorded by category.
5. Engineer records repair completion and verification notes.
6. Authorised engineer/supervisor closes the breakdown after verification.

## SLA escalation
A scheduled check runs while the desktop client is open. It selects critical-care breakdowns whose response deadline has passed and whose response timestamp is null. An escalation is inserted only if an equivalent escalation does not already exist. The UI displays the alert and logs the notification attempt. For unattended operation, configure Windows Task Scheduler to launch the escalation-check command at a suitable interval; do not rely on an Access macro running while the database is closed.

## Network alert
1. Ward sender transmits a single UTF-8 JSON line over TCP.
2. Receiver validates required fields and bounds, writes the alert to `integration/inbox/alerts.jsonl`, flushes the file and returns an ACK.
3. Desktop client imports pending inbox records, checks message ID uniqueness and asset existence, creates the breakdown and records the imported message ID.
4. Failed imports remain visible for retry; duplicate imports are safely ignored.
