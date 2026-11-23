package org.hbms.alerts;

public final class AlertMessageSmokeTest {
    private static int checks;
    public static void main(String[] args) {
        AlertMessage valid = new AlertMessage("123e4567-e89b-12d3-a456-426614174000", "VENT-ICU-001", "ICU",
                "CRITICAL", "2026-11-23T10:00:00+03:00", "Ventilator failed power-on self-test");
        valid.validate(); check(true, "valid alert accepted");
        expectInvalid(new AlertMessage("bad", "VENT-ICU-001", "ICU", "CRITICAL", valid.reportedAt(), "Failure"), "invalid UUID rejected");
        expectInvalid(new AlertMessage(valid.messageId(), "VENT-ICU-001", "ICU", "URGENT", valid.reportedAt(), "Failure"), "unknown severity rejected");
        expectInvalid(new AlertMessage(valid.messageId(), " ", "ICU", "CRITICAL", valid.reportedAt(), "Failure"), "blank asset rejected");
        expectInvalid(new AlertMessage(valid.messageId(), "VENT-ICU-001", "ICU", "CRITICAL", "yesterday", "Failure"), "invalid timestamp rejected");
        expectInvalid(new AlertMessage(valid.messageId(), "VENT-ICU-001", "ICU", "CRITICAL", valid.reportedAt(), "x".repeat(1001)), "oversized description rejected");
        System.out.println("Java alert validation checks passed: " + checks);
    }
    private static void expectInvalid(AlertMessage m, String label) {
        try { m.validate(); throw new AssertionError(label); }
        catch (IllegalArgumentException expected) { check(true, label); }
    }
    private static void check(boolean ok, String label) { if (!ok) throw new AssertionError(label); checks++; }
}
