using ProjectShift.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectShift.Editor
{
    /// <summary>
    /// Builds the M1 prototype scene deterministically
    /// (menu: Project Shift &gt; Build M1 Prototype Scene).
    /// Scenes stay reproducible: re-running regenerates from scratch.
    /// </summary>
    public static class PrototypeSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Prototype_M1.unity";
        private const string ItemsPath = "Assets/GameData/items.json";
        private const string NodesPath = "Assets/GameData/resource_nodes.json";

        [MenuItem("Project Shift/Build M1 Prototype Scene")]
        public static void Build()
        {
            Scene scene = EditorSceneManager.NewScene(
                NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "Prototype_M1";

            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            BuildLights();
            BuildGround();
            PlayerController player = BuildPlayer();
            CameraRig rig = BuildCamera(player.transform);
            MobileHud hud = BuildHud(font, out TouchJoystick joystick);
            BuildSampleNodes();
            BuildSystems(font, player, rig, hud, joystick);

            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorUtility.DisplayDialog("Project Shift",
                "M1 prototype scene built:\n" + ScenePath, "OK");
        }

        // ---------- world ----------

        private static void BuildLights()
        {
            var sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.2f;
            sun.color = new Color(1f, 0.96f, 0.9f);
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.55f, 0.6f, 0.68f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.75f, 0.85f, 0.95f);
            RenderSettings.fogDensity = 0.004f;
        }

        private static void BuildGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.localScale = new Vector3(10f, 1f, 10f);
            var renderer = ground.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.35f, 0.62f, 0.32f);
            renderer.sharedMaterial = mat;
        }

        private static PlayerController BuildPlayer()
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = "Player";
            go.transform.position = new Vector3(0f, 1f, 0f);
            Object.DestroyImmediate(go.GetComponent<Collider>());
            var cc = go.AddComponent<CharacterController>();
            cc.height = 2f;
            cc.radius = 0.4f;
            cc.center = new Vector3(0f, 0f, 0f);
            var renderer = go.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.25f, 0.5f, 0.95f);
            renderer.sharedMaterial = mat;

            var controller = go.AddComponent<PlayerController>();
            go.AddComponent<TouchPlayerInput>();
            go.AddComponent<KeyboardPlayerInput>();
            return controller;
        }

        private static CameraRig BuildCamera(Transform target)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.farClipPlane = 300f;
            var rig = go.AddComponent<CameraRig>();
            rig.SetTarget(target);
            go.transform.position = new Vector3(0f, 3f, -6f);
            return rig;
        }

        // ---------- sample harvest nodes (ids match resource_nodes.json) ----------

        private static void BuildSampleNodes()
        {
            var root = new GameObject("SampleNodes").transform;
            MakeNode(root, "tree.oak.01", new Vector3(4f, 0f, 6f),
                PrimitiveType.Cylinder, new Color(0.45f, 0.28f, 0.12f), 3f);
            MakeNode(root, "tree.oak.02", new Vector3(-5f, 0f, 8f),
                PrimitiveType.Cylinder, new Color(0.45f, 0.28f, 0.12f), 3f);
            MakeNode(root, "stone.01", new Vector3(7f, 0f, -3f),
                PrimitiveType.Sphere, new Color(0.55f, 0.56f, 0.6f), 3f);
            MakeNode(root, "stone.02", new Vector3(-6f, 0f, -5f),
                PrimitiveType.Sphere, new Color(0.55f, 0.56f, 0.6f), 3f);
            MakeNode(root, "ore.copper.01", new Vector3(2f, 0f, -8f),
                PrimitiveType.Cube, new Color(0.85f, 0.45f, 0.2f), 3f);
        }

        private static void MakeNode(Transform root, string nodeId, Vector3 pos,
            PrimitiveType shape, Color color, float radius)
        {
            var go = new GameObject("Node_" + nodeId);
            go.transform.SetParent(root);
            go.transform.position = pos;

            var full = GameObject.CreatePrimitive(shape);
            full.name = "Full";
            full.transform.SetParent(go.transform);
            full.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            full.transform.localScale = Vector3.one * 1.5f;
            var mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            full.GetComponent<Renderer>().sharedMaterial = mat;
            Object.DestroyImmediate(full.GetComponent<Collider>());

            var depleted = GameObject.CreatePrimitive(PrimitiveType.Cube);
            depleted.name = "Depleted";
            depleted.transform.SetParent(go.transform);
            depleted.transform.localPosition = new Vector3(0f, 0.15f, 0f);
            depleted.transform.localScale = new Vector3(1f, 0.3f, 1f);
            var dmat = new Material(Shader.Find("Standard"));
            dmat.color = new Color(0.2f, 0.2f, 0.22f);
            depleted.GetComponent<Renderer>().sharedMaterial = dmat;
            depleted.SetActive(false);
            Object.DestroyImmediate(depleted.GetComponent<Collider>());

            var behaviour = go.AddComponent<ResourceNodeBehaviour>();
            var f = typeof(Interactable).GetField("_radius",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            f.SetValue(behaviour, radius);
            var idField = typeof(ResourceNodeBehaviour).GetField("_nodeId",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            idField.SetValue(behaviour, nodeId);
            var fullField = typeof(ResourceNodeBehaviour).GetField("_fullVisual",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            fullField.SetValue(behaviour, full);
            var depField = typeof(ResourceNodeBehaviour).GetField("_depletedVisual",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            depField.SetValue(behaviour, depleted);
        }

        // ---------- HUD ----------

        private static MobileHud BuildHud(Font font, out TouchJoystick joystick)
        {
            // EventSystem.
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();

            // Canvas.
            var canvasGo = new GameObject("HUD");
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();
            RectTransform root = canvasGo.GetComponent<RectTransform>();

            // Prompt (top-center).
            Text prompt = MakeText(root, "Prompt", font, 28,
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -40f), new Vector2(600f, 50f),
                TextAnchor.MiddleCenter, Color.white);
            prompt.gameObject.SetActive(false);

            // Toast (bottom-center).
            Text toast = MakeText(root, "Toast", font, 24,
                new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 120f), new Vector2(700f, 44f),
                TextAnchor.MiddleCenter, Color.yellow);
            toast.gameObject.SetActive(false);

            // Interact button (right, above jump).
            Button interact = MakeButton(root, "InteractButton", font, "INTERACT",
                new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(-140f, 250f), new Vector2(200f, 90f));
            interact.gameObject.SetActive(false);

            // Jump button (right-bottom).
            Button jump = MakeButton(root, "JumpButton", font, "JUMP",
                new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(-140f, 140f), new Vector2(200f, 90f));

            // Save button (top-right).
            Button save = MakeButton(root, "SaveButton", font, "SAVE",
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-100f, -40f), new Vector2(150f, 70f));

            // Joystick (left-bottom).
            joystick = MakeJoystick(root);

            var hud = canvasGo.AddComponent<MobileHud>();
            SetField(hud, "_promptLabel", prompt);
            SetField(hud, "_toastLabel", toast);
            SetField(hud, "_interactButton", interact);
            SetField(hud, "_jumpButton", jump);
            SetField(hud, "_saveButton", save);
            return hud;
        }

        private static Text MakeText(RectTransform parent, string name, Font font,
            int size, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos,
            Vector2 sizeDelta, TextAnchor align, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            var text = go.AddComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.alignment = align;
            text.color = color;
            text.raycastTarget = false;
            return text;
        }

        private static Button MakeButton(RectTransform parent, string name, Font font,
            string label, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPos, Vector2 sizeDelta)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = sizeDelta;
            var image = go.AddComponent<Image>();
            image.color = new Color(0.1f, 0.12f, 0.16f, 0.75f);
            var button = go.AddComponent<Button>();
            MakeText(rt, "Label", font, 24,
                new Vector2(0f, 0f), new Vector2(1f, 1f),
                Vector2.zero, Vector2.zero,
                TextAnchor.MiddleCenter, Color.white).text = label;
            return button;
        }

        private static TouchJoystick MakeJoystick(RectTransform parent)
        {
            var baseGo = new GameObject("Joystick");
            baseGo.transform.SetParent(parent, false);
            var baseRt = baseGo.AddComponent<RectTransform>();
            baseRt.anchorMin = new Vector2(0f, 0f);
            baseRt.anchorMax = new Vector2(0f, 0f);
            baseRt.anchoredPosition = new Vector2(160f, 160f);
            baseRt.sizeDelta = new Vector2(220f, 220f);
            var baseImg = baseGo.AddComponent<Image>();
            baseImg.color = new Color(1f, 1f, 1f, 0.25f);

            var knobGo = new GameObject("Knob");
            knobGo.transform.SetParent(baseRt, false);
            var knobRt = knobGo.AddComponent<RectTransform>();
            knobRt.sizeDelta = new Vector2(100f, 100f);
            var knobImg = knobGo.AddComponent<Image>();
            knobImg.color = new Color(1f, 1f, 1f, 0.6f);
            knobImg.raycastTarget = false;

            var stick = baseGo.AddComponent<TouchJoystick>();
            SetField(stick, "_base", baseRt);
            SetField(stick, "_knob", knobRt);
            return stick;
        }

        // ---------- systems ----------

        private static void BuildSystems(Font font, PlayerController player,
            CameraRig rig, MobileHud hud, TouchJoystick joystick)
        {
            var go = new GameObject("Systems");
            var world = go.AddComponent<WorldState>();
            var saves = go.AddComponent<SaveManager>();
            var interactions = go.AddComponent<InteractionManager>();
            var bootstrap = go.AddComponent<GameBootstrap>();

            TextAsset items = AssetDatabase.LoadAssetAtPath<TextAsset>(ItemsPath);
            TextAsset nodes = AssetDatabase.LoadAssetAtPath<TextAsset>(NodesPath);
            if (items == null || nodes == null)
                throw new System.Exception("Missing GameData JSON. Expected:\n"
                    + ItemsPath + "\n" + NodesPath);

            SetField(bootstrap, "_itemsJson", items);
            SetField(bootstrap, "_nodesJson", nodes);
            SetField(bootstrap, "_world", world);
            SetField(bootstrap, "_saves", saves);
            SetField(bootstrap, "_hud", hud);
            SetField(bootstrap, "_player", player);
            SetField(bootstrap, "_cameraRig", rig);
            SetField(bootstrap, "_interactions", interactions);
            SetField(bootstrap, "_joystick", joystick);
            SetField(bootstrap, "_touchInput",
                player.GetComponent<TouchPlayerInput>());

            SetField(player, "_cameraYaw", rig.transform);
            SetField(player, "_inputProviderBehaviour",
                player.GetComponent<TouchPlayerInput>());
            interactions.Bind(player.transform, hud);
        }

        private static void SetField(object target, string name, object value)
        {
            var type = target.GetType();
            var field = type.GetField(name,
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (field == null)
                field = type.BaseType?.GetField(name,
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);
            field.SetValue(target, value);
        }
    }
}
