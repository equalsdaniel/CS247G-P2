#if UNITY_EDITOR
using System.IO;
using Investigation;
using MurderVilla.Interaction;
using MurderVilla.InvestigationSystem;
using MurderVilla.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MurderVilla.Editor
{
    [InitializeOnLoad]
    public static class VillaPrototypeBuilder
    {
        private const string Root = "Assets/_Project";
        private const string ScenePath = Root + "/Scenes/VillaPrototype.unity";
        private const string EvidencePath = Root + "/Data/Evidence";
        private const string MaterialPath = Root + "/Art/Materials";

        static VillaPrototypeBuilder()
        {
            EditorApplication.delayCall += BuildOnFirstOpen;
        }

        private static void BuildOnFirstOpen()
        {
            if (Application.isBatchMode || File.Exists(ScenePath))
                return;

            Build();
            EditorSceneManager.OpenScene(ScenePath);
        }

        [MenuItem("Murder in Old Villa/Build Prototype Scene")]
        public static void Build()
        {
            EnsureFolders();
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,
                NewSceneMode.Single);

            Material floor = MaterialAsset("Floor", new Color(0.18f, 0.14f, 0.12f));
            Material wall = MaterialAsset("Wall", new Color(0.42f, 0.38f, 0.32f));
            Material wood = MaterialAsset("Wood", new Color(0.20f, 0.08f, 0.035f));
            Material evidence = MaterialAsset("Evidence", new Color(0.60f, 0.08f, 0.06f));

            CreateEnvironment(floor, wall, wood);
            FirstPersonInteractor interactor = CreatePlayer();
            CreateSystems();
            CreateEvidence(evidence);
            CreateUI(interactor);
            CreateLighting();

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePath, true)
            };
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Murder in the Old Villa prototype created at {ScenePath}");
        }

        private static void EnsureFolders()
        {
            Folder(Root);
            Folder(Root + "/Art");
            Folder(MaterialPath);
            Folder(Root + "/Data");
            Folder(EvidencePath);
            Folder(Root + "/Prefabs");
            Folder(Root + "/Scenes");
        }

        private static void Folder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            string name = Path.GetFileName(path);
            AssetDatabase.CreateFolder(parent, name);
        }

        private static Material MaterialAsset(string name, Color color)
        {
            string path = $"{MaterialPath}/{name}.mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                Shader shader = Shader.Find("HDRP/Lit") ??
                                Shader.Find("Universal Render Pipeline/Lit") ??
                                Shader.Find("Standard");
                material = new Material(shader) { name = name };
                AssetDatabase.CreateAsset(material, path);
            }
            else
            {
                Shader shader = Shader.Find("HDRP/Lit") ??
                                Shader.Find("Universal Render Pipeline/Lit") ??
                                Shader.Find("Standard");
                if (material.shader != shader)
                    material.shader = shader;
            }

            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void CreateEnvironment(Material floor, Material wall, Material wood)
        {
            GameObject root = new("Environment_Graybox");
            Cube("Ground Floor", root.transform, new Vector3(0f, -0.15f, 0f),
                new Vector3(22f, 0.3f, 18f), floor);

            Cube("North Wall", root.transform, new Vector3(0f, 2f, 9f),
                new Vector3(22f, 4f, 0.3f), wall);
            Cube("South Wall", root.transform, new Vector3(0f, 2f, -9f),
                new Vector3(22f, 4f, 0.3f), wall);
            Cube("West Wall", root.transform, new Vector3(-11f, 2f, 0f),
                new Vector3(0.3f, 4f, 18f), wall);
            Cube("East Wall", root.transform, new Vector3(11f, 2f, 0f),
                new Vector3(0.3f, 4f, 18f), wall);

            Cube("Hall Divider A", root.transform, new Vector3(-3f, 2f, 2f),
                new Vector3(0.25f, 4f, 14f), wall);
            Cube("Hall Divider B", root.transform, new Vector3(4f, 2f, -2f),
                new Vector3(0.25f, 4f, 14f), wall);
            Cube("Bedroom Wall", root.transform, new Vector3(7.5f, 2f, 2f),
                new Vector3(7f, 4f, 0.25f), wall);
            Cube("Kitchen Wall", root.transform, new Vector3(-7f, 2f, -2f),
                new Vector3(8f, 4f, 0.25f), wall);

            CreateDoor(root.transform, new Vector3(4f, 1.15f, 5.5f), wood);
            CreateDoor(root.transform, new Vector3(-3f, 1.15f, -5f), wood);

            Cube("Bedroom Bed", root.transform, new Vector3(7.2f, 0.45f, 5.8f),
                new Vector3(3.4f, 0.9f, 5f), wood);
            Cube("Bedroom Side Table", root.transform, new Vector3(9.4f, 0.6f, 6.8f),
                new Vector3(1.2f, 1.2f, 1.2f), wood);
            Cube("Kitchen Counter", root.transform, new Vector3(-7f, 0.6f, -6.8f),
                new Vector3(6f, 1.2f, 1.2f), wood);

            GameObject signs = new("Room Labels");
            signs.transform.SetParent(root.transform);
            Label("MASTER BEDROOM", signs.transform, new Vector3(7.3f, 2.2f, 1.82f));
            Label("KITCHEN", signs.transform, new Vector3(-7f, 2.2f, -1.82f));
        }

        private static void CreateDoor(Transform parent, Vector3 position, Material material)
        {
            GameObject pivot = new("Interactive Door");
            pivot.transform.SetParent(parent);
            pivot.transform.position = position + Vector3.left * 0.75f;

            GameObject panel = Cube("Door Panel", pivot.transform, Vector3.right * 0.75f,
                new Vector3(1.5f, 2.3f, 0.15f), material, true);
            SimpleDoor door = pivot.AddComponent<SimpleDoor>();
            door.Configure(panel.transform);
        }

        private static FirstPersonInteractor CreatePlayer()
        {
            GameObject player = new("Player");
            player.transform.position = new Vector3(0f, 1.1f, -6f);
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.35f;

            GameObject cameraObject = new("View Camera");
            cameraObject.transform.SetParent(player.transform);
            cameraObject.transform.localPosition = new Vector3(0f, 0.65f, 0f);
            Camera camera = cameraObject.AddComponent<Camera>();
            AddHdrpComponentIfAvailable(cameraObject,
                "UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData, Unity.RenderPipelines.HighDefinition.Runtime");
            cameraObject.AddComponent<AudioListener>();
            camera.tag = "MainCamera";

            FirstPersonMotor motor = player.AddComponent<FirstPersonMotor>();
            motor.Configure(camera);
            FirstPersonInteractor interactor = player.AddComponent<FirstPersonInteractor>();
            interactor.Configure(camera);
            return interactor;
        }

        private static void CreateSystems()
        {
            GameObject systems = new("GameSystems");
            systems.AddComponent<EvidenceLog>();
        }

        private static void CreateEvidence(Material material)
        {
            GameObject root = new("Evidence");
            EvidenceObject("Milk Cup", PrimitiveType.Cylinder, root.transform,
                new Vector3(-7f, 1.35f, -6.7f), new Vector3(0.25f, 0.18f, 0.25f),
                material, Definition("milk_cup", "Milk Cup",
                    "Sedative residue remains in the warm milk.", SuspectId.Amy));
            EvidenceObject("Flat Newspaper", PrimitiveType.Cube, root.transform,
                new Vector3(7.2f, 0.93f, 5.8f), new Vector3(0.65f, 0.025f, 0.45f),
                material, Definition("flat_newspaper", "Flat Newspaper",
                    "It lies flat and untouched, contradicting the reported rustling.",
                    SuspectId.Ben));
            EvidenceObject("Curtain Cord", PrimitiveType.Cylinder, root.transform,
                new Vector3(9.8f, 1.1f, 7.9f), new Vector3(0.06f, 0.65f, 0.06f),
                material, Definition("curtain_cord", "Curtain Cord",
                    "The suspected murder weapon. Coco's fingerprints are present.",
                    SuspectId.Coco));
            EvidenceObject("Private USB", PrimitiveType.Cube, root.transform,
                new Vector3(-1.5f, 0.12f, 5f), new Vector3(0.16f, 0.05f, 0.35f),
                material, Definition("wang_usb", "Dean's Private USB",
                    "It may contain the unmodified surveillance recording.",
                    SuspectId.Dean));
        }

        private static EvidenceDefinition Definition(string id, string title,
            string description, SuspectId suspect)
        {
            string path = $"{EvidencePath}/{id}.asset";
            EvidenceDefinition definition =
                AssetDatabase.LoadAssetAtPath<EvidenceDefinition>(path);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<EvidenceDefinition>();
                AssetDatabase.CreateAsset(definition, path);
            }

            definition.id = id;
            definition.title = title;
            definition.description = description;
            definition.relatedSuspect = suspect;
            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static void EvidenceObject(string name, PrimitiveType type,
            Transform parent, Vector3 position, Vector3 scale, Material material,
            EvidenceDefinition definition)
        {
            GameObject item = GameObject.CreatePrimitive(type);
            item.name = name;
            item.transform.SetParent(parent);
            item.transform.position = position;
            item.transform.localScale = scale;
            item.GetComponent<Renderer>().sharedMaterial = material;
            item.AddComponent<EvidencePickup>().Configure(definition);
        }

        private static void CreateUI(FirstPersonInteractor interactor)
        {
            GameObject canvasObject = new("Investigation HUD");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject crosshairObject = new("Crosshair");
            crosshairObject.transform.SetParent(canvasObject.transform, false);
            Text crosshair = crosshairObject.AddComponent<Text>();
            crosshair.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            crosshair.text = "+";
            crosshair.fontSize = 24;
            crosshair.alignment = TextAnchor.MiddleCenter;
            crosshair.color = Color.white;
            RectTransform crosshairRect = crosshair.rectTransform;
            crosshairRect.anchorMin = crosshairRect.anchorMax = new Vector2(0.5f, 0.5f);
            crosshairRect.sizeDelta = new Vector2(40f, 40f);

            GameObject promptObject = new("Interaction Prompt");
            promptObject.transform.SetParent(canvasObject.transform, false);
            Text prompt = promptObject.AddComponent<Text>();
            prompt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            prompt.fontSize = 22;
            prompt.alignment = TextAnchor.MiddleCenter;
            prompt.color = Color.white;
            RectTransform promptRect = prompt.rectTransform;
            promptRect.anchorMin = promptRect.anchorMax = new Vector2(0.5f, 0.35f);
            promptRect.sizeDelta = new Vector2(700f, 60f);
            canvasObject.AddComponent<InteractionPromptUI>().Configure(interactor, prompt);

            GameObject counterObject = new("Evidence Counter");
            counterObject.transform.SetParent(canvasObject.transform, false);
            Text counter = counterObject.AddComponent<Text>();
            counter.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            counter.fontSize = 20;
            counter.alignment = TextAnchor.UpperLeft;
            counter.color = Color.white;
            RectTransform counterRect = counter.rectTransform;
            counterRect.anchorMin = counterRect.anchorMax = new Vector2(0f, 1f);
            counterRect.pivot = new Vector2(0f, 1f);
            counterRect.anchoredPosition = new Vector2(24f, -24f);
            counterRect.sizeDelta = new Vector2(320f, 40f);
            counterObject.AddComponent<EvidenceCounterUI>().Configure(counter, 4);

            GameObject helpObject = new("Controls");
            helpObject.transform.SetParent(canvasObject.transform, false);
            Text help = helpObject.AddComponent<Text>();
            help.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            help.text = "WASD Move  |  Space Jump  |  Mouse Look  |  E Interact  |  Esc Cursor";
            help.fontSize = 16;
            help.alignment = TextAnchor.LowerLeft;
            help.color = new Color(1f, 1f, 1f, 0.75f);
            RectTransform helpRect = help.rectTransform;
            helpRect.anchorMin = helpRect.anchorMax = new Vector2(0f, 0f);
            helpRect.pivot = new Vector2(0f, 0f);
            helpRect.anchoredPosition = new Vector2(24f, 20f);
            helpRect.sizeDelta = new Vector2(650f, 32f);

            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new("EventSystem");
                eventSystem.AddComponent<EventSystem>();
            }
        }

        private static void CreateLighting()
        {
            GameObject sun = new("Moonlight");
            Light directional = sun.AddComponent<Light>();
            directional.type = LightType.Directional;
            directional.intensity = 0.35f;
            directional.color = new Color(0.55f, 0.65f, 1f);
            sun.transform.rotation = Quaternion.Euler(35f, -25f, 0f);

            PointLight("Bedroom Lamp", new Vector3(8.8f, 2.4f, 6.8f),
                new Color(1f, 0.62f, 0.32f), 8f);
            PointLight("Hall Light", new Vector3(0f, 2.8f, 0f),
                new Color(1f, 0.78f, 0.55f), 10f);
            PointLight("Kitchen Light", new Vector3(-7f, 2.8f, -5f),
                new Color(0.85f, 0.90f, 1f), 8f);

            VolumeProfile profile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(
                "Assets/Settings/SkyandFogSettingsProfile.asset");
            if (profile != null)
            {
                GameObject volumeObject = new("Global Environment Volume");
                Volume volume = volumeObject.AddComponent<Volume>();
                volume.isGlobal = true;
                volume.sharedProfile = profile;
            }
        }

        private static void PointLight(string name, Vector3 position, Color color,
            float range)
        {
            GameObject lightObject = new(name);
            lightObject.transform.position = position;
            Light light = lightObject.AddComponent<Light>();
            AddHdrpComponentIfAvailable(lightObject,
                "UnityEngine.Rendering.HighDefinition.HDAdditionalLightData, Unity.RenderPipelines.HighDefinition.Runtime");
            light.type = LightType.Point;
            light.color = color;
            light.intensity = 2f;
            light.range = range;
        }

        private static void AddHdrpComponentIfAvailable(GameObject target, string typeName)
        {
            System.Type type = System.Type.GetType(typeName);
            if (type != null && target.GetComponent(type) == null)
                target.AddComponent(type);
        }

        private static GameObject Cube(string name, Transform parent, Vector3 position,
            Vector3 scale, Material material, bool localPosition = false)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent);
            if (localPosition)
                cube.transform.localPosition = position;
            else
                cube.transform.position = position;
            cube.transform.localScale = scale;
            cube.GetComponent<Renderer>().sharedMaterial = material;
            return cube;
        }

        private static void Label(string text, Transform parent, Vector3 position)
        {
            GameObject labelObject = new(text);
            labelObject.transform.SetParent(parent);
            labelObject.transform.position = position;
            labelObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            TextMesh mesh = labelObject.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.fontSize = 36;
            mesh.characterSize = 0.05f;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.color = new Color(0.8f, 0.72f, 0.55f);
        }
    }
}
#endif
