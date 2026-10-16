# 05 — Local Multiplayer Architecture (M8 target, designed in M0)

## 1. Scope (spec §32)
Local Wi-Fi + mobile hotspot only. One host, N clients. No MMO, no internet promises.

## 2. Recommended stack
- **Netcode for GameObjects** + **UnityTransport** (direct-IP connect works on LAN/hotspot).
- **Custom UDP broadcast discovery** on the subnet for "find host" UX, with **manual IP entry fallback** (mandatory: hotspot client isolation breaks broadcast on many Android devices — NET-002).
- Design now, implement in M8: Core APIs already take explicit `playerId` + `nowUtcTicks` so authority checks retrofit mechanically.

## 3. Authority model
Host owns: world state, time/weather, NPC/creature/boss AI, loot, building + permissions, nodes, pets/assignments, vehicles, teleport cooldowns, guardian outcomes, shared events (registry `authority: host` entries). Clients send validated requests; host simulates and replicates. See NET-003…NET-007.

## 4. Critical rules (already locked in Core)
| Rule | Core class | Registry |
|---|---|---|
| Teleport 5-min cooldown survives rejoin; failures don't consume | `TeleportNetwork` | TELE-003 |
| World Tree guardian: slay-loot ⊕ bond-pet, never both, never respawns | `WorldTreeGuardianOutcome` | TREEG-007 |
| City guardian: dormant → ~10-min resurrect with countdown | `SanctuaryGuardianState` | GUARD-003 |
| Journey squad cap 5/player; home posts restore | `PetSquad` | PET-005 |
| Stone nodes ~10-min respawn, timestamp-deterministic | `ResourceNodeSystem` | GATHER-002 |

## 5. Failure handling (M8 acceptance)
- Leave/rejoin: progression + cooldowns intact (host-persisted). (NET-006)
- Host disconnect: session pauses with clear UX; documented recovery (migrate or resume-from-save — decided in M8 after Netcode spike; no silent choice).
- Client isolation: discovery fails → manual-IP path with diagnostics (own IP shown, ping check, port documented).
- Global entity budgets enforced so a full group can't melt phones (NET-007, PERF-004).

## 6. M8 spike plan (before full build)
1. Two Android phones, one as hotspot: Netcode hello-world + transform sync.
2. Broadcast discovery test; record which devices isolate clients.
3. Manual-IP join test; measure RTT/loss baseline.
4. Go/no-go on Netcode vs fallback (raw UnityTransport or lightweight socket layer) — documented, benchmarked.
