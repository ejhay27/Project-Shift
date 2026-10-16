# PROJECT: SHIFT — Master Feature Registry

Every requirement from the Master Game Development Specification has an ID, status, milestone, dependencies, and acceptance criteria. Organized by the spec's section 37 groups.

**Status as of 2026-10-16 (Milestone 0 audit):** No gameplay code exists. Every row is **Planned** unless marked otherwise. Nothing is Implemented or Tested. Rows marked **Blocked** cannot proceed until the toolchain is available (see `02_ROADMAP.md`).

Columns: ID · Requirement · Milestone · Status · Depends on · Acceptance criteria (abbreviated; full text in the spec).

Multiplayer authority rule applies to every row that changes shared state: the host is authoritative; clients request, host validates.

## 1. World and narrative
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| WN-01 | Opening sequence as playable 3D cinematic (city, sofa, TV, glitch, curtains, window, Area 51 broadcast, portal, signal fail) | M9 (placeholder camera test M1) | Planned | IN-02, PQ-03 | Plays without slideshows; camera/light/sound transitions; replayable from settings; asset budget set for Android. |
| WN-02 | Narrative clues connected through discoverable evidence (plague, ancient civ, World Tree, relic tech, anomalies) | M4→M9 | Planned | MP-03, JR-01 | Evidence entries link to story flags; no full explanation given at once. |
| WN-03 | Ending: planetary departure, conclusion, credits; no sequel hook | M9 | Planned | ST-02 | Completable end-to-end; credits play; no hook required. |
| WN-04 | Dialogue and environmental storytelling hooks | M6 | Planned | NP-02 | Triggered from world state flags. |

## 2. Biomes and exploration
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| BI-01 | Multiple continents, forests, ancient forests, mountains, cliffs, caves, high routes | M4 (first), M9 (full) | Planned | WS-01 | Regions loadable and distinct; traversal reachable. |
| BI-02 | Jungles, volcanic highlands, frozen frontiers, grasslands, wilderness | M4→M9 | Planned | WS-01 | Each biome has unique resources, wildlife, hazards. |
| BI-03 | Rivers, lakes, oceans, islands, underground seas | M5→M9 | Planned | WA-01 | Swim/water rules applied. |
| BI-04 | Fallen megacities, abandoned neighborhoods, functional cities, underground stations/metro/shelters/labs/markets | M6→M9 | Planned | SN-01 | Interior and exterior rules consistent. |
| BI-05 | Underground kingdoms, forest settlements, ancient ruins, sealed vaults, hidden dungeons | M9 | Planned | BO-01 | Access requires specified conditions. |
| BI-06 | Dangerous boss-controlled territories | M3→M9 | Planned | BO-01 | See BO section. |
| BI-07 | Remote regions on distant parts of planet | M9 | Planned | WS-01 | Streamed on demand. |
| BI-08 | Each region unique resources, wildlife, landmarks, hazards, weather, settlements, discoveries | M4+ | Planned | BI-01 | Data-driven per region. |
| EX-01 | Climbing and traversal routes | M5 | Planned | CH-03 | Climb state with stamina cost. |
| EX-02 | Caves, cliffs, alternate paths, hidden passages, underground entrances | M4 | Planned | WS-01 | Reachable and persistent. |
| EX-03 | Searchable abandoned structures | M2 | Planned | SV-01 | Search yields loot once per respawn rule. |
| EX-04 | Discoverable landmarks and map markers | M4 | Planned | MP-01 | Discovery persisted. |
| EX-05 | Environmental puzzles and ancient mechanisms | M7 | Planned | DR-00 | Solve state persisted. |
| EX-06 | Hidden camps, labs, shelters, settlements | M5→M9 | Planned | SN-03 | Discoverable. |
| EX-07 | Dynamic points of interest | M4 | Planned | WD-01 | Spawn/expire rules. |
| EX-08 | Collectible discoveries and field journal | M4 | Planned | JR-01 | See JR section. |
| EX-09 | Region-specific rewards and secrets | M5+ | Planned | MP-02 | Not revealed by map tier alone. |
| EX-10 | Exploration achievements and progression | M4 | Planned | PG-01 | Awarded once, persisted. |
| EX-11 | Visible celestial objects (planets, moons) | M4 | Planned | WX-01 | Visible at night. |
| EX-12 | Dynamic world events tied to location, time, weather, progression | M4 | Planned | WD-01 | See WD. |

## 3. Plague and world simulation
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| WD-01 | World Director: manages events, environment, migration, threats, plague, simulations | M4 | Planned | WS-02 | Single service; host-authoritative. |
| WD-02 | Regional contamination levels | M4 | Planned | WD-01 | Per-region value persisted. |
| WD-03 | Wildlife behavior and population changes | M4 | Planned | WD-01 | Population summaries update offline. |
| WD-04 | Plants corrupt, mutate, recover, spread | M4 | Planned | WD-02 | Spread limited by budget. |
| WD-05 | NPC settlements react to danger | M6 | Planned | SN-03 | Reaction state changes. |
| WD-06 | Boss territories change with threats, food, player actions | M7 | Planned | BO-03 | Persisted. |
| WD-07 | Recovery via research, purification, restoration | M7 | Planned | WD-02 | Contamination decreases. |
| WD-08 | Anomalies affect relic tech or local rules | M7 | Planned | EL-01 | Rule modifiers applied per zone. |
| WD-09 | Persistent events where necessary; others temporary | M4 | Planned | WD-01 | Flag per event. |
| WD-10 | Offline simulation via efficient summaries | M4 | Planned | IN-01 | Elapsed-time summary; no per-entity catch-up. |
| WD-11 | Migrating giants, meteor showers, planetary alignments, plague surges, unusual weather, timed discoveries | M4→M7 | Planned | WD-01 | Each event has trigger, duration, effects, end. |
| WD-12 | Multiplayer hosts control authoritative events | M8 | Planned | LM-02 | Clients cannot trigger. |

