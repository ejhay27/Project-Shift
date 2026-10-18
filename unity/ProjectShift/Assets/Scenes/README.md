# Scenes

Scenes are **generated, not hand-edited YAML**, so they stay reproducible.

- Open the project in Unity 6, then run menu **Project Shift > Build M1 Prototype Scene**.
- Output: `Assets/Scenes/Prototype_M1.unity` (M1 playable slice: player, camera, HUD, sample nodes).
- The builder lives at `Assets/Scripts/Editor/PrototypeSceneBuilder.cs`.
- Never commit a hand-tweaked scene without also updating the builder (or documenting why it diverges).
