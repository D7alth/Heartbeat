# Heartbeat - Sistema de Monitoramento de Métricas

## 📋 Sobre o Projeto

O **Heartbeat** é um projeto de demonstração de um sistema de monitoramento de métricas de servidores, utilizando uma arquitetura baseada em eventos com Apache Kafka. 
O objetivo é coletar métricas como uso de CPU, espaço em disco e tráfego de rede, processá-las e tomar decisões baseadas nessas informações.


## Arquitetura

```
┌─────────────────────┐
│ Heartbeat.Producer  │  ──►  Coleta métricas do servidor
└──────────┬──────────┘
           │
           │ (publica evento)
           ▼
      ┌─────────┐
      │  Kafka  │
      │  Topic  │  ──►  Tópico de métricas brutas
      └────┬────┘
     Topic: Analyzer
           │
           ▼
    ┌──────────────┐
    │  Heartbeat   │
    │  .Analyzer   │  ──►  Analisa e decide o destino
    └──────┬───────┘
           │
           │ (republica evento)
           ▼
      ┌─────────┐
      │  Kafka  │  ──►  Tópicos específicos
      └────┬────┘
           │
           ├─────────────────────┐
           ▼                     ▼
    ┌──────────────┐     ┌──────────────┐
    │  Heartbeat   │     │  Heartbeat   │
    │  .Alerting   │     │  .Storage    │
    └──────────────┘     └──────────────┘
    Topic: alerts        Topic: storage
    Envia alertas        Persiste no banco
```

### Fluxo de Dados (Orquestração)

1. **Heartbeat.Producer**: Coleta métricas (CPU, disco, rede) do servidor e publica no Kafka
2. **Kafka (Tópico de Entrada)**: Recebe os eventos brutos de métricas
3. **Heartbeat.Analyzer**: Consome do tópico, analisa as métricas e decide o destino:
   - Se métricas estão **críticas** → republica no tópico de alertas
   - Se métricas estão **normais** → republica no tópico de storage
4. **Kafka (Tópicos de Saída)**: Distribui os eventos para os tópicos específicos
5. **Heartbeat.Alerting**: Escuta o tópico de alertas e dispara notificações
6. **Heartbeat.Storage**: Escuta o tópico de storage e persiste os snapshots no banco de dados

## 🚀 Heartbeat.Producer

Este repositório contém o **Producer**, uma aplicação console .NET que executa como background service.

### Funcionalidades (TODO)

- Coleta automática de métricas de servidor
- Publicação de eventos no Apache Kafka
- Execução contínua como serviço de background
- Suporte a containerização com Docker

### Métricas Coletadas

- **CPU**: Uso de processador
- **Disco**: Espaço utilizado e disponível
- **Rede**: Tráfego de entrada/saída

> **Nota**: A implementação atual utiliza dados mockados para testes. A coleta de métricas reais do servidor será implementada em futuras versões.

## 🛠️ Tecnologias

### Heartbeat.Producer
- **.NET 10.0** - Framework principal
- **C#** - Linguagem de programação
- **Confluent.Kafka** - Cliente Kafka para .NET
- **Microsoft.Extensions.Hosting** - Background Services
- **Docker** - Containerização

## 📦 Instalação e Execução

### Pré-requisitos

- .NET 10.0 SDK
- Docker e Docker Compose

### Executar Localmente

```bash
docker compose -f compose.yaml up -d
```

