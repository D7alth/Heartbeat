# Design: Conexão Kafka Persistente e Não Bloqueante

## Contexto e Problema Atual

No código atual (`KafkaPublisher`), **a cada chamada de `PublishEventAsync`** são feitas três operações custosas:

1. `GetProducerConfig()` — recria o objeto de configuração do zero.
2. `new ProducerBuilder<string, T>(config).Build()` — **abre uma nova conexão TCP com o broker Kafka**.
3. O producer criado **nunca é descartado** corretamente (`IDisposable` ignorado).

Além disso, o `IPublisher` está registrado como **Scoped**, o que é inconsistente para uma conexão que deveria ser compartilhada e de longa duração.

---

## Objetivo do Novo Design

| Princípio | Descrição |
|---|---|
| **Singleton real** | Um único `IProducer<K,V>` para toda a vida da aplicação |
| **Startup não bloqueante** | A aplicação sobe mesmo que o Kafka esteja offline |
| **Resiliência** | Reconexão automática com backoff exponencial em background |
| **Fail-safe no publish** | Publisher não lança exceção nem derruba o worker quando o Kafka está indisponível |
| **Observabilidade** | Estado da conexão acessível via logs e health-check |

---

## Arquitetura em Camadas

```
┌─────────────────────────────────────────────────────┐
│                    ServiceWorker                    │  ← Worker existente, sem mudança
│              (BackgroundService loop)               │
└────────────────────┬────────────────────────────────┘
                     │ usa
                     ▼
┌─────────────────────────────────────────────────────┐
│                 IPublisher (Facade)                 │  ← Singleton
│            KafkaPublisher (refatorado)              │
│   • Verifica disponibilidade antes de publicar      │
│   • Retorna PublishResult (sucesso/indisponível)    │
└────────────────────┬────────────────────────────────┘
                     │ depende de
                     ▼
┌─────────────────────────────────────────────────────┐
│            IKafkaConnectionManager                  │  ← Singleton
│          KafkaConnectionManager                     │
│   • Detém o IProducer<string, T> singleton          │
│   • Expõe estado: Starting | Ready | Degraded       │
│   • Thread-safe                                     │
└────────────────────┬────────────────────────────────┘
                     │ estado atualizado por
                     ▼
┌─────────────────────────────────────────────────────┐
│         KafkaConnectionInitializer                  │  ← IHostedService
│   • Executa tentativa de conexão em background      │
│   • Retry com backoff exponencial + jitter          │
│   • Não lança exceção para o host                   │
│   • Após conectar: loop de health-check periódico   │
└─────────────────────────────────────────────────────┘
```

---

## Responsabilidade de Cada Componente

### `IKafkaConnectionManager` + `KafkaConnectionManager`
**Lifecycle:** Singleton  
**Responsabilidade única:** guardar e fornecer o `IProducer` pronto para uso.

- Expõe a propriedade `ConnectionState` (enum: `Starting`, `Ready`, `Degraded`).
- Expõe o `IProducer` somente quando o estado é `Ready`.
- Thread-safe: uso de `volatile` ou `Interlocked` no estado, sem lock na leitura do producer.
- **Não tenta conectar** — essa responsabilidade é do `KafkaConnectionInitializer`.

### `KafkaConnectionInitializer`
**Lifecycle:** `IHostedService` (registrado no DI)  
**Responsabilidade única:** criar e validar a conexão em background.

Fluxo interno:
```
StartAsync() → dispara Task em background → NÃO bloqueia o host
                    │
                    ▼
              Tentativa de conexão
                    │
            ┌───────┴────────┐
          Sucesso           Falha
            │                 │
     ConnectionManager     espera com backoff exponencial
     → Ready               (ex: 2s, 4s, 8s... até máx 60s)
                               │
                          nova tentativa
```

- `StartAsync` retorna imediatamente, a lógica fica em uma `Task` em background.
- Em caso de falha, usa **backoff exponencial com jitter** para não bombardear o broker.
- Após `Ready`, executa um **health-check periódico** simples (ex: verificar metadata do topic).
- Se o health-check falhar, transita para `Degraded` e inicia nova sequência de reconexão.

### `KafkaPublisher` (refatorado)
**Lifecycle:** Singleton  
**Responsabilidade única:** publicar, usando o producer gerenciado pelo `ConnectionManager`.

Fluxo de publicação:
```
PublishEventAsync()
        │
        ▼
ConnectionManager.State == Ready?
        │
    ┌───┴───┐
   Sim     Não
    │        │
 Publica   Retorna PublishResult.Unavailable
    │       (log de warning, sem exceção)
    ▼
Captura KafkaException
    │
    ▼
Notifica ConnectionManager → Degraded
Retorna PublishResult.Failed
```

**Ponto importante:** o `ServiceWorker` deve tratar `PublishResult.Unavailable` como **não-fatal** (continuar o loop), e não como exceção. O `throw` atual no worker precisa ser removido ou isolado.

---

## Máquina de Estados da Conexão

```
         ┌─────────┐
  início │ Starting│
         └────┬────┘
              │ conexão OK
              ▼
         ┌─────────┐       health-check falha      ┌───────────┐
         │  Ready  │ ──────────────────────────────► Degraded  │
         └────▲────┘                               └─────┬─────┘
              │                                          │
              └──────────────── reconexão OK ────────────┘
```

