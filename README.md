# Flow — Rocket League Companion

Flow is a lightweight, modern companion application for Rocket League. It reads data from the official Rocket League Stats API (WebSocket) and renders a transparent overlay with live session statistics.

> ⚠️ **Flow does NOT modify Rocket League in any way.** It only reads the official API.

## Architecture

| Project | Responsibility |
|---------|----------------|
| `Flow.App` | Avalonia UI entry point, views, and app lifecycle |
| `Flow.Core` | Domain models, interfaces, and core abstractions |
| `Flow.API` | Rocket League Stats API client and DTOs |
| `Flow.Overlay` | Overlay rendering and window management |
| `Flow.Input` | Global hotkey and input handling |
| `Flow.Storage` | Settings persistence and file I/O |
| `Flow.Widgets` | Widget definitions and management |
| `Flow.Shared` | Common utilities, converters, and helpers |

## Tech Stack

- **C# / .NET 8**
- **Avalonia UI** — cross-platform UI framework
- **CommunityToolkit.Mvvm** — MVVM source generators
- **Microsoft.Extensions.Hosting / DI**
- **Serilog** — structured logging
- **System.Text.Json** — JSON serialization

## Getting Started

1. Clone the repository.
2. Open `Flow.sln` in your IDE.
3. Build and run `Flow.App`.

## Project Status

This is the **foundation** phase. The solution structure, DI container, logging, settings system, and service placeholders are in place. Game-specific features will be added incrementally.

## License

MIT — see [LICENSE](LICENSE).