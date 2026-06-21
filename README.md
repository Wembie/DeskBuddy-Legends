# DeskBuddy Legends

> A desktop companion that lives on your screen and grows while you work, study, code, or play.

**Genre:** Desktop Companion · Idle · Collection · Achievement Hunter · Life Simulation  
**Engine:** Godot 4 (C#, .NET 8) · **Target:** Steam — Windows · Linux · SteamOS · Steam Deck  
**Architecture:** Clean Architecture (Domain → Application → Infrastructure → Godot)

---

## Vision

DeskBuddy Legends is a virtual companion RPG that evolves alongside the user's real computer activity. While you work, your companion levels up, unlocks new forms, and collects rare items — with zero interruption to your workflow.

**Core pillars:**
- Constant, genuine-feeling progression
- Deep collection system (companions, skins, rarities, effects)
- 500+ achievements, data-driven and expandable without code changes
- Extremely lightweight: **< 150 MB RAM · < 1% CPU average**

---

## Architecture

```
┌─────────────────────────────────────┐
│   DeskBuddyLegends.Game (Godot 4)   │  ← Presentation only. Godot nodes, scenes, autoloads.
├─────────────────────────────────────┤
│   DeskBuddyLegends.Infrastructure   │  ← Implements Core abstractions (SQLite, Steam, OS hooks).
├─────────────────────────────────────┤
│   DeskBuddyLegends.Core             │  ← Zero external dependencies. Pure C# domain + application.
│   ├── Domain                        │      Entities, Value Objects, Events, Repository interfaces
│   ├── Application                   │      Use Cases, Services (XpEngine, AchievementEngine...)
│   └── SharedKernel                  │      Result<T>, Guard, Option<T>
└─────────────────────────────────────┘
```

**Dependency rule (strict):** Domain never imports Application, Infrastructure, or Godot. Infrastructure never imports Game.

---

## Key Architectural Decisions

| Decision | Choice | Reason |
|---|---|---|
| Renderer | Compatibility (OpenGL) | Transparent window breaks on Vulkan/Forward+ on many GPUs |
| GodotSteam | GDExtension + LauraWebdev C# Bindings | No custom Godot build needed; standard editor + export templates |
| Activity monitoring | `GetLastInputInfo` polling (5s interval) | `SetWindowsHookEx` deadlocks under .NET Core without a Win32 message loop |
| Window focus | `WS_EX_NOACTIVATE` via P/Invoke | Godot's `FLAG_NO_FOCUS` alone is insufficient for true click-through |
| EventBus | C# typed `event Action<T>` | Avoids GDScript↔C# marshalling overhead; Godot signals only for UI nodes |
| Persistence | SQLite (`Microsoft.Data.Sqlite` + Dapper) | Offline-first, ACID, zero-config, migration framework trivial |
| Save integrity | HMAC-SHA256, key = `HKDF(Steam64ID)` | Tamper detection without encryption; key ties save to account |
| Achievements | Data-driven JSON, metric-indexed at runtime | Adding achievement #501 = JSON entry only, zero code changes |
| Drop system | Cosmic Capsules — NOT chest clones | Original identity; pity tracked per-table, persisted to SQLite |
| FPS management | `OS.LowProcessorUsageMode` + `_Notification` | Primary CPU budget weapon; idle companion = near-zero CPU |
| DI | Thin `ServiceLocator` in Game layer | Core + Infrastructure use constructor injection; no heavy framework |

---

## Repository Structure

```
DeskBuddy-Legends/
├── .github/
│   └── workflows/
│       ├── pr-checks.yml           # lint · unit tests · 80% coverage gate · cross-platform build
│       ├── main-merge.yml          # integration tests · Codecov · perf regression
│       ├── nightly.yml             # daily BenchmarkDotNet threshold assertions
│       └── codeql.yml              # weekly security scanning
│
├── src/
│   ├── DeskBuddyLegends.Core/              # Pure C# — ZERO Godot/Steam dependencies
│   │   ├── SharedKernel/                   # Result<T> · Guard · Option<T> · IDateTimeProvider
│   │   ├── Domain/
│   │   │   ├── Entities/                   # Companion · PlayerProfile · Achievement · ActivitySession
│   │   │   ├── ValueObjects/               # Rarity · XpAmount · GenuineScore · EmotionalState · ...
│   │   │   ├── Events/                     # IDomainEvent + 7 concrete domain event records
│   │   │   ├── Repositories/               # ICompanionRepository · IPlayerProfileRepository · ...
│   │   │   └── Exceptions/                 # DomainException hierarchy
│   │   └── Application/
│   │       ├── Interfaces/                 # IActivityMonitor · ISteam* · ISaveIntegrityGuard · ...
│   │       └── Services/                   # XpEngine · AchievementEngine · AntiAbuseService
│   │
│   ├── DeskBuddyLegends.Infrastructure/    # Implements Core abstractions
│   │   ├── Persistence/SQLite/             # DatabaseContext · MigrationRunner · Repositories
│   │   ├── Persistence/Integrity/          # SaveIntegrityGuard (HMAC-SHA256)
│   │   ├── Persistence/Cloud/              # SteamCloudStorage · ConflictResolver
│   │   ├── Steam/                          # ISteam* adapters via GodotSteam
│   │   ├── ActivityMonitoring/             # WindowsActivityMonitor · LinuxActivityMonitor
│   │   ├── AntiAbuse/                      # Entropy · Pattern · Burst scoring layers
│   │   └── Localization/                   # JsonLocalizationProvider
│   │
│   └── DeskBuddyLegends.Game/              # Godot 4 project (added Phase 3)
│       ├── autoloads/                      # GameManager · EventBus · SteamManager · ...
│       ├── scenes/                         # Main · CompanionView · SettingsOverlay · ...
│       ├── resources/                      # CompanionDefinitions · AchievementDefs · DropTables
│       └── assets/                         # Sprites · Audio · Localization JSON
│
├── tests/
│   ├── DeskBuddyLegends.Tests.Unit/        # xUnit · Core only · no Godot · no SQLite
│   ├── DeskBuddyLegends.Tests.Integration/ # xUnit · Core + Infrastructure · real SQLite :memory:
│   ├── DeskBuddyLegends.Tests.Performance/ # BenchmarkDotNet · RAM/CPU threshold assertions
│   └── DeskBuddyLegends.Tests.Helpers/    # Builders · Fakes · shared fixtures
│
├── docs/
│   ├── Architecture/                       # Layer dependency map · Data flow diagrams
│   ├── ADR/                                # Architecture Decision Records (ADR-001 → ...)
│   ├── Roadmap/                            # MVP · Alpha · Beta · Early Access · V1
│   └── Systems/                            # Per-system deep dives
│
├── tools/
│   ├── build/                              # build-windows.ps1 · build-linux.sh · package-steam.ps1
│   └── art-pipeline/                       # compress-sprites.py · generate-atlas.py
│
├── art/Source/                             # Aseprite / PSD source files (not shipped)
├── steam/                                  # app_build.vdf · depot configs
├── VERSION                                 # Single source of truth for version (e.g. 0.1.0)
├── DeskBuddyLegends.sln
├── Directory.Build.props                   # Reads VERSION · shared MSBuild props
├── Directory.Packages.props                # Central NuGet version management
├── global.json                             # .NET 8 SDK pin
└── .editorconfig
```

---

## Domain Model

### Value Objects

| Type | Purpose |
|---|---|
| `Rarity` | 8-tier enum (Common → Void) + drop weight. Extensible via JSON registry — no code change for new tiers |
| `XpAmount` | Wraps non-negative `long`. Prevents negative XP at construction |
| `GenuineScore` | Normalized `float [0.0, 1.0]`. Below `0.25` → XP withheld, session flagged |
| `EmotionalState` | 10 states (Neutral/Happy/Excited/Sad/Curious/Bored/Tired/Affectionate/Sleeping/Working) with decay |
| `AffinityType` | None · Techno · Nature · Cosmic · Shadow · Flame · Frost · Storm · Light |
| `CompanionId` · `PlayerId` · `AchievementId` · `SkinId` · `EvolutionStage` | Strongly typed IDs and identifiers |

### Entities

| Entity | Responsibility |
|---|---|
| `Companion` | Core entity. State mutates only through methods that emit domain events. XP/level/evolution/skin/emotional state. Max level 100 (quadratic XP curve) |
| `PlayerProfile` | Aggregate root. Owns unlocked companions, achievements, total XP, session stats, login streak |
| `Achievement` | Data-driven. `CriteriaDefinition` evaluated generically by `AchievementEngine` — no per-achievement code |
| `ActivitySession` | Transient monitoring window. Holds raw event counts, `GenuineScore`, flagged status |

### Domain Events

`XpAwardedEvent` · `CompanionLeveledUpEvent` · `CompanionEvolvedEvent` · `EmotionalStateChangedEvent` · `AchievementUnlockedEvent` · `CapsuleOpenedEvent` · `ActivitySessionCompletedEvent` · `SeasonalEventActivatedEvent`

---

## Core Systems

### XpEngine
Formula: `BaseXp × GenuineScore × SessionLengthBonus × EventMultiplier`  
Daily XP cap enforced in `XpEngine` AND re-validated in `AwardXpUseCase` (defense in depth).  
`GenuineScore < 0.25` → `XpAmount.Zero`, no exception thrown.

### AchievementEngine
- Loads all definitions from JSON at boot into a **metric-indexed dictionary**
- Per domain event: evaluates only achievements matching the triggered metric — **O(relevant)**, not O(500)
- `CriteriaDefinition`: `{ Metric, Operator, Threshold, Window? }` — generic evaluator
- Adding achievement #501: JSON entry only, zero code changes required

### AntiAbuseService
Three-layer scoring → produces `GenuineScore`:
1. **Shannon entropy** of inter-event timing distribution (low entropy = bot-like regularity)
2. **Sliding window pattern hash** — detects repeated identical sequences
3. **Burst rate cap** — >25 events/sec is physiologically impossible sustained

No bans. Flagged sessions logged locally to `flagged_sessions` SQLite table. Never exfiltrated.

### Drop System — Cosmic Capsules
- Weighted random draw from `DropTable` resources
- **Pity system** per-table, persisted to SQLite across restarts (non-exploitable)
- Seeded PRNG: `SHA256(PlayerId + CapsuleId + UTCTimestamp)` — deterministic for audit

### Seasonal Events
- Remote activation via ECDSA P-256 signed JSON (public key pinned at compile time)
- Local calendar fallback for offline mode
- Modifies: drop weights, XP multiplier, UI theme, exclusive achievements

---

## Performance Targets

| Metric | Target |
|---|---|
| RAM (peak) | < 150 MB |
| CPU (average) | < 1% |
| Achievement evaluation (500 loaded) | < 5 ms per domain event |
| Companion asset lazy load | < 50 ms |
| Coverage (Core) | ≥ 80% |

### Key performance rules (Godot layer)
- `OS.LowProcessorUsageMode = true` always in background — primary CPU weapon
- Cache all `GetNode<T>()` in `_Ready()` via `[Export]`. Never in `_Process`
- No lambda allocations in `_Process` — method references only
- `VisibleOnScreenNotifier2D` → disable AnimationTree + particles + `SetProcess(false)` when off-screen
- Background thread → main thread: always via `Callable.From(...).CallDeferred()`

---

## Security

| Concern | Mitigation |
|---|---|
| Save tampering | HMAC-SHA256, key = `HKDF(Steam64ID, "DeskBuddy-Save-v1")`, sidecar `save.db.sig`. Mismatch → Steam Cloud restore |
| Remote event abuse | ECDSA P-256 signature on remote config JSON, public key pinned at compile time |
| XP/achievement farming | 4-layer AntiAbuseService + daily XP cap (double-enforced) |
| Sensitive data | Steam64 ID is public. No payment data. No tokens stored. Nothing exfiltrated |

---

## Roadmap

| Phase | Focus | Status |
|---|---|---|
| **Phase 0 — Foundation** | Solution scaffold · Domain · Events · Interfaces · CI/CD | ✅ Complete |
| **Phase 1 — Persistence** | SQLite DatabaseContext · MigrationRunner · Repositories · SaveIntegrityGuard | 🔲 Next |
| **Phase 2 — Core Systems** | XpEngine wiring · AntiAbuse · ActivityMonitor (OS) · AchievementEngine data load | 🔲 |
| **Phase 3 — MVP Godot Shell** | Transparent window · CompanionView · EventBus · Steam init · HUD | 🔲 |
| **Phase 4 — Steam Integration** | GodotSteam · Achievements · Cloud sync · Rich presence · Release pipeline | 🔲 |
| **Phase 5 — Alpha Content** | 5 companions · Cosmic Capsules · Collection UI · Emotional state machine | 🔲 |
| **Phase 6 — Beta Systems** | Seasonal events · Localization (6 langs) · 500 achievements | 🔲 |
| **Phase 7 — Early Access** | Steam Deck cert · BenchmarkDotNet thresholds · Accessibility | 🔲 |

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Godot 4.x with .NET support](https://godotengine.org/download/) *(required from Phase 3)*
- Steam account + Steamworks access *(required from Phase 4)*

### Build (Phases 0–2, no Godot needed)

```bash
dotnet restore DeskBuddyLegends.sln
dotnet build DeskBuddyLegends.sln -c Release
dotnet test tests/DeskBuddyLegends.Tests.Unit -c Release
dotnet test tests/DeskBuddyLegends.Tests.Integration -c Release
```

### Bump version

Edit only the [VERSION](VERSION) file. All projects inherit it via `Directory.Build.props`.

```bash
echo "0.2.0" > VERSION
```

---

## CI/CD

| Workflow | Trigger | Jobs |
|---|---|---|
| `pr-checks.yml` | PR → main | Format check · Unit tests · 80% coverage gate · Cross-platform build |
| `main-merge.yml` | Push → main | Integration tests · Codecov · Performance regression |
| `nightly.yml` | Daily 03:00 UTC | BenchmarkDotNet thresholds (< 150 MB / < 1% CPU) |
| `codeql.yml` | Weekly + PR | C# security scanning |

---

## Testing Strategy

| Suite | Scope | Framework |
|---|---|---|
| Unit | Core only — no Godot, no SQLite, no Steam | xUnit · FluentAssertions · NSubstitute |
| Integration | Core + Infrastructure — real SQLite `:memory:` | xUnit + SQLite in-process |
| Performance | Core + Infrastructure | BenchmarkDotNet with threshold assertions |

---

## Companion Roster (Planned)

Slime · Robot · Ghost · Dragon · Capybara · Fox · Raccoon · Spirit · Alien · Golem · Cosmic Being · ...

Each companion has: Level · XP · Rarity · Affinity · Emotional state · Evolution tree · Skins · Animations.  
Skins implemented as palette-swap shaders on shared sprite sheets — new skin = new `.tres` file, no new textures.

---

## License

Proprietary. All rights reserved. © 2026 DeskBuddy Legends.
