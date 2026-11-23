package org.hbms.alerts;

import java.io.*;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.time.OffsetDateTime;
import java.util.UUID;

public final class AlertSender {
    public static void send(String host, int port, String asset, String ward, String severity, String description) throws Exception {
        AlertMessage message = new AlertMessage(UUID.randomUUID().toString(), asset, ward, severity.toUpperCase(), OffsetDateTime.now().toString(), description);
        message.validate();
        try (Socket socket = new Socket()) {
            socket.connect(new InetSocketAddress(host, port), 5000);
            socket.setSoTimeout(10000);
            try (BufferedWriter out = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream(), StandardCharsets.UTF_8));
                 BufferedReader in = new BufferedReader(new InputStreamReader(socket.getInputStream(), StandardCharsets.UTF_8))) {
                out.write(message.toJson());
                out.newLine();
                out.flush();
                String response = in.readLine();
                if (response == null) throw new IOException("Receiver closed without acknowledgement");
                System.out.println(response);
                if (!response.contains("RECEIVED")) throw new IOException("Alert rejected by receiver: " + response);
            }
        }
    }
}
