package org.hbms.alerts;

import java.nio.file.Path;
import java.util.*;

public final class AlertApplication {
    public static void main(String[] args) throws Exception {
        Map<String,String> options = parse(args);
        if (options.containsKey("server")) {
            String host = options.getOrDefault("host", "0.0.0.0");
            int port = Integer.parseInt(options.getOrDefault("port", "5055"));
            Path inbox = Path.of(options.getOrDefault("inbox", "../integration/inbox/alerts.jsonl"));
            new AlertReceiver(host, port, inbox).start();
        } else if (options.containsKey("send")) {
            AlertSender.send(required(options,"host"), Integer.parseInt(options.getOrDefault("port","5055")),
                    required(options,"asset"), required(options,"ward"), required(options,"severity"), required(options,"description"));
        } else {
            System.out.println("HBMS Alert Service\nServer: --server [--host 0.0.0.0] [--port 5055] [--inbox path]\nSender: --send --host <workshop> --asset <code> --ward <ward> --severity <level> --description <text>");
        }
    }
    private static String required(Map<String,String> o, String key) { String v=o.get(key); if(v==null||v.isBlank()) throw new IllegalArgumentException("Missing --"+key); return v; }
    private static Map<String,String> parse(String[] args) {
        Map<String,String> map=new HashMap<>();
        for(int i=0;i<args.length;i++) { String a=args[i]; if(!a.startsWith("--")) continue; String k=a.substring(2); if(k.equals("server")||k.equals("send")) map.put(k,"true"); else { if(i+1>=args.length) throw new IllegalArgumentException("Missing value for "+a); map.put(k,args[++i]); } }
        return map;
    }
}