## 4. Bosses and territorial creatures
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| BO-01 | Boss framework: health bar, phases, telegraphs, arenas | M3 | Planned | CB-01 | One repeatable test encounter. |
| BO-02 | Boss patterns, terrain interactions, environmental effects | M3→M7 | Planned | BO-01 | Per-boss data. |
| BO-03 | Territory change after permanent defeat (safer/ownership change) | M7 | Planned | WD-06 | No unique boss recreated without authorization. |
| BO-04 | Resistances and weaknesses | M3 | Planned | AF-02 | Damage multipliers. |
| BO-05 | Reward distribution (fair, anti-farm) | M3/M8 | Planned | EC-03, LM-05 | Per-participant rules. |
| BO-06 | Multiplayer boss sync | M8 | Planned | LM-02 | Host state. |
| BO-07 | Persistent defeat state where specified | M3 | Planned | IN-01 | Saved. |
| BO-08 | Controlled respawn rules per encounter | M3 | Planned | BO-07 | Table in data. |
| BO-09 | Example bosses: Thorn Crown, Mountain King, Deep Witness, Drownmaw (refinable) | M3→M9 | Planned | BO-01 | Each has unique pattern. |
| BO-10 | Cars and flying cars prohibited in active boss arenas | M6 | Planned | TR-03 | Blocked from entering arena. |
| BO-11 | Eligible pets/dragons may participate; pet count limit | M5 | Planned | PT-04 | Limit enforced. |
| BO-12 | Pet abilities cannot infinite stun-lock or trivialize bosses | M5 | Planned | PT-02, AM-03 | Stun diminishing returns; boss immunity windows. |
| BO-13 | Boss AI reacts to pets, player position, group | M5 | Planned | BO-01 | Target scoring test. |

## 5. World Tree
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| WT-01 | World Tree environment (majestic, magical, non-horror) | M7 | Planned | WS-01 | Art review; performance budget. |
| WT-02 | Worldheart Timber: rare building material | M7 | Planned | BB-01 | Controlled source. |
| WT-03 | Worldheart Crystal: rare crystal material | M7 | Planned | EC-03 | Controlled source. |
| WT-04 | Everbloom Leaves: rare ingredient | M7 | Planned | SC-03 | Controlled source. |
| WT-05 | Worldheart structures effectively indestructible to ordinary damage, creatures, fire, storms, plague, siege | M7 | Planned | BB-06 | Damage test returns zero. |
| WT-06 | Ownership/permissions preserved on Worldheart structures | M7 | Planned | LM-04 | Permission tests. |
| WT-07 | Unauthorized dismantling prevented | M7 | Planned | WT-06 | Rejected in test. |
| WT-08 | Placement validated; cannot bypass protected areas | M7 | Planned | BB-05 | Tests. |
| WT-09 | Scripted events may disable protection temporarily and restore afterward | M7 | Planned | WD-01 | Restored after event. |
| WT-10 | Guardian: unique territory, boss, health bar, animations, attacks, rewards | M7 | Planned | BO-01 | Full encounter. |
| WT-11 | Outcome A: defeat for boss rewards; mark defeated; never respawn; no pet form | M7 | Planned | IN-01, BO-07 | Test: second fight impossible; pet unavailable. |
| WT-12 | Outcome B: non-repeatable bonding quest; trust/exploration/progression conditions; pet form; joins five-pet squad; marked bonded; no boss loot | M7 | Planned | PT-01 | Test: loot not granted; boss unavailable. |
| WT-13 | Outcomes mutually exclusive; never both | M7 | Planned | WT-11, WT-12 | Invariant test on save and load. |
| WT-14 | Guardian never respawns in either outcome | M7 | Planned | WT-13 | Test across reload and reconnect. |
| WT-15 | Outcome persists authoritatively in multiplayer; prevents duplication via reload, reconnect, host exploit | M8 | Planned | LM-02 | Multiplayer tests. |
| WT-16 | World Tree appearance/resources reflect player decision | M7 | Planned | WT-13 | Visual and data state. |

## 6. Resource gathering and mining
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| RG-01 | Trees: species, health/harvest state, yields | M2 | Planned | IN-01 | Species data. |
| RG-02 | Wood types for building, furniture, crafting, fuel | M2 | Planned | RG-01 | Recipes reference types. |
| RG-03 | Tool-based harvesting with animations | M2 | Planned | TL-01 | Tool required. |
| RG-04 | Controlled, biome-aware regrowth; no unlimited generation | M2/M4 | Planned | WS-02 | Caps per region. |
| RG-05 | Stone nodes: ~10-minute respawn, continues while away or closed | M1 (prototype), M2 | Planned | IN-01 | UTC timestamp test. |
| RG-06 | Respawn never inside players, buildings, roads, or occupied locations | M2 | Planned | BB-05 | Overlap test. |
| RG-07 | Surface ores: copper, iron, coal, stone, regional minerals | M2 | Planned | RG-01 | Data-driven. |
| RG-08 | Underground ores: iron, silver, gold, rare crystals, ancient minerals, high-tier | M2→M7 | Planned | TL-02 | Tier gating. |
| RG-09 | Rarity, yield, location, regeneration rules per resource | M2 | Planned | DATA-01 | Data table. |
| RG-10 | Rare deposits respawn slower than common | M2 | Planned | RG-09 | Test per tier. |
| RG-11 | Tools have condition, durability, upgrade progression | M2 | Planned | TL-01 | Durability decreases. |
| RG-12 | Visual and audio feedback on gather | M1 | Planned | — | Prototype: visual; audio pass later. |
| RG-13 | Host-authoritative, deterministic resource state (multiplayer) | M8 | Planned | LM-02 | Tests. |

