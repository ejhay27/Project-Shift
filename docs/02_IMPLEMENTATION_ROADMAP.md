# 02 — Implementation Roadmap (dependency-ordered)

Companion to the registry (`depends_on` fields are authoritative for ordering). Each milestone ends with a **playable, tested exit**.

## M0 — Audit & validation ✅ (docs) / 🟡 (Unity open pending)
Deliverables: this doc set, registry, skeleton, core rules, validators, CI.
Exit: docs match repo; validators pass; Unity 6 opens project clean (dev machine).

## M1 — Mobile foundation (next)
- Prototype scene via `Project Shift > Build M1 Prototype Scene`.
- Player movement + camera rig + touch joystick + prompts + interact.
- Inventory add/remove + starter items; save/load round-trip.
- URP migration + Low/Med/High presets; settings UI shell.
- **Exit:** APK on a real device — move, gather, save, reload. (TECH-004)

## M2 — Survival & construction
Wood/stone/ore gathering + respawn, tools, crafting, build T1/T2, furniture/doors, needs, storage, workbenches, tutorial library seed.
**Exit:** gather → craft → build → save → return to persistent base.

## M3 — Combat & creatures
Vitals, weapons/ammo, enemy AI, boss framework + Thorn Crown, wildlife seed, pet collection basics, affinities seed, bombs, progression framework.
**Exit:** repeatable test encounter with real combat logic.

## M4 — World systems
Streaming, day/night + weather, plague director seed, resources, map T1–T5, journal, discovery, NPC routines, forest/mountain/grassland/water biomes, ruins.
**Exit:** world changes over time without full simulation everywhere.

## M5 — Pets & advanced survival
Ability collection/loadouts, direct control, guardians, habitats, dragon riding, skywalk, fishing/gardening/food buffs, rainwater networks, legendaries/armor, build T3.
**Exit:** pet/survival/exploration interplay persists through saves.

## M6 — Cities & transportation
Sanctuary City #1, NPC dialogue + stored VO, markets + 3-currency economy, subway/maglev, vehicles + dealerships, city guardians + trials, relic beasts, customization.
**Exit:** one city with exploration, services, transport, persistence.

## M7 — Ancient systems & World Tree
Tree + guardian + exclusive outcomes, Worldheart materials/construction, Wayfarer + Waygates, power/defense grids, puzzles doors, ancient projects begin, build T4.
**Exit:** guardian outcomes/cooldowns/construction exploit-proof vs reload/reconnect.

## M8 — Local multiplayer
Netcode + transport + LAN discovery + manual IP, authority enforcement, full-domain sync, permissions, reconnect/host-loss, budgets.
**Exit:** multi-device local sessions with documented limits.

## M9 — Story & expansion
Opening cinematic, remaining biomes/regions, underground kingdoms, more cities/bosses, ancient quests, departure ending + credits.
**Exit:** main narrative completable start → finish.

## M10 — Optimization & release
Profiling, min-spec doc, migration/recovery tests, abuse matrix, UI/device matrix, signing, RC.
**Exit:** signed Android release candidate. PC + internet MP stay explicitly unscheduled until then.

## Backlog policy
Anything that slips keeps its registry entry with status + reason in `notes`. Nothing is deleted or downgraded silently (§1.9, §37).
