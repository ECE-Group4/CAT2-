package org.hbms.alerts;

import java.io.*;
import java.net.*;
import java.nio.charset.StandardCharsets;
import java.nio.file.*;
import java.util.concurrent.*;

public final class AlertReceiver implements AutoCloseable {
    private static final int MAX_FRAME = 8192;
    private final String host;
    private final int port;
    private final Path inbox;
    private static final Object INBOX_LOCK = new Object();
    private final ExecutorService clients = Executors.newFixedThreadPool(8);
    private volatile boolean running;
    private ServerSocket server;

    public AlertReceiver(String host, int port, Path inbox) {
        this.host = host;
        this.port = port;
        this.inbox = inbox;
    }

    public void start() throws IOException {
        Path parent = inbox.toAbsolutePath().getParent();
        if (parent != null) Files.createDirectories(parent);
        server = new ServerSocket();
        server.bind(new InetSocketAddress(host, port));
        running = true;
        System.out.printf("HBMS alert receiver listening on %s:%d; inbox=%s%n", host, port, inbox.toAbsolutePath());
        while (running) {
            try {
                Socket socket = server.accept();
                socket.setSoTimeout(10000);
                clients.submit(() -> handle(socket));
            } catch (SocketException ex) {
                if (running) throw ex;
            }
        }
    }

    private void handle(Socket socket) {
        try (socket;
             BufferedInputStream input = new BufferedInputStream(socket.getInputStream());
             BufferedWriter output = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream(), StandardCharsets.UTF_8))) {
            byte[] frame = readFrame(input);
            if (frame == null) return;
            try {
                String frameStr = new String(frame, StandardCharsets.UTF_8);
                AlertMessage message = AlertMessage.fromJson(frameStr);
                message.validate();

                // Append one complete line and force it to stable storage before acknowledging receipt.
                byte[] line = (message.toJson() + System.lineSeparator()).getBytes(StandardCharsets.UTF_8);
                synchronized (INBOX_LOCK) {
                    try (FileChannelAppend append = new FileChannelAppend(inbox)) {
                        append.write(line);
                    }
                }
                String ack = "{\"messageId\":\"" + AlertMessage.escapeJson(message.messageId()) + "\",\"status\":\"RECEIVED\"}";
                output.write(ack);
                output.newLine();
                output.flush();
            } catch (Exception ex) {
                String rejected = "{\"status\":\"REJECTED\",\"reason\":\"" + AlertMessage.escapeJson(safe(ex.getMessage())) + "\"}";
                output.write(rejected);
                output.newLine();
                output.flush();
            }
        } catch (IOException ex) {
            System.err.println("Client connection ended: " + ex.getMessage());
        }
    }

    private static byte[] readFrame(InputStream in) throws IOException {
        ByteArrayOutputStream out = new ByteArrayOutputStream();
        int b;
        while ((b = in.read()) != -1) {
            if (b == '\n') break;
            if (out.size() >= MAX_FRAME) throw new IOException("frame exceeds 8192 bytes");
            if (b != '\r') out.write(b);
        }
        if (out.size() == 0 && b == -1) return null;
        return out.toByteArray();
    }

    private static String safe(String s) {
        return s == null ? "invalid request" : s.replace('\n', ' ').replace('\r', ' ');
    }

    @Override
    public void close() throws IOException {
        running = false;
        if (server != null) server.close();
        clients.shutdown();
    }

    private static final class FileChannelAppend implements AutoCloseable {
        private final java.nio.channels.FileChannel channel;
        FileChannelAppend(Path path) throws IOException {
            channel = java.nio.channels.FileChannel.open(path, StandardOpenOption.CREATE, StandardOpenOption.WRITE, StandardOpenOption.APPEND);
        }
        void write(byte[] data) throws IOException {
            java.nio.ByteBuffer buffer = java.nio.ByteBuffer.wrap(data);
            while (buffer.hasRemaining()) channel.write(buffer);
            channel.force(true);
        }
        public void close() throws IOException { channel.close(); }
    }
}