## 7. Salvage, looting, and recycling
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| SV-01 | Search houses, apartments, garages, basements, attics, cabinets, drawers, toolboxes, containers | M2 | Planned | IN-02 | Container state persisted. |
| SV-02 | Resource categories (fabric, wire, batteries, tools, scrap, components, electronics, alloys, sensors, power cells, science, blueprints, relic parts) | M2→M7 | Planned | DATA-01 | Data-driven loot tables. |
| SV-03 | Location-based loot (residential, junkyards, industrial, labs, ruins) | M2→M7 | Planned | SV-01 | Loot table per location type. |
| SV-04 | On-site dismantling, repair, restoration, recycling | M2 | Planned | BB-03 | Resource returns. |
| SV-05 | Controlled, location-aware regeneration | M4 | Planned | WS-02 | Cooldown per container. |
| SV-06 | NPC traders, manufacturers, industrial hubs, settlement services as additional sources | M6 | Planned | SN-04 | Trade tables. |

## 8. Survival, cooking, and effects
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| SC-01 | Configurable survival: hunger, thirst, rest, shelter, environmental resistance, health recovery | M2 | Planned | — | Presets: relaxed/standard/hard. |
| SC-02 | Food buffs: grilled fish, hearty stew, berry dish, mountain trail meal, ancient feast | M5 | Planned | SC-04 | Duration and icon shown. |
| SC-03 | Drinks: clean water, herbal tea, energy tonic, cooling drink, aether infusion | M5 | Planned | WA-02 | Same. |
| SC-04 | Effect stacking, refresh, replacement rules | M5 | Planned | SC-02 | Tests for each rule. |
| SC-05 | Cooking at campfires, kitchens, city restaurants | M5 | Planned | SN-06 | Station check. |
| SC-06 | Clear durations and UI indicators | M5 | Planned | UI-01 | Visible timer. |

## 9. Water systems
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| WA-01 | Water bodies with swimming rules | M5 | Planned | CH-03 | Swim state. |
| WA-02 | Rainwater collection: gutters, rain collectors, barrels, pipes, tanks | M5 | Planned | EL-02 | Flow simulation. |
| WA-03 | Filters and purification devices | M5 | Planned | WA-02 | Contamination removed. |
| WA-04 | Connected network tracking capacity, flow, contamination, storage consistently | M5 | Planned | WA-02 | Graph tests. |
| WA-05 | Rainfall and drought affect collection | M5 | Planned | WX-02 | Weather input. |
| WA-06 | Contaminated water needs treatment | M5 | Planned | WD-02 | Treated flag. |

## 10. Combat and equipment
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| CB-01 | Health, armor, stamina, recovery | M3 | Planned | — | Tests. |
| CB-02 | Modular weapons: handling, animation, sound, effects, stats, upgrades, condition | M3 | Planned | IN-01 | Data-driven. |
| CB-03 | Weapon crafting via fictional component system and benches | M3 | Planned | IN-03 | Recipe validation. |
| CB-04 | Repair and modify compatible weapons | M3 | Planned | CB-03 | Compatibility rules. |
| CB-05 | Responsive aiming and camera control; configurable aim sensitivity | M3 | Planned | UI-03 | Sensitivity setting. |
| CB-06 | Recoil, reloads, feedback, hit reactions, damage numbers, telegraphs | M3 | Planned | CB-02 | Visual check. |
| CB-07 | Melee and ranged combat; weapon switching; contextual actions | M3 | Planned | CB-02 | Input test. |
| CB-08 | Enemy weaknesses, resistances, elemental reactions | M3 | Planned | AF-02 | Multiplier tests. |
| CB-09 | Mobile controls usable while moving and aiming | M3 | Planned | UI-03 | Gesture arbitration tests. |
| CB-10 | Legendary weapons: SUNFALL, THUNDERJAW, LAST HORIZON, VOIDRENDER, GHOSTBRANCH, IRON REQUIEM | M5→M7 | Planned | CB-02 | Unique mechanics each. |
| CB-11 | Armor: AEGIS-9 EXOSUIT, WRAITH RUNNER, EDEN VEIL, CROWN OF ASH; distinct playstyles | M5 | Planned | CB-01 | Trade-offs in data. |

