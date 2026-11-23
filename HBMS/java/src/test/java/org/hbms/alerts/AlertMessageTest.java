package org.hbms.alerts;

import static org.junit.jupiter.api.Assertions.*;
import org.junit.jupiter.api.Test;

class AlertMessageTest {
    private AlertMessage valid() {
        return new AlertMessage("123e4567-e89b-12d3-a456-426614174000", "VENT-ICU-001", "ICU",
                "CRITICAL", "2026-11-23T10:00:00+03:00", "Ventilator failed power-on self-test");
    }

    @Test void acceptsValidAlert() { assertDoesNotThrow(() -> valid().validate()); }
    @Test void rejectsNonUuidMessageId() {
        AlertMessage m = new AlertMessage("not-a-uuid", "VENT-ICU-001", "ICU", "CRITICAL",
                "2026-11-23T10:00:00+03:00", "Failure");
        assertThrows(IllegalArgumentException.class, m::validate);
    }
    @Test void rejectsUnknownSeverity() {
        AlertMessage m = new AlertMessage(valid().messageId(), "VENT-ICU-001", "ICU", "URGENT",
                valid().reportedAt(), "Failure");
        assertThrows(IllegalArgumentException.class, m::validate);
    }
    @Test void rejectsInvalidTimestamp() {
        AlertMessage m = new AlertMessage(valid().messageId(), "VENT-ICU-001", "ICU", "CRITICAL", "yesterday", "Failure");
        assertThrows(IllegalArgumentException.class, m::validate);
    }
    @Test void rejectsBlankAssetCode() {
        AlertMessage m = new AlertMessage(valid().messageId(), " ", "ICU", "CRITICAL", valid().reportedAt(), "Failure");
        assertThrows(IllegalArgumentException.class, m::validate);
    }
    @Test void rejectsOversizedDescription() {
        AlertMessage m = new AlertMessage(valid().messageId(), "VENT-ICU-001", "ICU", "CRITICAL", valid().reportedAt(), "x".repeat(1001));
        assertThrows(IllegalArgumentException.class, m::validate);
    }
}
