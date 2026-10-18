# PROJECT: SHIFT — Architecture and Technology Decision

Status: **Proposed (Milestone 0). Not yet validated on a device.**

## 1. Engine: Unity 6 with C# (retain the proposed default)

Reasons:
- The repository is empty, so there is no migration cost to weigh.
- Unity 6 supports Android (ARM64 for phones and tablets) and Windows, so the future PC milestone has a path.
- C# covers gameplay, AI, saves, and UI logic; Kotlin/Java only where a native plugin is needed (e.g., hotspot or Wi-Fi state).
- ScriptableObjects and JSON fit the data-driven content the spec asks for (items, recipes, creatures, pets, abilities, loot, dialogue).
- Mature Android tooling and profilers (Unity Profiler, Memory Profiler, Android GPU tools).

Alternatives considered and rejected for now:
- **Unreal Engine:** higher mobile baseline cost and build size; C++/Blueprint split adds complexity for a mobile-first team.
- **Godot:** good for 2D/light 3D; open-world streaming, mobile shader and tooling maturity for this scope are weaker.
- **Web/React/HTML:** explicitly prohibited by the spec for the primary game.

Re-evaluate only if Milestone 1 on a real device shows the Unity Android baseline cannot reach the minimum performance target.

## 2. Rendering pipeline: Universal Render Pipeline (URP), mobile profile

- Use URP (not HDRP). HDRP is not a mobile target.
- Forward rendering with **single-pass instanced** rendering; Vulkan preferred, OpenGL ES 3.2 fallback. Test both on the minimum device class.
- No real-time global illumination on Android baseline. Use baked lightmaps for static geometry and light probes; reserve real-time shadows for the player and nearby pets.
- Shadow cascades: 1–2 on low, up to 3 on high; shadow distance is a quality setting.
- Post-processing: limited to a single cheap pass (color grade/bloom optional). No SSAO/SSR on baseline.
- Custom HLSL only where justified: water, foliage wind, affinity glow, World Tree material. Each shader must have a mobile variant and an LOD/fallback.
- Quality presets: **Low / Medium / High / Ultra (PC later)** controlling render scale, shadow quality, draw distance, LOD bias, particle budget, and target frame rate (30 / 45 / 60 fps).

## 3. Minimum device class (to be fixed by measurement)

Spec requires a documented minimum based on actual testing. Until a device run exists, this is **undetermined**. Proposed test matrix:
- Low: 4 GB RAM, Adreno 6xx / Mali-G5x class, 1080p phone.
- Mid: 6–8 GB RAM, Adreno 7xx / Mali-G7x class, phone.
- Tablet: 8 GB+, 2K display.

Record measured frame time, thermal state after 20 minutes, battery drain per 10 minutes, and peak memory. Only after that set the minimum.

## 4. Simulation and streaming model

- World divided into fixed-size **regions** (proposed 256 m × 256 m cells, tuned later) loaded by distance using Addressables or a controlled scene-streaming manager.
- Simulation tiers: **Active** (player-near: full AI/physics), **Nearby** (simplified AI, reduced update rate), **Distant** (summarized state, scheduled updates), **Unloaded** (persistence and offline events only; no per-entity updates).
- Object pooling for projectiles, particles, pickups, hit numbers, and pet effects.
- Async loading for regions and audio; main thread budget enforced per frame.
- A central `WorldDirector` service owns event scheduling, plague state, and offline summaries (spec section 6).

## 5. Data and persistence

- Content definitions: **ScriptableObjects** for editor-authored data; exported **JSON** for designers and tests. Each content type has a stable string ID.
- Save data: **versioned JSON** with a schema version field, validated on load.
- Write strategy: write to `save.tmp`, flush, then atomic rename to `save.json`; keep rolling backups (`save.bak1..3`). On load, validate; if invalid, fall back to the newest valid backup and report to the player. Never overwrite a valid save with a failed/partial write.
- Save slots and app-interruption recovery are required in Milestone 1.
- Save migration: each version bump needs a migration function and a test fixture from the previous version.
- Timestamps stored as UTC and game-time ticks; offline summaries use elapsed real time with caps.

## 6. Networking strategy (local Wi-Fi and hotspot first)

Spec constraints: host-authoritative, local discovery plus manual IP, no MMO architecture in the first milestone.

