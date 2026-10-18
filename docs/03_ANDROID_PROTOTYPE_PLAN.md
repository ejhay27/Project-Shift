# 03 — Android Prototype Plan (Milestone 1, first runnable)

Goal: the smallest **playable** slice — install APK → move → camera → harvest → inventory → save → reload.

## 1. Scene (generated, deterministic)
`unity/ProjectShift/Assets/Scripts/Editor/PrototypeSceneBuilder.cs`
Menu: **Project Shift > Build M1 Prototype Scene** → `Assets/Scenes/Prototype_M1.unity`
Contains: ground plane + grid of sample nodes (tree/stone/copper primitives), sun + hemisphere light, player capsule (CharacterController + `PlayerController`), `CameraRig`, HUD canvas (joystick, Jump, Interact, prompt, toast, Save), EventSystem, `GameBootstrap` + `WorldState` + `SaveManager`.

## 2. Controls (phone + tablet, landscape)
- Left: `TouchJoystick` (fixed or floating, adjustable size) → move.
- Right-half drag: orbit; pinch: zoom; mouse right-drag + wheel in Editor.
- Buttons: Jump, Interact (contextual), Save (HUD).
- Keyboards (Editor/PC path): WASD/arrows + Space + E via `KeyboardPlayerInput`.
- No gesture conflicts: joystick consumes left-zone touches; camera ignores touches starting on UI (EventSystem check).

## 3. Systems wired in M1
| System | Core | Game bridge | Test |
|---|---|---|---|
| Inventory | `InventorySystem` + `ItemDefinition` | `WorldState`, harvest flow | `InventoryTests` |
| Saves | `VersionedSave` envelope | `SaveManager`, `PlayerSaveData` | `VersionedSaveTests` |
| Nodes | `ResourceNodeSystem` (10-min rule) | `ResourceNodeBehaviour` | `ResourceNodeTests` |
| Wallet/clock/teleport/squad/guardians | written now as **locked rules** | integrated in their milestones | NUnit tests written now |

## 4. M1 task list (dev machine)
1. Open `unity/ProjectShift` in Unity 6 LTS → resolve packages → run EditMode tests (all green).
2. Run scene builder → press Play → verify movement/camera/harvest/save/load in Editor.
3. URP migration per `docs/04_RENDERING_PERFORMANCE_STRATEGY.md` + quality presets.
4. Android SDK setup per `docs/06_BUILD_ENVIRONMENT.md` → build APK → install on 1 phone + 1 tablet.
5. Fill M1 test sheet: movement, camera, harvest, inventory counts, save→kill→reload, 10-min node timer (shorten timer in a test build to verify quickly, then restore).
6. Record evidence (video/log) → flip registry entries to `tested` **only with evidence**.

## 5. Exit criteria (M1)
- [ ] APK installs and plays on real Android hardware.
- [ ] Player gathers resources; counts correct; no dupes on reload.
- [ ] Save survives app kill; corrupt-main falls back to backup (tested by fault injection).
- [ ] 30-min soak: no crash, no soft-lock, thermals sane on test devices.
- [ ] Registry updated with evidence links.
