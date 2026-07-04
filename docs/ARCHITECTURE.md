# Flow Architecture

## Overview

Flow follows a modular, layered architecture with clear separation of concerns.

## Layers

- **Flow.Core** — Domain models, interfaces, and abstractions. No external dependencies.
- **Flow.Storage** — Persistence layer. Depends on Flow.Core.
- **Flow.API** — External API integration (Rocket League Stats API). Depends on Flow.Core.
- **Flow.Overlay** — Overlay window management. Depends on Flow.Core.
- **Flow.Input** — Global input handling. Depends on Flow.Core.
- **Flow.Widgets** — Widget definitions and management. Depends on Flow.Core.
- **Flow.Shared** — Common utilities and converters. Minimal dependencies.
- **Flow.App** — Avalonia UI, views, view models, and composition root. Depends on all other projects.

## Dependency Direction

```
Flow.App
  -> Flow.Core
  -> Flow.Storage
  -> Flow.API
  -> Flow.Overlay
  -> Flow.Input
  -> Flow.Widgets
  -> Flow.Shared
```

Flow.Core has zero project references and defines all contracts.
