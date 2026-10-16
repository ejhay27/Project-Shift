# 00 — Repository & Architecture Audit (Milestone 0)

**Date:** 2026-10-16 · **Auditor:** Development AI (Arena Agent Mode) · **Branch:** `arena/473c1088-project-shift`

## 1. What was found

| Item | Result |
|---|---|
| Repo contents at audit | `README.md` (1 line) + `.git` only. **Effectively empty.** |
| Engine / code / assets | None present. No Unity project, no source, no assets, no CI. |
| Prior branch | `main` @ `4050465` ("Initial commit"). No other history. |
| Working features to preserve | **None exist** — nothing to migrate or protect. |
| Build pipeline | None. No Android SDK, JDK, Unity, or .NET in this sandbox (verified: only Node 22 + Python 3.11). |

Conclusion: per spec §1 ("if there's no repo inside it, build it from scratch"), this is a **greenfield build**.

## 2. Engine decision

**Recommended: Unity 6 (6000.x LTS) + C#, as the spec proposes.** Reasons:

1. Mature Android export, ARM/IL2CPP pipeline, and per-device quality scaling.
2. URP gives the needed mobile/GPU scalability for the §3 art direction and §34 budgets.
3. Largest hiring/community base for the systems required (open world streaming, Netcode, mobile UI).
4. No existing engine to migrate away from, so migration cost is zero.

Alternatives considered and rejected: Unreal (heavier on low-end Android, larger APKs, smaller mobile-survival precedent on the target class); Godot (weaker Android/console tooling maturity for this scope, smaller 3D open-world ecosystem); custom engine (unjustifiable cost).

## 3. Stack confirmation

| Layer | Choice |
|---|---|
| Engine | Unity 6 LTS (project opens on default settings first; URP migration is TECH-005) |
| Language | C# (gameplay); HLSL only where custom shaders justify it |
| Data | JSON game data now (validated by `tools/validate_data.py`); ScriptableObject mirrors when editor tooling lands |
| UI | Unity uGUI for M1 (zero package risk); evaluate UI Toolkit / Input System migration in M2–M3 |
| Networking (M8) | Netcode for GameObjects + UnityTransport (direct IP) + custom UDP LAN discovery + manual-IP fallback (see `05_MULTIPLAYER_ARCHITECTURE.md`) |
| Saves | Versioned JSON envelope, SHA-256 checksum, temp-file + backup rotation (INV-004) |
| Tests | Unity Test Framework (EditMode NUnit) for `ProjectShift.Core`; Python validators for registry/data/static checks in CI |

## 4. What was created in response (this milestone)

- `registry/feature_registry.json` — master registry (all spec sections, stable IDs).
- `unity/ProjectShift/` — Unity 6 skeleton: manifest, version pin, asmdefs, pure-C# core, Unity adapters, EditMode tests, starter JSON data, programmatic prototype-scene builder.
- `tools/` — registry / data / C# static validators (runnable here).
- `.github/workflows/validate.yml` — CI for the above.
- `docs/01–07` — architecture, roadmap, prototype plan, rendering, multiplayer, build env, data formats.

## 5. Honest verification status

- ✅ Python validators **run and pass in this sandbox** (see commit message / CI log).
- ⚠️ Unity compile, EditMode tests, and APK build **cannot run here** (no Unity/Android SDK; sandbox network allows only github/npm/pypi hosts). They are dev-machine/CI-with-license tasks with steps in `docs/06_BUILD_ENVIRONMENT.md`.
- Nothing is marked `tested` in the registry without evidence (registry notes + §1 rule 7/10).