- **Starting → Ready:** `KafkaConnectionInitializer` estabelece conexão com sucesso.
- **Ready → Degraded:** health-check periódico detecta falha ou `KafkaException` no publish.
- **Degraded → Ready:** nova tentativa de reconexão bem-sucedida no background.

---

## Configuração Recomendada (`appsettings.json`)

```json
"PublisherOptions": {
  "BootstrapServer": "localhost:9092",
  "TopicName": "heartbeat.events",
  "ClientId": "heartbeat-producer",
  "SecurityProtocol": "PLAINTEXT",
  "Acks": "All",
  "MessageTimeout": 5000,
  "Retry": {
    "InitialDelaySeconds": 2,
    "MaxDelaySeconds": 60,
    "JitterSeconds": 1
  },
  "HealthCheck": {
    "IntervalSeconds": 30
  }
}
```

---

## Estrutura de Diretórios Sugerida

O projeto já adota a convenção `Core/` para contratos e modelos de domínio e `Infrastructure/` para implementações técnicas. O novo design segue exatamente essa separação.

```
Heartbeat.Producer/
│
├── Core/
│   ├── Contracts/
│   │   ├── IMetricCollector.cs          ← já existe
│   │   └── IPublisher.cs                ← mover de Infrastructure/Messaging
│   │
│   └── Models/
│       ├── Events/                      ← já existe
│       ├── Metrics/                     ← já existe
│       └── Messaging/
│           ├── PublishResult.cs         ← novo (enum: Success, Failed, Unavailable)
│           └── ConnectionState.cs       ← novo (enum: Starting, Ready, Degraded)
│
└── Infrastructure/
    ├── Messaging/
    │   ├── Kafka/
    │   │   ├── IKafkaConnectionManager.cs   ← novo
    │   │   ├── KafkaConnectionManager.cs    ← novo
    │   │   ├── KafkaConnectionInitializer.cs← novo (IHostedService)
    │   │   ├── KafkaPublisher.cs            ← refatorado
    │   │   └── PublisherOptions.cs          ← já existe, mover para cá
    │   └── (outros brokers futuros, ex: RabbitMq/)
    │
    ├── Services/
    │   └── ServiceWorker.cs             ← já existe, ajuste leve
    │
    └── Stubs/
        └── ...                          ← já existe
```

### Justificativas das decisões

| Decisão | Motivo |
|---|---|
| `IPublisher` vai para `Core/Contracts` | É um contrato de domínio, não deve depender de Kafka. `Core` não referencia `Infrastructure`. |
| `PublishResult` e `ConnectionState` em `Core/Models/Messaging` | São tipos compartilhados entre camadas. Ficam no domínio, não na infra. |
| Subpasta `Kafka/` dentro de `Messaging/` | Isola a implementação Kafka. Se amanhã vier RabbitMQ, cria-se `RabbitMq/` sem tocar no resto. |
| `KafkaConnectionInitializer` dentro de `Kafka/` | Faz parte do ciclo de vida da conexão Kafka, não é um serviço de domínio. |
| `PublisherOptions` permanece em `Kafka/` | É uma opção técnica específica do Kafka, não um contrato de domínio. |

---

## Registro de Dependências (`Program.cs`)

A ordem de registro no DI deve respeitar a hierarquia:

```
AddSingleton<IKafkaConnectionManager, KafkaConnectionManager>()
AddSingleton<IPublisher, KafkaPublisher>()
AddHostedService<KafkaConnectionInitializer>()
```

O `KafkaConnectionInitializer` recebe o `IKafkaConnectionManager` por injeção e atualiza seu estado — sem dependência circular.

---

## Tratamento de Falhas no `ServiceWorker`

O worker atual tem um `throw` dentro do catch, o que **derruba o `BackgroundService`** em caso de falha de publicação. Com o novo design:

- O `Publisher` absorve a falha de Kafka e retorna um resultado tipado.
- O worker loga o aviso e **continua o loop** — nunca lança exceção por falha de publicação.
- Apenas erros de coleta de métricas (domínio) devem propagar exceção.

---

## Observabilidade

| Evento | Nível de Log |
|---|---|
| Tentativa de conexão inicial | `Information` |
| Conexão estabelecida (`Starting → Ready`) | `Information` |
| Falha na tentativa (com próximo retry) | `Warning` |
| Publicação descartada (`Unavailable`) | `Warning` |
| Exceção de publicação (`KafkaException`) | `Error` |
| Degraded → início de reconexão | `Warning` |
| Reconexão bem-sucedida (`Degraded → Ready`) | `Information` |

Health-check endpoint (se aplicável): expor `ConnectionManager.State` como `Healthy/Degraded/Unhealthy`.

---

## Checklist de Implementação

- [ ] Criar `IKafkaConnectionManager` com `ConnectionState` e acesso ao `IProducer`
- [ ] Implementar `KafkaConnectionManager` como singleton thread-safe
- [ ] Criar `KafkaConnectionInitializer` como `IHostedService` com backoff exponencial
- [ ] Refatorar `KafkaPublisher` para usar `IKafkaConnectionManager` e retornar resultado tipado
- [ ] Ajustar `IPublisher` para retornar `PublishResult` ao invés de `bool`
- [ ] Corrigir registro do DI: todos Singleton
- [ ] Remover `throw` no `ServiceWorker`, tratar `Unavailable` como aviso
- [ ] Adicionar `PublisherOptions.Retry` e `PublisherOptions.HealthCheck` na configuração
