# PROJECT: SHIFT — Android Prototype Plan (Milestone 0 → 1)

Goal: prove the Android pipeline on a real device, then deliver the smallest playable loop: move, look, gather, save, reload.

## Prototype definition

"Prototype v0.1" = one landscape scene that a player can:
1. Start the app and see a title screen (Play / Settings / Quit).
2. Move with a left-side virtual joystick and look with a right-side swipe area.
3. Walk up to a tree or rock; a prompt appears ("Gather").
4. Gather: the node gives a resource stack to inventory; the node disappears and returns after a timer (short test value, e.g., 10 s, configurable).
5. Open a simple inventory panel that lists the counts.
6. Save manually and automatically on pause/quit; relaunch restores position, inventory, and node state.
7. Works on phone and tablet aspect ratios with a safe-area-aware layout.

Out of scope for v0.1: combat, base building, pets, multiplayer, cinematics, real art.

## Steps

### Step 1 — Blank Android build (M0 exit)
- Create Unity 6 URP mobile project; set IL2CPP, ARM64, target API 36, JDK/SDK/NDK per the installed editor.
- Commit `ProjectSettings/`, `Packages/manifest.json`, `Assets/` skeleton. Keep `Library/`, `Temp/`, `Build/`, `Logs/`, and APK/AAB out of Git (see `.gitignore`).
- Build debug APK; install via `adb install`; record the result in `docs/03_FEATURE_REGISTRY.md`.
- Test: app launches, shows a frame, stays alive for 5 minutes.

### Step 2 — Movement and camera (M1)
- Character controller on a flat test terrain with a few props (primitives OK for prototype; no fake art claims).
- Virtual joystick and camera swipe with ownership rules (a touch belongs to one control).
- Test (EditMode): joystick vector mapping and dead zone. Test (PlayMode/device): movement works at two resolutions.

### Step 3 — Interaction and gathering (M1)
- Interaction trigger volume shows a prompt; one tap on the Gather button collects.
- Resource node data as a ScriptableObject with ID, yield, respawn seconds.
- Respawn uses **wall-clock timestamps** (UTC) so it continues while the app is closed. Test: set node harvest time to T, advance a mocked clock 10 min, node is available.

### Step 4 — Inventory and save (M1)
- Inventory: item ID, count, stack limit. Test: add beyond stack limit spills correctly.
- Save: `save.json` with `"schemaVersion": 1`. Writes go to `save.tmp` then rename; keep `save.bak1`.
- Tests: round-trip; corrupt file falls back to backup; interrupted write (tmp present, main valid) keeps main valid.

### Step 5 — Mobile UI pass (M1)
- Canvas Scaler reference 1920×1080 landscape; safe-area padding.
- Verify on: a 16:9 phone (e.g., 2400×1080), a 4:3 tablet (e.g., 2000×1500), and a 16:10 tablet.
- Touch targets ≥ 48 dp.

### Step 6 — Device evidence (M1 exit)
- Install on real device(s); record model, OS, GPU, RAM, average fps for 3 minutes, peak memory from the Unity Profiler, and battery/thermal observations.
- Record results in `docs/05_DEVICE_TEST_LOG.md` (created at that time). Do not record numbers that were not measured.

## Acceptance checklist (v0.1)

- [ ] APK builds from clean checkout (commands recorded)
- [ ] Installs and launches on a real Android device
- [ ] Joystick and camera swipe work together without conflicts
- [ ] Gather produces inventory items
- [ ] Node disappears and returns after the configured time, including after app restart
- [ ] Save/load restores position, inventory, and node state
- [ ] Corrupt-save recovery tested
- [ ] Layout checked at three aspect ratios
- [ ] Performance numbers recorded from device, not estimated

Until the boxes are checked with evidence, the prototype is **Planned**, not complete.
