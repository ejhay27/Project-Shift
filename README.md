# PROJECT: SHIFT

**Android-first 3D open-world survival adventure** — Unity 6 + C#.
*The world does not end. It shifts.*

## Status: Milestone 0 (audit + foundation)

| Deliverable | Location |
|---|---|
| Repository audit | `docs/00_REPOSITORY_AUDIT.md` |
| Architecture & risk report | `docs/01_ARCHITECTURE_AND_RISK_REPORT.md` |
| Implementation roadmap | `docs/02_IMPLEMENTATION_ROADMAP.md` |
| Android prototype plan (M1) | `docs/03_ANDROID_PROTOTYPE_PLAN.md` |
| Rendering & performance strategy | `docs/04_RENDERING_PERFORMANCE_STRATEGY.md` |
| Multiplayer architecture | `docs/05_MULTIPLAYER_ARCHITECTURE.md` |
| Build environment setup | `docs/06_BUILD_ENVIRONMENT.md` |
| Data & save formats | `docs/07_DATA_AND_SAVE_FORMATS.md` |
| Master feature registry | `registry/feature_registry.json` |
| Unity 6 project | `unity/ProjectShift/` |

## Quickstart (dev machine with Unity 6 LTS)

1. Open `unity/ProjectShift/` in Unity 6 → expect **0 compile errors**.
2. Test Runner → run **EditMode** tests → expect all green.
3. Menu **Project Shift > Build M1 Prototype Scene** → press Play.
4. Android APK steps: `docs/06_BUILD_ENVIRONMENT.md`.

## Repo layout

```
registry/   master feature registry (every spec requirement, stable IDs)
docs/       M0 audit, architecture, roadmap, strategies
unity/      Unity 6 project: Core (pure C# rules) + Game (adapters) + Tests + GameData
tools/      validators (registry / data / C# static) — run in CI
```

## Honesty policy

Registry entries become `tested` only with verification evidence. Unity compile,
EditMode runs, and APK builds require a dev machine (see audit §5) — the agent
sandbox runs the Python validators only.
