# PROJECT: SHIFT — Dependency-Ordered Roadmap

Status key: **Planned** · **In progress** · **Implemented** (code exists, not yet tested) · **Tested** (verified by test/build evidence) · **Blocked** (cannot proceed; reason given).

| Milestone | Scope (summary) | Depends on | Status | Exit condition (from spec) |
|---|---|---|---|---|
| M0 | Repo audit, technology validation, Android pipeline, feature registry | — | **In progress**: audit, architecture, registry, roadmap written. Android pipeline **Blocked** (toolchain). | Tech decision documented and minimal Android build demonstrated. |
| M1 | Mobile foundation: small 3D scene, movement/camera, phone+tablet UI, prompts, basic inventory and gathering, local save/load, APK | M0 | Planned (blocked on M0 build) | Install and play on a device, gather, save progress. |
| M2 | Survival and construction: woodcutting, stone/ore, respawn rules, crafting, building tiers I–II, furniture/doors, basic needs, storage/workbench, initial Settings tutorials | M1 | Planned | Gather, craft, build, interact, save, return to persistent base. |
| M3 | Combat and creatures: health/stamina, weapons, enemy AI, feedback, boss framework, initial wildlife, pet collection + basic commands, initial affinities | M2 | Planned | One complete, repeatable test encounter with real combat logic. |
| M4 | World systems: day/night, weather, streaming, plague/environment events, dynamic resources, map progression, journal, discovery, NPC routines | M3 | Planned | World changes over time without fully simulating every region. |
| M5 | Pets and advanced survival: ability collection/loadouts, direct pet control, home guardians, habitats, dragon riding, skywalking, fishing/gardening/food buffs/rain network, advanced crafting | M4 | Planned | Pet, survival, exploration interact correctly and persist through saves. |
| M6 | Cities and transport: Sanctuary City, NPC dialogue with stored audio, markets/currency, subway/elevated, vehicles, interiors, City Guardians trials, Relic Beasts | M5 | Planned | One city with meaningful exploration, services, transport, persistence. |
| M7 | Ancient systems and World Tree: World Tree environment, Worldheart materials/construction, unique Guardian with exclusive outcomes, Wayfarer Relic and Waygates, electrical systems and automated defense, ancient world-scale weapons, relic research | M6 | Planned | Guardian outcomes, save state, resource access, construction rules tested and not exploitable by reconnect/reload. |
| M8 | Local multiplayer: host/join, discovery + manual IP, player/world sync, building permissions, pet/vehicle sync, boss rewards, authority, reconnection, host disconnect handling | M7 (networking spike may start earlier) | Planned; spike **Blocked** on device/toolchain | Multiple Android devices reliably play the supported scenario; limitations documented. |
| M9 | Main story and expansion: opening cinematic, more continents/regions, underground kingdoms, more cities, bosses, ancient quests, departure ending, credits | M8 | Planned | Narrative completable from opening to ending. |
| M10 | Optimization and release: device compatibility, profiling, save migration tests, multiplayer abuse tests, touch/UI tests, crash testing, signing, final feature audit, RC | M9 | Planned | Android release candidate. |

## Dependency notes

- The Worldheart, Guardian, and Waygate rules depend on M6 transport and M3 bosses; they are scheduled at M7, not earlier.
- The opening cinematic (spec section 5) is scheduled at M9 but a short placeholder sequence may be used to test the camera pipeline in M1. A placeholder must be labelled as such in the registry.
- Multiplayer authority (M8) constrains all earlier systems: M1–M7 must keep state changes in host-capable services from day one (no client-only progression writes). This is a design rule for every milestone, not just M8.
- Save format (M1) must be versioned from the first save, so later milestones add fields via migrations.
- Ancient world-scale weapons (M7) depend on construction, power, and world events and are intentionally late.

## Unblocking options for the Android toolchain (choose one)

1. **Expand the sandbox allowlist** to include `dl.google.com`, `services.gradle.org`, `download.unity3d.com`, `public-cdn.cloud.unity3d.com`, and a JDK 17 source (e.g., `api.adoptium.net`). Then M0 can install Unity 6 and Android SDK here (large downloads, plan time and disk: several GB, sandbox has ~20 GB free).
2. **Owner-built pipeline:** the owner runs Unity Hub on a local machine, or a self-hosted runner, and commits the pinned versions and build logs. The agent can then work from those results.
3. **Pre-provisioned image** with Unity 6 (matching editor version), JDK 17, Android SDK platform 36 and build-tools, and NDK as Unity requires.

Until one is available, M0 exit is not met and no M1+ build or device claim can be made.

## Milestone 0 remaining work (once a toolchain is available)

1. Install Unity 6 and record the exact version.
2. Create the URP mobile project, set IL2CPP + ARM64, target API 36.
3. Build a blank scene APK; install on a real Android device (or emulator with ARM64 translation, documented as weaker evidence).
4. Record device model, Android version, GPU, RAM, and frame time.
5. Run a LAN discovery spike with two devices on Wi-Fi and on a hotspot; record which path works.
6. Update `docs/03_FEATURE_REGISTRY.md` statuses with evidence.
