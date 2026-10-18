# Master Feature Registry

The authoritative registry is **`registry/feature_registry.json`** (machine-checked by `tools/validate_registry.py` in CI).

- Every spec requirement has a stable ID (`TECH-001`, `BOSS-002`, …), status, milestone, dependencies, multiplayer authority, save requirements, and acceptance criteria.
- Status rules: `planned` → `in-progress` → `implemented` → `tested` (evidence required) / `blocked`.
- Nothing is marked `tested` without verification evidence (spec §1 rules 7 & 10).
- Slipped work keeps its entry with a reason in `notes` — nothing is silently dropped (§37).
