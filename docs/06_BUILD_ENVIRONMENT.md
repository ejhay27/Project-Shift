# 06 — Build Environment (dev machine setup)

> The Agent sandbox has **no** Unity, JDK, or Android SDK, so builds happen on a dev machine (or licensed CI). This doc is the repeatable setup.

## 1. Required software
1. **Unity 6 LTS (6000.x)** via Unity Hub. Open `unity/ProjectShift/` (project version pin: `ProjectSettings/ProjectVersion.txt`).
2. **Android Build Support** module (SDK + NDK + OpenJDK) from Unity Hub — preferred over manual SDK installs.
3. **JDK 17** (bundled with Unity 6 Android support; use the bundled one).
4. **Git LFS** for future binary assets (`git lfs install`) — not needed while the repo is code+JSON.

## 2. First open checklist
- [ ] Unity opens with **0 compile errors** (TECH-002).
- [ ] Window → General → Test Runner → run **EditMode** tests → all green.
- [ ] Menu **Project Shift > Build M1 Prototype Scene** → press Play → verify §03 flows.

## 3. Android build checklist
- [ ] File → Build Settings → Android → Switch Platform (IL2CPP, ARM64; ARMv7 only if min-spec demands).
- [ ] Player Settings: package `com.projectshift.game`, landscape orientation, min SDK per tests (start 26+, confirm in M10), target = latest verified.
- [ ] Build APK → `adb install` on test phone + tablet → run §03 exit tests.
- [ ] Release later: keystore (never commit), App Bundle (AAB), ProGuard/R8 as verified.

## 4. CI notes
- `validate.yml` runs Python validators on every push (works anywhere).
- A Unity build job is intentionally **not enabled**: it needs a license secret + Android modules + large runners. Enable in M1 on project infrastructure following this doc, not in the agent sandbox.

## 5. Troubleshooting
| Symptom | Fix |
|---|---|
| Wrong editor version warning | Install the pinned 6000.x (or newer 6.x LTS; let Unity upgrade a *copy* first) |
| Package resolve fails | Check Unity package registry access; delete `Library/` and reopen |
| `adb` sees no device | Enable USB debugging; `adb kill-server; adb start-server`; try another cable |
| Hotspot join fails (M8) | Use manual-IP path; see `05_MULTIPLAYER_ARCHITECTURE.md` §5 |
