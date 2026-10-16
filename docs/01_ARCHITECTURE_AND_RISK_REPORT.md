# 01 — Architecture & Risk Report (Milestone 0)

## 1. Architecture: three layers + data

```
┌────────────────────────────────────────────────────────┐
│ Unity Scene / Prefabs  (composition only, thin)        │
├────────────────────────────────────────────────────────┤
│ ProjectShift.Game      (MonoBehaviours: input, camera,  │
│  Unity adapters         HUD, save IO, MonoBehaviour     │
│                         bridges to Core)                │
├────────────────────────────────────────────────────────┤
│ ProjectShift.Core      (PURE C# — no UnityEngine;       │
│  simulation rules       inventory, wallet, clock, nodes, │
│                         teleport, saves, squads,        │
│                         guardian outcomes)              │
├────────────────────────────────────────────────────────┤
│ GameData (JSON now → ScriptableObjects later)           │
│  items / nodes / recipes / loot / creatures / dialogue  │
└────────────────────────────────────────────────────────┘
```

**Rules:**
1. All *rules* (costs, timers, caps, exclusivity, authority checks) live in **Core** and are deterministic functions of explicit inputs (UTC ticks, parameters) — no `DateTime.Now` or `Random` inside Core. Randomness/clock injection happens in `Game`.
2. `Game` never duplicates a rule — it adapts Core to Unity (input, physics, rendering, file paths).
3. One implementation per system (spec §1 rule 5). The registry's `code_refs` prove it.
4. Save format is versioned from day one (INV-004); every persistent system exposes `ToData()/Load` DTOs using only JsonUtility-safe types (no Dictionaries in DTOs).

## 2. Key structural decisions

| Decision | Rationale |
|---|---|
| Core has `noEngineReferences` | Forces testability; EditMode tests + future `dotnet test` both work |
| Timestamps, not tick counters, for respawns/cooldowns | Offline progress (GATHER-007) and anti-bypass (TELE-003) fall out naturally |
| Host-authoritative model designed now, built in M8 | Authority fields already in registry + Core APIs take explicit `playerId`/`nowUtc` so the MP retrofit is mechanical |
| Programmatic scene builder (`PrototypeSceneBuilder`) | Scenes are fragile YAML to hand-author; the builder regenerates M1 deterministically |
| JSON data + validator before ScriptableObjects | Content is reviewable/diffable now; SO tooling is additive, not a rewrite |

## 3. Top risks

| # | Risk | Impact | Mitigation |
|---|---|---|---|
| R1 | Scope dwarfism: spec is ~10× a normal M1 | Schedule collapse | Registry + strict milestone exits; every milestone is playable; backlog preserved, never silently cut (§1.9) |
| R2 | No Unity/Android verification in this sandbox | False "done" claims | Honest statuses; validators that *can* run here do; device tasks explicitly assigned (TECH-004, QA-*) |
| R3 | Open-world streaming on low-end Android | Perf failure | URP + sim-distance tiers designed in M0 (docs/04); streaming framework is M4, before content scale-up |
| R4 | Hotspot client isolation breaks discovery | MP unusable on phones | Manual-IP fallback from day one of M8; real-device matrix (NET-002) |
| R5 | Save corruption / dupe exploits | Progression loss, economy collapse | Atomic-write + checksum + backup (INV-004); exclusivity/cooldown state machines in Core with tests (TREEG-007, TELE-003, GUARD-003) |
| R6 | Touch UX sprawl (10-ability pets, build mode) | Unplayable on phones | Loadout-based ability UI mandated (UI-005); adjustable layouts (UI-002/UI-009) |
| R7 | Content data rot (IDs drift, bad refs) | Runtime errors | `validate_data.py` in CI; unique-ID + cross-ref checks |

## 4. Dependency order (condensed)

```
TECH (M0) → UI-001/002/003 + INV-001/004 (M1) → TECH-004 APK
  → GATHER/BUILD/FURN/SURV/INV-003 (M2) → COMBAT/BOSS/AFFIN/PET-001 (M3)
  → WORLD/TIME/MAP/PLAGUE/NPC-001 (M4) → PET/RIDE/HAB/WATER (M5)
  → CITY/NPC/VEH/GUARD/ECON/CHAR-001 (M6) → TREEG/TELE/POWER/ANCIENT (M7)
  → NET-* (M8) → WORLD-004/005/014 + STORY (M9) → PERF-005/QA (M10)
```

Full per-feature dependencies live in `registry/feature_registry.json` (`depends_on`), validated by `tools/validate_registry.py`.
