# 07 — Data & Save Formats

## 1. Game data (JSON, validated)
Location: `unity/ProjectShift/Assets/GameData/`. Validated by `tools/validate_data.py` in CI.

- `items.json` — `{ "items": [ { id, displayName, category, stackLimit, description } ] }`. IDs: lowercase `a-z0-9_` namespaced (`wood.oak`, `ore.copper`).
- `resource_nodes.json` — `{ "nodes": [ { nodeId, resourceId→items, yieldMin/Max, requiredToolTier, respawnMinutes, regionId } ] }`. Stone default `respawnMinutes: 10`.
- `recipes.json` — `{ "recipes": [ { recipeId, station, inputs[{itemId, count}], outputs[{itemId, count}] } ] }`.
- Later: loot tables, creatures, pets, abilities, dialogue, quests — same conventions (unique IDs, cross-refs validated).

ScriptableObjects later mirror these for editor workflows; JSON stays the diffable source of truth.

## 2. Save format (INV-004)
`Application.persistentDataPath/ProjectShift/save_slotN.json`:
```json
{
  "schemaVersion": 1,
  "gameVersion": "0.1.0-m1",
  "savedAtUtcTicks": 6390...,
  "payloadJson": "{ ...PlayerSaveData... }",
  "checksumSha256": "hex-of-payloadJson"
}
```
- **Write protocol:** serialize payload → checksum → write `save.json.tmp` → rotate `save.json` → `save.json.bak` → move tmp into place. Crash-safe: a valid save is never overwritten by a partial write.
- **Read protocol:** parse main → verify schema + checksum → on failure try `.bak` → on failure report corrupt (never silently start over a valid save).
- **Payload DTOs** use only JsonUtility-safe types (classes, Lists, primitives — no Dictionaries).
- Schema bumps require a migration path (INV-005, QA-002).
