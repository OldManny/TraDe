<h1 align="center"> TraDe (Trading Engine)</h1>

<div align="center">

[![Tests](https://github.com/OldManny/TraDe/actions/workflows/dotnet-ci.yml/badge.svg)](https://github.com/OldManny/TraDe/actions/workflows/dotnet-ci.yml)

</div>

TraDe is a trading engine simulation built in C#/.NET, focused on deterministic order matching, concurrency safety, and data integrity.

The project models a simplified execution layer rather than trading strategies or pricing models.

## Tech Stack
- C# (.NET 8)
- PostgreSQL
- Docker / Docker Compose

## Project Status
**Phase 1 – Core Domain (In Progress)**

Current focus:
- Domain modelling
- In-memory order book
- FIFO order matching logic

## Roadmap
The following phases describe the intended evolution of the project.

- [x] Phase 1: Solution setup & core domain
- [ ] Phase 2: In-memory order book logic
- [ ] Phase 3: Concurrency layer (queue-based matching)
- [ ] Phase 4: Async persistence (PostgreSQL)
- [ ] Phase 5: API & market data simulation
- [ ] Phase 6: Containerized deployment & infrastructure (K8s/Terraform) 

## Getting Started

```bash
cp .env.example .env
docker-compose up