Proposed approach:
- **Topology:** one host (authoritative simulation) and up to N clients (proposed initial cap: 4 players, tuned by measurement).
- **Transport:** UDP-based reliable/unreliable channels. Candidates to evaluate in a spike (Milestone 8 spike, or earlier if needed): **Unity Netcode for GameObjects with Unity Transport**, or **LiteNetLib** (C#, lightweight, widely used for LAN games). Evaluation criteria: Android IL2CPP support, packet sizes, maintenance status, host-authority model fit, and testability. **Decision deferred until the spike; recorded here as open.**
- **Discovery:** UDP broadcast on the local subnet with a game-specific signature. Fallback: manual entry of host IP and port, and a short join code displayed by the host.
- **Hotspot caveats (documented, not assumed solved):** Android hotspot client isolation, mobile-data routing, and broadcast filtering can block discovery. The manual IP path must work even when discovery fails.
- **Authority:** the host owns time, weather, AI, bosses, loot, building permissions, resource nodes, pet assignments, vehicles, teleport cooldowns, guardian outcomes, and shared events. Clients send intents; the host validates and replies with state deltas.
- **Bandwidth budget:** target update rate of 10–20 Hz for remote entities, delta-compressed snapshots, and interest management by region. Exact budgets set after the spike.
- **Future internet multiplayer:** keep the authority/transport boundary behind an interface so a relay or dedicated server can be added later. Do not promise scaling now.

## 7. Android build pipeline

- Unity 6 editor version: **to be pinned** once installed (record in `ProjectSettings/ProjectVersion.txt`).
- Scripting backend: **IL2CPP**, target architecture **ARM64** (ARMv7 dropped unless a measured need appears).
- Minimum API: to be set from measured device support; Unity 6 default minimums apply until measured.
- **Target API: 36** for new and updated store builds (Google Play requirement from 2026-08-31; verify on Google's official page before release).
- Build: Unity exports the Android Gradle project; Gradle builds the APK/AAB. JDK 17 required. NDK and Gradle versions come from the installed Unity version's Android Build Support settings.
- Outputs: development APK for testing; AAB for store. Build artifacts stay out of Git (see `.gitignore`).
- Signing: debug keystore for development; release keystore is owner-managed and never committed.

## 8. Input and UI architecture

- Unity's Input System with touch and on-screen controls.
- Separate **input intents** (move, look, interact, fire, aim, ability) from touch mapping, so the UI layout can be swapped and tested without changing gameplay.
- Gesture arbitration: a touch is owned by exactly one control (joystick, camera swipe, build/aim, UI). Ownership rules are unit-tested.
- Responsive UI: Canvas Scaler set to a reference resolution with safe-area handling; layouts switch between phone and tablet breakpoints.
- Contextual buttons: only actions available now are visible.

## 9. Code organization (proposed, created incrementally)

```
Assets/ProjectShift/
  Core/          bootstrap, service locator, logging, time
  Save/          versioned save, atomic writer, migration
  World/         regions, streaming, WorldDirector, time/weather
  Survival/      needs, cooking, water
  Gathering/     resource nodes, respawn, tools
  Inventory/     items, stacks, crafting
  Building/      placement, snapping, permissions
  Combat/        health, weapons, damage, AI
  Pets/          collection, squad, abilities
  Networking/    host, client, discovery, authority
  UI/            mobile layouts, touch controls, menus
  Data/          ScriptableObject definitions, JSON importers
Assets/Tests/    EditMode and PlayMode tests
docs/            audit, architecture, roadmap, registry, prototype plan
```

Rules: no parallel implementations of one system; gameplay logic in plain C# where possible so it can be tested without scene objects.

## 10. Open decisions (must be resolved before the named milestone)

| ID | Decision | Needed by | Proposed answer |
|---|---|---|---|
| D1 | Networking library | Milestone 8 spike (earlier if discovery prototype needs it) | Spike both Netcode for GameObjects and LiteNetLib; choose by criteria in section 6. |
| D2 | Minimum Android device spec | Milestone 1 | Measure on Low/Mid/Tablet matrix. |
| D3 | Inherited pet ability rule | Milestone 3 (before pet abilities are coded) | See section 11. |
| D4 | Player-usable ability cost model | Milestone 5 | Energy plus cooldown, values in data. |
| D5 | Currency specifics (SHD/CR/AET) sinks and caps | Milestone 6 | Defined in economy registry; tuned by balance tests. |
| D6 | Save slot count and cloud (none planned) | Milestone 1 | 3 local slots; no cloud. |
| D7 | Exact region size and count | Milestone 4 | Proposed 256 m cells; measure. |

## 11. Proposed decision D3: inherited pet abilities

Proposal (needs owner sign-off):
- **Pet-exclusive abilities** are permanently tied to the pet that has them and work only while that pet is in play.
- **Player/inherited abilities** are **permanently learned** into the player's ability book once the source pet is first bonded to a required mastery level. This keeps collection value without requiring the pet to be present for every use.
- **Using** a learned inherited ability by the player additionally requires the source pet to be in the active five-pet squad. Without that pet, the ability is greyed out with a reason shown.
- Loadout: the player picks up to a fixed number of active ability slots (set in data), separate from the pet's collection capacity.

This keeps three distinct things separate, as the spec requires: pet ability collection, individual pet loadout, and player ability loadout.