## 11. Ammunition and consumables
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| AM-01 | Ammo types: conventional, energy cells, heat-limited self-recharging, EMP, cryo, purification, ancient energy, tranquilizer, armor-piercing, nonlethal shock | M3→M7 | Planned | CB-02 | Fictional; no real-world instructions. |
| AM-02 | Ammo crafting as fictional mechanic | M3 | Planned | CB-03 | — |
| AM-03 | Crowd control has diminishing returns; cannot permanently disable powerful enemies | M5 | Planned | BO-12 | Test repeated CC. |
| AM-04 | Special bombs: Fire, EMP, Dark, Frost, Restoration | M5 | Planned | AF-01 | Area effects. |
| AM-05 | Bomb inventory limits, cooldowns, durations, target restrictions, boss resistance | M5 | Planned | AM-04 | Tests. |

## 12. Affinities
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| AF-01 | 16 affinities: Fire, Dark, Storm, Frost, Nature, Water, Earth, Light, Wind, Poison, Crystal, Gravity, Plasma, Sound, Ancient, Void | M3 (initial), M5+ | Planned | — | Data table. |
| AF-02 | Enemy resistance and weakness system | M3 | Planned | CB-08 | Multiplier tests. |
| AF-03 | Affinity workbench: apply, change, remove where permitted; catalysts and costs | M3 | Planned | IN-03 | Cost validation. |
| AF-04 | Weapon and armor compatibility rules | M3 | Planned | AF-03 | Tests. |
| AF-05 | Pet affinity compatibility | M5 | Planned | PT-01 | Tests. |
| AF-06 | Hybrid affinities (e.g., Storm+Frost) | M5 | Planned | AF-03 | Recipe-gated. |
| AF-07 | Mastery and progression | M5 | Planned | PG-01 | XP per affinity. |
| AF-08 | Visual/audio feedback; adjustable glow intensity | M3 | Planned | UI-04 | Quality setting. |
| AF-09 | Mobile-friendly effects; particle cap | M3 | Planned | PQ-02 | Budget test. |

## 13. Pets and abilities
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| PT-01 | Pet data: species, rarity, appearance, animations, health, energy, abilities, traits, bond, mastery, recovery, habitat, behavior, optional evolution | M3 (basic), M5 (full) | Planned | DATA-01 | Pet record in save. |
| PT-02 | Rarity tiers I–IX (Common…Transcendent) | M3 | Planned | — | Enum + data. |
| PT-03 | Ability collection capacity by rarity: I=2, II=3, III=4, IV=5, V=6, VI=7, VII=8, VIII=10; IX (Transcendent) capacity not stated in spec | M5 | Planned | PT-02 | Capacity test. IX value needs a balance decision; values are initial targets subject to balance testing. |
| PT-04 | Five-pet active journey squad per player; combat limit | M5 | Planned | LM-05 | Limit tests. |
| PT-05 | Player-usable vs pet-exclusive ability categories | M5 | Planned | PT-06 | Classification in data. |
| PT-06 | Inherited ability rule (decision D3 proposed in `01_ARCHITECTURE_DECISION.md`) | M5 | Planned — **needs owner sign-off** | — | Documented before implementation. |
| PT-07 | Player ability loadout separate from pet loadout and squad | M5 | Planned | PT-06 | UI separation. |
| PT-08 | Cooldowns and energy costs per ability | M5 | Planned | PT-01 | Data + tests. |
| PT-09 | Example Ancient pet "Aetherfang" with 10 abilities (5 shared, 5 pet-only as listed) | M5 | Planned | PT-05 | Each ability implemented individually before marked complete. |
| PT-10 | Direct pet control with camera follow; player remains in world and may be vulnerable | M5 | Planned | PT-01, CH-03 | Switch back test. |
| PT-11 | Pet commands: Follow, Wait, Return, Guard, Scout, Attack, Move to target | M3 (basic), M5 | Planned | PT-01 | Each command tested. |
| PT-12 | Home guardians: perimeter patrol, gate guardian, scout, defender, resource guardian, support, resting | M5 | Planned | PT-13 | Post-return behavior. |
| PT-13 | Journey squad vs home assignment swap; restore prior assignment | M5 | Planned | PT-12 | Test. |
| PT-14 | Home guardians do not attack authorized teammates | M5 | Planned | LM-04 | Test. |
| PT-15 | Pets have health, energy, recovery, habitat needs | M5 | Planned | HB-01 | Test. |
| PT-16 | Pet collection persists and cannot be duplicated | M5 | Planned | IN-01 | Test. |

## 14. Dragons and skywalking
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| DR-00 | Rideability eligibility by species, bonding, size, equipment — not rarity | M5 | Planned | PT-01 | Eligibility tests. |
| DR-01 | Mount, dismount, takeoff, landing, 3D flight, hover, ascend/descend | M5 | Planned | DR-00 | Mobile control test. |
| DR-02 | Flight stamina/energy; aerial abilities where supported | M5 | Planned | PT-08 | Drain test. |
| DR-03 | Collision and landing validation | M5 | Planned | DR-01 | Tests. |
| DR-04 | Skywalk pets: magical glowing pawprints, ravine traversal, high ruins access | M5 | Planned | DR-00 | Path validation. |
| DR-05 | Skywalking is distinct from flying | M5 | Planned | DR-04 | Separate movement states. |

