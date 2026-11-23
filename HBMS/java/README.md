# HBMS Java alert service

Requires Java 17+ and Maven 3.9+.

Build: `mvn clean package`

Start workshop receiver:
`java -jar target/hbms-alerts-1.0.0.jar --server --host 0.0.0.0 --port 5055 --inbox ../integration/inbox/alerts.jsonl`

Send a synthetic ward alert:
`java -jar target/hbms-alerts-1.0.0.jar --send --host 127.0.0.1 --port 5055 --asset ASSET-VENT-001 --ward ICU --severity CRITICAL --description "Ventilator not powering on"`

The receiver validates and durably appends the message before returning `RECEIVED`. It does not create an Access row; import the inbox from the VB desktop client. Restrict the listener to a trusted network and firewall the port.
