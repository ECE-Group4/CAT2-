# User Guide

## Sign in
Launch HBMS Desktop and sign in with your assigned username and password. The bootstrap account is for first-run demonstration only; change it before shared use.

## Register a breakdown
Open Breakdown Queue → Log breakdown. Select the asset, choose severity, enter a concise fault description and submit. Record the generated breakdown reference for follow-up.

## Respond and assign
Select a breakdown in the queue. Use Record response when the engineering team has acknowledged the fault. Use Assign engineer to create a maintenance job for an active engineer/supervisor/admin account.

## Record parts and costs
Open Spare Parts → Record part usage. Enter the maintenance job ID, select the part and quantity. HBMS checks stock and decrements it transactionally. Use Record repair cost to add labour, parts, vendor or other expenditure against a job.

## Import urgent ward alerts
Start the Java receiver on the workshop workstation. Ward PCs send alerts to port 5055. In the desktop client choose Import ward alerts. Unknown assets and malformed records are reported and remain in the inbox for correction/retry.

## Executive portal
Open `portal/index.html`. Use the left navigation to view the overview, department availability, ward map, equipment coverage, SLA concepts and maintenance workflow. The bundled figures are synthetic examples; publish an approved data snapshot before using it for a real management briefing.