## 15. Base building
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| BB-01 | Tier I: wood/scrap walls, floors, roofs, doors, beds, storage, workbenches, lighting | M2 | Planned | IN-03 | Placement tests. |
| BB-02 | Tier II: metal walls/doors, towers, gates, defenses | M4→M5 | Planned | BB-01 | — |
| BB-03 | Tier III: advanced barriers, contamination control, workshops, power rooms, advanced security | M5→M7 | Planned | EL-01 | — |
| BB-04 | Tier IV: bunkers, research labs, pet habitats, ammo workshops, ancient chambers | M7 | Planned | BB-03 | — |
| BB-05 | Snapping, grid, rotation, placement preview, collision validation, overlap prevention | M2 | Planned | — | Overlap tests. |
| BB-06 | Upgrade, repair, relocate, dismantle with resource recovery | M2 | Planned | BB-01 | Round-trip cost test. |
| BB-07 | Material durability, cost, weaknesses | M2 | Planned | BB-01 | Data. |
| BB-08 | Ownership and access permissions | M2 | Planned | LM-04 | Permission test. |
| BB-09 | Persistent saves; multiplayer sync | M2 / M8 | Planned | IN-01 | Reload test. |

## 16. Furniture and doors
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| FD-01 | Functional furniture: beds, wardrobes, bookshelves, desks, cabinets, storage, trophy displays, kitchen counters, refrigerators, dining, sofas, lamps, pet beds, crafting benches, research desks, charging, medical, equipment racks | M2→M7 | Planned | BB-01 | Interaction per item. |
| FD-02 | Placement, rotation, relocation, interaction animations, material/color customization | M2 | Planned | BB-05 | — |
| FD-03 | Door types: sliding, bookcase secret, hinged, barn, folding, glass, reinforced, keycard/keypad, pressure-sealed, powered, ancient puzzle | M2→M7 | Planned | EL-03 | Collision, animation, sound, permission tests. |
| FD-04 | Auto or manual operation where appropriate | M2 | Planned | FD-03 | Setting per door. |

## 17. Electricity and automated defense
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| EL-01 | Components: wires, switches, fuses, connectors, batteries, generators, solar, circuit boards, sensors, relays, panels, motors, converters, energy cells, ancient cores | M5→M7 | Planned | BB-01 | Graph model. |
| EL-02 | Power network tracking; generator fuel; finite battery capacity; damaged cables interrupt power | M5 | Planned | EL-01 | Graph tests. |
| EL-03 | Switches/relays control devices; sensors detect events; powered doors respond to power loss with fallback | M5 | Planned | EL-02 | Power-loss test. |
| EL-04 | Powered equipment: lights, doors, pumps, turrets, fences, workshops, habitats, teleport devices, relic equipment | M5→M7 | Planned | EL-02 | Each consumes power. |
| EL-05 | Inspect and troubleshoot systems | M5 | Planned | UI-05 | Diagnostics panel. |
| EL-06 | Automated turrets, barriers, fences/gates, alarms, floodlights, sensors, defense drones | M5 | Planned | EL-02 | Target rules. |
| EL-07 | Defense target rules, friendly-fire protection, patrol zones, ammo limits, power use, damage states, alert behavior | M5 | Planned | EL-06 | Tests. |
| EL-08 | Defenses run out of ammo/power, get damaged, or have sensors disrupted | M5 | Planned | EL-07 | Tests. |
| EL-09 | Host synchronizes power state | M8 | Planned | LM-02 | Multiplayer test. |

## 18. Teleportation
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| TP-01 | Wayfarer Relic: placement on clear eligible location; unfold, beam, relay registration, activation | M5 | Planned | EL-02 | Full sequence. |
| TP-02 | Beam visible in sky; relay visual response | M5 | Planned | PQ-03 | Visual check. |
| TP-03 | Discovered, active destinations via network | M5 | Planned | TP-01, MP-01 | Destination test. |
| TP-04 | Waygates: scattered, some immediate, some requiring restoration/power/quest | M6→M7 | Planned | TP-03 | Unlock conditions. |
| TP-05 | Must physically discover before fast travel | M5 | Planned | EX-04 | Test. |
| TP-06 | Successful teleport triggers 5-minute cooldown; host enforces in multiplayer | M5 / M8 | Planned | LM-02 | Cooldown test. |
| TP-07 | Leave and rejoin does not reset cooldown; failed attempts don't consume cooldown | M8 | Planned | TP-06 | Reconnect test. |
| TP-08 | Destination validity: discovered, active, safe; relay disruption events | M5 | Planned | WD-01 | Tests. |
| TP-09 | Preserve inventory and pet state; prevent duplication, item loss, invalid geometry | M5 | Planned | IN-01 | Tests. |
| TP-10 | Teleport is distinct from vehicles and dragon riding | M5 | Planned | — | — |

Note: spec says treat the 5-min cooldown as a network/device system rule unless ownership model approved. Current proposal: host-authoritative in multiplayer, device-local in single-player. Needs owner approval for the single-player ownership model.

## 19. Sanctuary Cities and NPCs
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| SN-01 | Protected Sanctuary Cities with relic barriers, gate scanning, security | M6 | Planned | EL-06 | Entry tests. |
| SN-02 | Barrier disruption events; underground tunnels and labs remain dangerous | M6 | Planned | WD-11 | — |
| SN-03 | City facilities: dealerships, subway/elevated, hospitals, research, markets, hotels, residential, crafting vendors, repair garages, pet care, relic research, vaults | M6 | Planned | SN-01 | Each facility has interaction. |
| SN-04 | Markets and traders | M6 | Planned | EC-01 | Buy/sell tests. |
| SN-05 | Hotels and residential districts | M6 | Planned | SN-03 | Rest/lodging. |
| SN-06 | Crafting vendors, repair garages | M6 | Planned | IN-03 | Services. |
| NP-01 | NPC types: farmers, engineers, scientists, guards, traders, explorers, families, faction members, settlement workers | M6 | Planned | — | Data. |
| NP-02 | Routines, motivations, reactions, dialogue choices, trading, crafting, repair, event responses | M4→M6 | Planned | WX-01 | Routine test. |
| NP-03 | NPC voice dialogue from stored assets; ambient conversations; subtitles | M6 | Planned | AU-01 | Audio loads. |
| NP-04 | Reusable audio assets; mobile loading and memory management | M6 | Planned | PQ-02 | Memory budget. |
| NP-05 | No live AI voice generation required per conversation | M6 | Planned | NP-03 | Design rule. |

