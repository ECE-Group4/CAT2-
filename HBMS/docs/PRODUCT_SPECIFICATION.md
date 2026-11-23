# HBMS Product Specification

## 1. Product purpose
HBMS coordinates biomedical equipment maintenance from first fault report through triage, response, repair, verification and closure. It gives engineering teams a prioritised operational queue, administrators visibility into vendor commitments and equipment availability, and ward staff a reliable channel for urgent fault notifications.

## 2. Operating roles
- **System administrator:** user accounts, roles, reference data and system configuration.
- **Ward reporter:** registers equipment faults and views submitted reports.
- **Biomedical engineer:** triages, accepts and works on assigned maintenance jobs; records diagnosis, work, parts and costs.
- **Biomedical supervisor:** manages priority queues, assignments, escalations and operational reports.
- **Procurement/store officer:** maintains spare-parts catalogue and stock transactions.
- **Hospital executive:** read-only access to uptime, downtime, workload and SLA performance.
- **Vendor contact:** represented as an SLA/vendor record; vendor portal access is outside this release.

## 3. Core capabilities
1. Asset register with ward, department, equipment class, criticality and lifecycle status.
2. Ward and department directory.
3. Vendor directory and effective-dated response/resolution SLA definitions.
4. Breakdown registration with severity, narrative, report time, response deadline and status history.
5. Priority queue ordered by severity (Critical, High, Medium, Low), then oldest report first.
6. Assignment and maintenance-job tracking with diagnosis, work performed, verification and timestamps.
7. Spare-parts inventory, usage transactions and stock decrement.
8. Repair-cost ledger with labour, parts, vendor and other cost categories.
9. Four-hour response escalation for critical-care equipment when no response has been recorded by the deadline.
10. Escalation audit trail and duplicate-escalation prevention.
11. Java TCP urgent-alert receiver for ward clients, with durable inbox and acknowledgement.
12. Controlled import of Java alerts into the authoritative Access database.
13. Executive portal showing department uptime and ward map navigation.
14. Audit records, validation, exception handling and operational reports.

## 4. Breakdown lifecycle
`Reported → Triaged → Assigned → Acknowledged/Responded → In Progress → Awaiting Parts (optional) → Repaired → Verified → Closed`

A report can also be marked `Cancelled` by an authorised supervisor with a reason. Response and repair completion are distinct events. Acknowledgement does not imply repair completion. A job cannot be closed without verification notes and a recorded completion time.

## 5. SLA semantics
- **Response target:** time from report registration to the first recorded response/acknowledgement.
- **Resolution target:** time from report registration to verified repair completion.
- **Critical-care four-hour rule:** a critical-care asset breakdown is escalated once the response deadline is exceeded and no response timestamp exists.
- The four-hour response rule is not a four-hour repair guarantee.
- Vendor-specific SLA values may override the default response/resolution targets when an applicable vendor SLA is assigned.
- All timestamps are stored as local workstation timestamps in this prototype. Production deployments should standardise on UTC and display local time zones explicitly.

## 6. Data and privacy boundaries
Use synthetic data only. Do not store patient names, patient identifiers, diagnoses or clinical records. Store only equipment, maintenance, staff-account and vendor operational data.

## 7. Non-functional design
- Referential integrity and uniqueness constraints for key identifiers.
- Role-based UI access and password hashing.
- Input validation at UI and service boundaries.
- Explicit database transactions for stock usage and cost recording.
- Durable alert inbox and duplicate-message detection.
- Clear error messages without leaking credentials or database details.
- Backups and restore procedures documented for deployment.
- Responsive static portal layout and accessible semantic labels.

## 8. Explicit release boundaries
This release is a single-site departmental prototype using Access as its data store. It does not provide a vendor self-service portal, patient data integration, medical-device telemetry, automated clinical decisions, mobile app, enterprise identity federation or high-availability database cluster.
