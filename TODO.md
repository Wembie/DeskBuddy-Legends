# DeskBuddy Legends — TODO

## Immediate

- [ ] Commit staged CI fixes (10 files: IDE1006 field renames + whitespace)
- [ ] Verify CI passes: format check + build-validation + unit-tests jobs

---

## Phase 1 — Persistence Foundation

> Prereq: .NET 8 SDK installed. Run `dotnet restore` before starting.

### Infrastructure / SQLite

- [ ] `src/DeskBuddyLegends.Infrastructure/Persistence/SQLite/DatabaseContext.cs`
  - Open SQLite connection via `Microsoft.Data.Sqlite`
  - Enable WAL mode + foreign keys pragma
  - Expose `IDbConnection` for Dapper queries

- [ ] `src/DeskBuddyLegends.Infrastructure/Persistence/SQLite/IMigration.cs`
  - Interface: `int Version`, `Task Up(IDbConnection)`

- [ ] `src/DeskBuddyLegends.Infrastructure/Persistence/SQLite/MigrationRunner.cs`
  - Read `user_version` pragma
  - Apply pending `IMigration.Up()` in version order, atomically per migration
  - Update `user_version` after each

- [ ] `src/DeskBuddyLegends.Infrastructure/Persistence/SQLite/Migrations/V001InitialSchema.cs`
  - Tables: `companions`, `player_profile`, `achievements`, `unlocked_achievements`,
    `activity_sessions`, `flagged_sessions`, `pity_counters`
  - All FK constraints + indexes on hot query paths

### Repositories (implement Core interfaces)

- [ ] `CompanionSqliteRepository` — implements `ICompanionRepository`
- [ ] `PlayerProfileSqliteRepository` — implements `IPlayerProfileRepository`
- [ ] `AchievementSqliteRepository` — implements `IAchievementRepository`
- [ ] `ActivitySessionSqliteRepository` — implements `IActivitySessionRepository`

### Save Integrity

- [ ] `src/DeskBuddyLegends.Infrastructure/Persistence/Integrity/SaveIntegrityGuard.cs`
  - Implements `ISaveIntegrityGuard`
  - HMAC-SHA256 over DB snapshot bytes
  - Key = `HKDF(Steam64ID, "DeskBuddy-Save-v1")`
  - Writes/reads sidecar `save.db.sig`
  - Mismatch → `Result.Failure("TAMPERED")`

### Tests

- [ ] `tests/DeskBuddyLegends.Tests.Integration/` — scaffold xUnit project
- [ ] Migration chain test: fresh `:memory:` DB → apply V001 → assert all tables exist
- [ ] Repository round-trip tests (save → load → assert equal)
- [ ] `SaveIntegrityGuard` tests: valid file passes, tampered file fails

---

## Phase 2 — Core Systems

### Use Cases

- [ ] `AwardXpUseCase` — calls XpEngine, validates daily cap (defense in depth), calls `Companion.AwardXp`, saves via repo

### Activity Monitoring

- [ ] `WindowsActivityMonitor` — `GetLastInputInfo` polling every 5s on background thread
  - Fire `Callable.From(...).CallDeferred()` for thread-safe Godot crossing
  - Token bucket: MAX_TOKENS=1000, refill 10/sec
- [ ] `LinuxActivityMonitor` — `/proc/interrupts` diff or `libinput` events
- [ ] `ActivityMonitorFactory` — returns platform-appropriate impl

### Data Loading

- [ ] Achievement definitions JSON schema + 50 seed achievements
  - `resources/AchievementDefs.json` in Core (or Infrastructure reads from file)
  - `AchievementEngine.LoadDefinitions()` wired to JSON loader at app boot
- [ ] `JsonLocalizationProvider` — implements `ILocalizationProvider`
  - Load from `assets/Localization/{lang}.json`

### Tests

- [ ] `XpEngine` unit tests: daily cap, GenuineScore=0 → zero XP, session length bonus
- [ ] `AntiAbuseService` unit tests: constant-interval input → score < 0.25, random → score ≥ 0.5
- [ ] `AchievementEngine` unit tests: 500 loaded, metric-indexed eval, O(relevant) not O(500)

---