## 20. City Guardians and Relic Beasts
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| GD-01 | City Guardians (dragon, six-legged beast, gravity creature) | M6 | Planned | BO-01 | Encounter. |
| GD-02 | Trial tiers: Trial (Uncommon–Rare), Elite (Epic–Legendary), Ancient Awakening (Mythic–Ancient) | M6 | Planned | PT-02 | Eligibility check. |
| GD-03 | Authorized challenge temporarily defeats Guardian (dormant, not killed) | M6 | Planned | GD-01 | State test. |
| GD-04 | Loot per encounter rules | M6 | Planned | EC-03 | Anti-farm. |
| GD-05 | Resurrection after ~10 min with visible countdown | M6 | Planned | IN-01 | Timer test across app restart. |
| GD-06 | City remains protected during Guardian downtime | M6 | Planned | SN-01 | Test. |
| GD-07 | Unauthorized attack triggers city defense response | M6 | Planned | SN-01 | Test. |
| GD-08 | Host controls Guardian health, challenge state, loot, resurrection | M8 | Planned | LM-02 | Tests. |
| GD-09 | Relic Beasts: City Guardian, Ancient Watcher, Relic Beast, dormant colossus, tamed Relic Beast | M6→M9 | Planned | GD-01 | Roles in data. |
| GD-10 | Some Relic Beasts story-only (not collectible); some rare pets via quests | M6 | Planned | PT-01 | Flag. |
| GD-11 | Relic Beasts react to Ancient pets, Wayfarer Relic, discoveries; unlock districts/quests | M6 | Planned | TP-01 | Flag-based. |

## 21. Transportation and vehicles
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| TR-01 | Subway networks, stations, shelters, underground settlements; some lines require repair | M6 | Planned | SN-03 | Line state. |
| TR-02 | Elevated trains/maglev: schedules, stations, boarding | M6 | Planned | TR-01 | Schedule test. |
| TR-03 | Vehicle classes: standard, off-road, utility, luxury, armored, flying, relic-powered | M6 | Planned | IN-01 | Data. |
| TR-04 | Vehicle entry/exit, interiors, doors, seats, passengers, dashboards, storage | M6 | Planned | TR-03 | Interaction tests. |
| TR-05 | Fuel, battery, or fictional energy; repair; paint and customization | M6 | Planned | TR-03 | Tests. |
| TR-06 | Parking garages; persistent ownership; multiplayer sync | M6 / M8 | Planned | LM-04 | Ownership survives restart. |
| TR-07 | Flying cars: flight controls, altitude limit, collision, energy | M6 | Planned | TR-03 | Tests. |
| TR-08 | Dealerships: inspect, compare, buy, customize; ownership retained | M6 | Planned | SN-03 | Purchase test. |
| TR-09 | Unauthorized use prevented where permissions require | M6 | Planned | TR-06 | Test. |
| TR-10 | Luxury vehicles: prestige and customization, not strongest survival | M6 | Planned | TR-08 | Balance. |
| TR-11 | Vehicles and flying cars prohibited in active boss arenas | M6 | Planned | BO-10 | Test. |

## 22. Ancient world-scale weapons
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| AW-01 | SKYBREAKER (ground-built orbital weapon; Aether Oil) | M7+ | Planned | AW-00 | Full project pipeline. |
| AW-02 | TITANFALL (mountain-scale pulse siege) | M7+ | Planned | AW-00 | — |
| AW-03 | AEGIS CROWN (shield array) | M7+ | Planned | AW-00 | — |
| AW-04 | STORMCALLER (weather engine) | M7+ | Planned | WX-02 | — |
| AW-05 | EDEN ENGINE (ecosystem restoration) | M7+ | Planned | WD-07 | — |
| AW-06 | WORLDSONG (resonance tower) | M7+ | Planned | WD-08 | — |
| AW-07 | GRAVEMIND ZERO (gravity engine) | M7+ | Planned | AF-01 | — |
| AW-08 | FROSTVEIL (cryo suppression) | M7+ | Planned | WD-11 | — |
| AW-09 | DEVOURER ARRAY (plague containment, energy storage) | M7+ | Planned | WD-07 | — |
| AW-10 | VOIDGATE (portal to sealed regions) | M7+ | Planned | TP-01 | — |
| AW-11 | OMNISIGHT (planetary observation) | M7+ | Planned | EX-11 | — |
| AW-12 | NULL CROWN (changes selected world rules) | M7+ | Planned | WD-08 | — |
| AW-00 | Shared project system: blueprint discovery, research, rare materials, bosses/dungeons, construction, power/maintenance, world-event consequences, multiplayer permissions, balance | M7 | Planned | EL-01, RS-01 | Framework tested with one project first. |

