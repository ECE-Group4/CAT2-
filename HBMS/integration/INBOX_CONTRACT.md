# Alert Inbox Contract

The receiver appends one validated JSON object per line to `integration/inbox/alerts.jsonl`. It does not modify prior lines. The desktop importer tracks imported `messageId` values in `ImportedAlerts` and creates a breakdown only once. The importer must retain failed records and display their error so an operator can retry after correcting asset/reference data. Archive the inbox only after all lines are imported or explicitly dispositioned.
