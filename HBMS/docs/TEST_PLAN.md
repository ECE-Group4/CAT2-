# Verification Plan

This is an implementation verification checklist, not a grading or evaluation report.

| ID | Scenario | Expected behaviour |
|---|---|---|
| T-01 | Login with valid account | User enters operations console |
| T-02 | Login with wrong password | Access denied without exposing stored hash |
| T-03 | Register fault with missing description | Validation prevents submission |
| T-04 | Register valid breakdown | Unique reference and deadlines created |
| T-05 | Critical-care report | Response deadline no later than four hours |
| T-06 | Record response before deadline | Response timestamp stored; escalation not created |
| T-07 | Critical report exceeds four hours without response | One escalation record created |
| T-08 | Run escalation check repeatedly | No duplicate escalation for same breakdown/level |
| T-09 | Queue contains mixed severities | Critical before High, Medium, Low; FIFO within level |
| T-10 | Assign engineer | Maintenance job linked to breakdown |
| T-11 | Consume available stock | Stock decreases and usage transaction appears |
| T-12 | Consume more than available stock | Transaction rejected; stock unchanged |
| T-13 | Record repair cost | Amount and category persist |
| T-14 | Import valid ward alert | One breakdown and ImportedAlerts record created |
| T-15 | Import same message twice | Duplicate ignored |
| T-16 | Import unknown asset alert | Import fails visibly; original inbox line retained |
| T-17 | Java sender to receiver | Receiver persists JSONL record before ACK |
| T-18 | Malformed Java packet | REJECTED response; no inbox append |
| T-19 | Portal loads JSON snapshot | Department table and summary metrics render |
| T-20 | Portal opened without fetch support | Bundled fallback sample remains available |

## Manual test evidence to capture during Windows deployment
- Database file creation and tables/relationships in Access.
- Login and main dashboard.
- Breakdown registration and priority queue.
- Response recording and escalation event.
- Engineer assignment, part usage and cost entry.
- Java sender and receiver console showing RECEIVED.
- Imported alert visible as a breakdown.
- Executive portal overview, uptime table and ward map.