## 23. Character customization
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| CH-01 | Face, features, skin tone, hair, eyes, body presets, proportions | M6 | Planned | — | Preset save. |
| CH-02 | Clothing, armor, masks, backpacks, gloves, boots, accessories; color customization | M6 | Planned | CH-01 | Visual layer. |
| CH-03 | Movement, climbing, swimming, combat, vehicle, mount animations | M1 (walk), M5+ | Planned | — | Animator states. |
| CH-04 | Names and player profiles | M1 | Planned | IN-01 | Name persists. |
| CH-05 | Wardrobes, mirrors, city shops for appearance; cosmetics separate from stats | M6 | Planned | CH-02 | Separation test. |

## 24. Progression
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| PG-01 | Single coherent progression framework: combat, survival, engineering, crafting, exploration, affinity mastery, pet bonding, research, upgrades | M3→M5 | Planned | — | One XP/skill system; no overlapping currencies. |
| PG-02 | Reward exploration, experimentation, survival, crafting, combat, world interaction | M4+ | Planned | PG-01 | Design review. |

## 25. Map and field journal
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| MP-01 | Map Tier 1 Local | M1 (basic), M4 | Planned | — | Shows nearby POIs. |
| MP-02 | Map Tiers 2–5 (Regional, Explorer's, Continental, World) with discovery restrictions | M4 | Planned | MP-01 | Coverage expands; secrets not auto-revealed. |
| MP-03 | Map upgrades from cartographers, quests, crafting, observatories, exploration, rare rewards | M4 | Planned | MP-02 | Source test. |
| MP-04 | Overlays: boss territories, dangers, resources, settlements, transport, teleports, player markers, quests, landmarks, unexplored | M4→M7 | Planned | MP-02 | Toggle test. |
| MP-05 | Confirmed vs estimated vs unexplored information distinguished | M4 | Planned | MP-02 | Visual key. |
| JR-01 | Field journal entries: plants, fish, wildlife, NPCs, bosses, ores, regions, ancient tech, events, plague | M4 | Planned | EX-08 | Entries with confirmed/estimated flags. |
| JR-02 | Entries include uses, weaknesses, habitat, discovery location | M4 | Planned | JR-01 | Data. |
| JR-03 | Journal rewards exploration without revealing undiscovered secrets | M4 | Planned | JR-01 | Test. |

## 26. Tutorials and Settings
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| TU-01 | Tutorial Library in Settings, replayable | M2 (initial), M10 (full) | Planned | UI-01 | Replay test. |
| TU-02 | Topics: movement/camera, touch aiming, gathering, crafting, building, electricity, pets, maps, vehicles, teleport, multiplayer, advanced survival | M2→M9 | Planned | — | Each topic exists. |
| TU-03 | Short text, visual demos, touch diagrams, optional practice | M2+ | Planned | TU-01 | Review. |
| TU-04 | Pause, replay, disable reminders | M2 | Planned | TU-01 | Setting. |
| TU-05 | Adapts to phone and tablet | M2 | Planned | UI-02 | Layout tests. |

## 27. Weather, time, and celestial
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| WX-01 | Continuous day/night: morning, day, afternoon, dusk, night; sun, moon, stars, planets | M4 | Planned | — | Time-of-day test. |
| WX-02 | Weather: rain, storms, fog, snow, heat, supernatural — region-aware | M4 | Planned | WD-02 | Region tests. |
| WX-03 | Time affects light, fog, NPC schedules, wildlife, city lights, events, resources | M4 | Planned | WX-01 | Hooks. |
| WX-04 | Sleep/rest advances time where appropriate | M2 | Planned | SC-01 | Rest test. |
| WX-05 | Host controls authoritative time and events in multiplayer | M8 | Planned | LM-02 | Test. |
| WX-06 | Mobile-efficient local weather effects | M4 | Planned | PQ-02 | Budget. |

## 28. Pet habitats and base life
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| HB-01 | Pet beds, resting areas, specialized habitats, feeding/recovery | M5 | Planned | FD-01 | Recovery test. |
| HB-02 | Assigned guardians, patrol routes, return-to-post, player interaction | M5 | Planned | PT-12 | Tests. |
| HB-03 | Base-assigned pets persist when player leaves region; efficient off-screen simulation | M5 | Planned | WD-10 | Offline test. |

## 29. Economy
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| EC-01 | Shards (SHD): everyday currency; sources and sinks defined | M2→M6 | Planned | — | Ledger tests. |
| EC-02 | Credits (CR): Sanctuary City purchases, vehicles, services | M6 | Planned | EC-01 | Sink tests. |
| EC-03 | Aether (AET): rare energy for relic tech and high-tier rewards; boss rare chance | M7 | Planned | BO-05 | Rate limits. |
| EC-04 | Anti-exploit: no infinite loops, no duplicated rewards | M3→M10 | Planned | LM-05 | Tests. |
| EC-05 | Prices, rewards, costs configurable and balance-tested | M2+ | Planned | DATA-01 | Data files. |

## 30. Inventory, crafting, persistence
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| IN-01 | Inventory: items, stacks, equipment, materials, consumables, weapons, ammo, relics, pet items, quest items, currency, storage | M1 | Planned | DATA-01 | Stack tests. |
| IN-02 | Sorting, filtering, tooltips, comparison, contextual actions | M2 | Planned | UI-01 | UI tests. |
| IN-03 | Crafting validates ingredients, stations, permissions, consumption | M2 | Planned | IN-01 | Tests. |
| IN-04 | Storage containers with permissions | M2 | Planned | IN-01 | Tests. |
| IN-05 | Save everything listed in spec section 31 | M1→M10 | Planned | SAVE-01 | Round-trip per category. |
| SAVE-01 | Versioned save, validation, recovery, backups; never silently overwrite valid save | M1 | Planned | — | Interrupted-write and corrupt-file tests. |
| IN-06 | Player settings persisted | M1 | Planned | SAVE-01 | — |

## 31. Local multiplayer
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| LM-01 | Host a world; join via local discovery or manual IP | M8 | Planned — spike **Blocked** | UI-01 | Two-device test. |
| LM-02 | Host-authoritative simulation (world, time, AI, bosses, loot, building, nodes, pets, vehicles, cooldowns, guardian, events) | M8 | Planned | NET-01 | Authority tests. |
| LM-03 | Join/leave handling; player state sync; world-state sync | M8 | Planned | LM-01 | Two-device test. |
| LM-04 | Permission-controlled building, storage, vehicle ownership, pet permissions | M8 | Planned | BB-08 | Permission tests. |
| LM-05 | Boss participation and reward distribution fair; five-pet limit per player | M8 | Planned | BO-05 | Tests. |
| LM-06 | Reconnection handling; host-disconnect behavior; clear status and errors | M8 | Planned | LM-03 | Disconnect tests. |
| LM-07 | Duplicate prevention for rewards, boss kills, cooldowns | M8 | Planned | LM-02 | Abuse tests. |
| LM-08 | Global entity and combat budgets | M8 | Planned | PQ-02 | Cap tests. |
| NET-01 | Networking library chosen by spike (Netcode for GameObjects or LiteNetLib) | M8 spike | Planned — **Blocked** | — | Decision record. |

## 32. Android UI and accessibility
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| UI-01 | Main menu, character select, settings, inventory, crafting, equipment, pet collection/abilities, map, journal, quests, build, power, vehicles, tutorials, multiplayer, save feedback, error messages | M1→M9 | Planned | — | Each screen reachable and functional. |
| UI-02 | Responsive landscape layouts for phone and tablet; safe areas | M1 | Planned | — | Three aspect ratios. |
| UI-03 | Touch: joystick, camera swipe, buttons, combat, aim; gesture ownership; adjustable layout/sensitivity | M1 (move/look), M3 (combat) | Planned | — | Arbitration tests. |
| UI-04 | Graphics settings, resolution scale, effects quality, draw distance, shadows, frame rate | M1 (basic), M4 | Planned | PQ-01 | Presets apply. |
| UI-05 | Contextual buttons; no clutter; readable text; touch targets ≥ 48 dp | M1→M3 | Planned | — | Review. |
| UI-06 | Audio settings | M1 | Planned | — | Volume saved. |
| UI-07 | Controls and sensitivity settings | M1 | Planned | UI-03 | Persisted. |

## 33. Performance, quality assurance, and release
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| PQ-01 | Quality presets with graceful fallback | M4 | Planned | UI-04 | Preset test on device. |
| PQ-02 | Pooling, LOD, culling, streaming, budgets, AI tiers | M1→M4 | Planned | — | Profiler evidence. |
| PQ-03 | Async loading without unnecessary blocking | M4 | Planned | WS-01 | Frame-time test. |
| PQ-04 | Device profiling: fps, thermal, battery, memory on real Android devices | M1 onward | **Blocked** (no device/toolchain) | — | Log in `docs/05_DEVICE_TEST_LOG.md`. |
| PQ-05 | Documented minimum device spec from testing | M10 | Planned | PQ-04 | Document. |
| PQ-06 | Save recovery after interruption | M1 | Planned | SAVE-01 | Test. |
| PQ-07 | Crash and stability tests | M10 | Planned | — | Test log. |
| PQ-08 | Build signing and release configuration | M10 | Planned | — | Release keystore owner-managed. |
| PQ-09 | Target API 36 for store builds | M10 | Planned | — | Build settings check. |
| PQ-10 | Final feature audit | M10 | Planned | — | Registry update. |

## 34. Cross-cutting data
| ID | Requirement | M | Status | Depends | Acceptance |
|---|---|---|---|---|---|
| DATA-01 | ScriptableObject and JSON content pipeline with stable IDs | M1 | Planned | — | Import test. |
| WS-01 | Region streaming | M4 | Planned | PQ-02 | Load/unload test. |
| WS-02 | Simulation tiers (Active, Nearby, Distant, Unloaded) | M4 | Planned | WS-01 | Tier tests. |
| TL-01 | Tool system with condition and tiers | M2 | Planned | RG-11 | — |
| TL-02 | Tool tier gating for deposits | M2 | Planned | TL-01 | — |
| RS-01 | Rare materials and specialized components | M5→M7 | Planned | RG-08 | — |
| AU-01 | Audio streaming and mobile memory budget | M6 | Planned | PQ-02 | — |
| IN-02b | Ancient technology chambers and relic research | M7 | Planned | BB-04 | — |

## Summary

- Implemented: 0 · Tested: 0 · In progress: M0 documentation only · Blocked: PQ-04, NET-01 spike, Android build pipeline (M0 exit)
- Requirements tracked: see IDs above. Any requirement in the spec not listed here must be added before the next registry update. This registry is a planning artifact; its statuses must change only with build or test evidence.
