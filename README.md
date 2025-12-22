<h1 align="center"> TraDe (Trading Engine)</h1>

<div align="center">

[![Tests](https://github.com/OldManny/TraDe/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/OldManny/TraDe/actions/workflows/dotnet-ci.yml)
![.NET Version](https://img.shields.io/badge/.NET-8.0-blueviolet)

</div>

A high-performance, low-latency Limit Order Book (LOB) built with .NET 8, designed for thread-safe order matching and asynchronous persistence.

## Project Status: Phase 2
This project is currently under active development following a 6-phase architecture plan.

## Technical Architecture
- **Language:** C# (.NET 8)
- **Data Structures:** $O(\log n)$ Order Book matching using `SortedDictionary` and `LinkedList`.
- **Concurrency:** Producer-Consumer pattern via `System.Threading.Channels`.
- **Infrastructure:** Containerized PostgreSQL, managed via Terraform & Kubernetes.

## Roadmap
- [x] Phase 1: Solution setup & core domain
- [x] Phase 2: In-memory order book logic
- [ ] Phase 3: Concurrency layer (queue-based matching)
- [ ] Phase 4: Async persistence (PostgreSQL)
- [ ] Phase 5: API & market data simulation
- [ ] Phase 6: Containerized deployment & infrastructure (K8s/Terraform) 

## Getting Started

```bash
cp .env.example .env
docker-compose up