## Phase 3 — MVP Godot Shell

> Prereq: Godot 4.x with .NET support installed.

- [ ] Create Godot 4 project in `src/DeskBuddyLegends.Game/`
  - `project.godot`: Compatibility renderer, transparent window, borderless, `LowProcessorMode=true`
- [ ] `NativeWindowManager.cs` — P/Invoke `SetWindowLongPtr`, add `WS_EX_NOACTIVATE | WS_EX_TOOLWINDOW | WS_EX_LAYERED`
- [ ] `ServiceLocator.cs` — composition root, wires Core + Infrastructure
- [ ] Autoloads: `GameManager`, `EventBus`, `SteamManager`, `WindowManager`, `SettingsManager`, `SaveManager`, `AudioManager`
- [ ] `CompanionView.tscn` — `AnimationTree` state machine (Idle/Working/Happy/Excited), placeholder sprite
- [ ] `VisibleOnScreenNotifier2D` — disables AnimationTree + particles + `SetProcess(false)` off-screen
- [ ] `NotificationPopup.tscn` — achievement toast, level-up banner, 4s auto-dismiss queue
- [ ] HUD: XP bar, level label, activity indicator
- [ ] Wire full loop: `ActivityMonitor → XpEngine → Companion → EventBus → CompanionView`
- [ ] Manual smoke test: transparent window on Windows + Linux

---

## Phase 4 — Steam Integration

- [ ] Install GodotSteam GDExtension + LauraWebdev C# bindings in `addons/`
- [ ] `SteamAchievementsAdapter` — implements `ISteamAchievements`
- [ ] `SteamCloudStorage` — implements `ISteamCloud`
- [ ] `SteamRichPresenceAdapter` — implements `ISteamRichPresence`
- [ ] `ConflictResolver` — highest `TotalSessionSeconds` wins on startup sync
- [ ] `SyncPendingAchievementsUseCase` — push locally unlocked achievements to Steam on startup
- [ ] `steam/app_build.vdf` + depot configs
- [ ] `release.yml` pipeline — Godot headless export + `steamcmd` depot upload
- [ ] Wire 50 achievements to Steam API keys

---

## Phase 5 — Alpha Content

- [ ] 4 additional companions (Robot, Ghost, Dragon, Capybara) with affinities + evolution trees
- [ ] `DropSystem`: `CosmicCapsule`, `DropTable` resources, per-table pity persisted to SQLite
- [ ] `CapsuleOpening.tscn` — animated reveal
- [ ] `CollectionUI.tscn` — grid + rarity shader filter
- [ ] `InventoryRepository` + `OpenCosmicCapsuleUseCase`
- [ ] Emotional state machine: resource-file `StateTransitionTable`
- [ ] Expand to 150 achievements

---

## Phase 6 — Beta Systems

- [ ] `SeasonalEventActivator` — ECDSA P-256 signed JSON + local calendar fallback
- [ ] Halloween + Christmas seasonal events (drop weights, XP multiplier, exclusive achievements)
- [ ] `JsonLocalizationProvider` + `FallbackLocalizationChain`
- [ ] Translations: `es`, `pt`, `de`, `fr`, `zh-simplified`, `ja`
- [ ] `SettingsOverlay.tscn` with language selector
- [ ] Expand to 500 achievements
- [ ] Steam Trading Card metadata

---

## Phase 7 — Early Access Hardening

- [ ] BenchmarkDotNet profiling suite: verify < 150 MB RAM, < 1% CPU on target hardware
- [ ] `nightly.yml` benchmark pipeline wired to assertions
- [ ] Steam Deck: `SteamInputAdapter`, 800×480 layout tests, touch support
- [ ] Accessibility: font scaling, high contrast toggle
- [ ] Localize Steam store page
- [ ] Steam Deck certification checklist

---

## CI / Quality Gates

| Gate | Status |
|---|---|
| `dotnet format --verify-no-changes` (Core + Infra) | Fixing (staged) |
| `dotnet build` — Core compiles | Passing |
| `dotnet build` — full solution | Passing |
| Unit tests + 80% coverage | No tests written yet |
| Integration tests | Phase 1 |
| BenchmarkDotNet thresholds | Phase 7 |
