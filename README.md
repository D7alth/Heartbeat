# Heartbeat

A lightweight IoT sensor digest built with .NET 8 Worker Services, MQTT, and InfluxDB.  
Listens to sensor events from ESP32 and Arduino devices, persists readings as time-series data, and exposes them to a Grafana dashboard.

```
[ESP32 / Arduino] → MQTT → Mosquitto → Heartbeat → InfluxDB → Grafana
```

---

## Stack

- **Runtime** — .NET 8 Worker Service
- **Messaging** — MQTT via Mosquitto
- **Storage** — InfluxDB 2.x
- **Dashboard** — Grafana

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker and Docker Compose

### Run

```bash
# Start infrastructure
docker compose up -d

# Set InfluxDB token
dotnet user-secrets set "Influx:Token" "your-token-here"

# Run
dotnet run
```

---

## Roadmap

- [ ] Kafka integration for event streaming and external notifications
- [ ] Device health panel (uptime, RSSI, firmware version)
- [ ] Alert rules for abnormal readings
- [ ] Support for additional sensor types

---

## License

MIT