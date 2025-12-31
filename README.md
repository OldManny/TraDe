<h1 align="center"> TraDe </h1>

<div align="center">

[![Tests](https://github.com/OldManny/TraDe/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/OldManny/TraDe/actions/workflows/dotnet-ci.yml)
![.NET Version](https://img.shields.io/badge/.NET-8.0-blueviolet)

</div>

A high-performance, low-latency Limit Order Book (LOB) built with .NET 8, designed for thread-safe order matching and asynchronous persistence.

## Performance Benchmarks
*Tested on Apple M1 Pro (16GB RAM)*

- **Throughput:** ~13.2 Million matches per second.
- **Latency (Mean):** ~75 nanoseconds per match.
- **Tail Latency (P95):** ~76 nanoseconds.
- **Memory Efficiency:** ~94 bytes per order.

These numbers demonstrate the efficiency of the **Single-Threaded Actor Pattern** and $O(\log n)$ data structures, minimizing Garbage Collector (GC) pressure and eliminating the need for heavy locking mechanisms.

## Technical Architecture
- **Core Engine:** Single-threaded execution model via `System.Threading.Channels` (Actor Pattern).
- **Matching Logic:** Price-Time Priority (FIFO) using `SortedDictionary` and `LinkedList`.
- **Persistence:** Asynchronous batching to PostgreSQL via EF Core, decoupled from the hot path.
- **Orchestration:** Fully automated via **Terraform** and **Kubernetes**.

## Roadmap
- [x] Phase 1: Solution setup & core domain
- [x] Phase 2: In-memory order book logic (Price-Time Priority)
- [x] Phase 3: Concurrency layer (High-speed Channels)
- [x] Phase 4: Async persistence (PostgreSQL)
- [x] Phase 5: API & market data simulation
- [x] Phase 6: Infrastructure Orchestration (K8s/Terraform)
- [ ] **Next:** Real-time Visualizations (SignalR & Grafana)

## Getting Started

### Prerequisites
- Docker Desktop with **Kubernetes** enabled.
- Terraform.

### Deployment
```bash
# 1. Prepare Environment
cp .env.example .env

# 2. Build the Image
docker build -t trade-engine:v1 -f TraDe.Server/Dockerfile .

# 3. Deploy via Terraform
cd infra
terraform init && terraform apply -auto-approve
