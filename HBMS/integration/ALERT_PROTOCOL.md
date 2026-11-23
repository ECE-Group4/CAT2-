# Ward Alert TCP Protocol v1

Transport: TCP, UTF-8, one JSON object per line, newline-delimited. Default port: `5055`.

## Request
```json
{"messageId":"550e8400-e29b-41d4-a716-446655440000","assetCode":"ASSET-VENT-001","ward":"ICU","severity":"CRITICAL","reportedAt":"2026-11-23T09:30:00+03:00","description":"Ventilator not powering on"}
```

Required fields: `messageId` (UUID), `assetCode` (1–40 chars), `ward` (1–80), `severity` (`CRITICAL|HIGH|MEDIUM|LOW`), `reportedAt` (ISO-8601 offset date-time), `description` (1–1000 chars). Newlines are not permitted inside field values. The receiver caps each frame at 8192 bytes.

## Success response
```json
{"messageId":"550e8400-e29b-41d4-a716-446655440000","status":"RECEIVED"}
```

## Error response
```json
{"status":"REJECTED","reason":"invalid severity"}
```

A `RECEIVED` acknowledgement means the message was validated and durably appended to the receiver inbox. It does not mean that a database breakdown record has been created or an engineer has responded.
