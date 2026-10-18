# 04 — Rendering & Performance Strategy

## 1. Pipeline decision: URP (mobile-first)
- **Start:** built-in RP defaults so the skeleton opens anywhere (TECH-002).
- **M1 exit:** migrate to **Universal Render Pipeline** with three quality presets (TECH-005):

| Preset | Resolution scale | Shadows | Draw distance | Particles | Target |
|---|---|---|---|---|---|
| Low | 0.6–0.7 | off/blob | short | minimal | old phones (e.g. 3–4 GB RAM class, TBD by test) |
| Medium | 0.8–0.9 | 1 cascade 1024 | medium | reduced | mainstream phones |
| High | 1.0 | 2 cascades 2048 | long | full | flagships/tablets |

- Never auto-max: default = Medium, auto-drop on sustained thermal/frame-time breach (M4+).
- No invented min-spec: `PERF-005` documents it from real profiling only.

## 2. Art-direction guardrails (§3) that are also perf wins
- Bright day look via cheap gradient sky + fog, not volumetrics on Low/Medium.
- Water/foliage/tree/glow customs in HLSL only where a cheap Shader Graph/URP lit variant can't do it; every custom shader ships a Low fallback.
- Combat VFX: pooled, capped particle counts, intensity slider (AFFIN-004).

## 3. Simulation budgets (§34)
- **Sim tiers:** near = full; nearby = simplified (reduced tick); distant = summarized/scheduled; unloaded = persist + event math only (WORLD-001, PERF-002).
- **Object pooling** for projectiles, pickups, VFX, NPC/wildlife spawns — zero per-frame allocations in hot loops.
- **Region streaming** by distance with async loads; gameplay never blocks on streaming (M4).
- **Culling:** frustum always; occlusion where it pays (cities/interiors); LOD groups on creatures/trees/structures.
- **Physics:** layered collision matrix; minimal active rigidbodies; simple colliders for nodes/buildings.
- **Audio:** streamed music/VO, compressed banks, concurrent-voice caps (NPC-003).

## 4. Profiling gates
- M1: Editor + 1 device baseline (frame time, draw calls, tris, memory).
- M4: streaming + weather + director soak on min-class device.
- M8: MP bandwidth capture (PERF-004).
- M10: thermal/battery/memory matrix → min-spec doc (PERF-005).
