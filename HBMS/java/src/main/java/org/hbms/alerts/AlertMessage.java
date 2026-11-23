package org.hbms.alerts;

import java.time.OffsetDateTime;
import java.util.HashMap;
import java.util.Map;
import java.util.UUID;

public record AlertMessage(String messageId, String assetCode, String ward, String severity,
                           String reportedAt, String description) {
    public void validate() {
        try { UUID.fromString(messageId); } catch (Exception ex) { throw new IllegalArgumentException("messageId must be a UUID"); }
        require(assetCode, 40, "assetCode");
        require(ward, 80, "ward");
        require(description, 1000, "description");
        if (!severity.matches("CRITICAL|HIGH|MEDIUM|LOW")) throw new IllegalArgumentException("invalid severity");
        try { OffsetDateTime.parse(reportedAt); } catch (Exception ex) { throw new IllegalArgumentException("reportedAt must be ISO-8601 with offset"); }
    }

    private static void require(String value, int max, String field) {
        if (value == null || value.isBlank() || value.length() > max || value.contains("\n") || value.contains("\r"))
            throw new IllegalArgumentException("invalid " + field);
    }

    public String toJson() {
        return "{\"messageId\":\"" + escapeJson(messageId) + "\","
             + "\"assetCode\":\"" + escapeJson(assetCode) + "\","
             + "\"ward\":\"" + escapeJson(ward) + "\","
             + "\"severity\":\"" + escapeJson(severity) + "\","
             + "\"reportedAt\":\"" + escapeJson(reportedAt) + "\","
             + "\"description\":\"" + escapeJson(description) + "\"}";
    }

    public static AlertMessage fromJson(String json) {
        if (json == null || json.isBlank()) throw new IllegalArgumentException("empty json");
        Map<String, String> map = parseSimpleJsonObject(json);
        return new AlertMessage(
            map.get("messageId"),
            map.get("assetCode"),
            map.get("ward"),
            map.get("severity"),
            map.get("reportedAt"),
            map.get("description")
        );
    }

    private static Map<String, String> parseSimpleJsonObject(String json) {
        Map<String, String> map = new HashMap<>();
        String trimmed = json.trim();
        if (!trimmed.startsWith("{") || !trimmed.endsWith("}")) {
            throw new IllegalArgumentException("JSON must be enclosed in braces");
        }
        int i = 1;
        int len = trimmed.length() - 1;
        while (i < len) {
            while (i < len && (Character.isWhitespace(trimmed.charAt(i)) || trimmed.charAt(i) == ',')) i++;
            if (i >= len) break;
            if (trimmed.charAt(i) != '"') throw new IllegalArgumentException("Expected string key in JSON at index " + i);
            i++;
            StringBuilder key = new StringBuilder();
            while (i < len && trimmed.charAt(i) != '"') {
                if (trimmed.charAt(i) == '\\' && i + 1 < len) i++;
                key.append(trimmed.charAt(i++));
            }
            i++; // skip closing quote
            while (i < len && (Character.isWhitespace(trimmed.charAt(i)) || trimmed.charAt(i) == ':')) i++;
            if (i >= len) throw new IllegalArgumentException("Missing value for key: " + key);
            StringBuilder val = new StringBuilder();
            if (trimmed.charAt(i) == '"') {
                i++;
                while (i < len && trimmed.charAt(i) != '"') {
                    if (trimmed.charAt(i) == '\\' && i + 1 < len) {
                        i++;
                        char c = trimmed.charAt(i);
                        if (c == '"') val.append('"');
                        else if (c == '\\') val.append('\\');
                        else if (c == 'n') val.append('\n');
                        else if (c == 'r') val.append('\r');
                        else if (c == 't') val.append('\t');
                        else val.append(c);
                    } else {
                        val.append(trimmed.charAt(i));
                    }
                    i++;
                }
                i++; // skip closing quote
            } else {
                while (i < len && trimmed.charAt(i) != ',' && trimmed.charAt(i) != '}') {
                    val.append(trimmed.charAt(i++));
                }
            }
            map.put(key.toString(), val.toString());
        }
        return map;
    }

    public static String escapeJson(String s) {
        if (s == null) return "";
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < s.length(); i++) {
            char c = s.charAt(i);
            if (c == '"') sb.append("\\\"");
            else if (c == '\\') sb.append("\\\\");
            else if (c == '\b') sb.append("\\b");
            else if (c == '\f') sb.append("\\f");
            else if (c == '\n') sb.append("\\n");
            else if (c == '\r') sb.append("\\r");
            else if (c == '\t') sb.append("\\t");
            else sb.append(c);
        }
        return sb.toString();
    }
}
