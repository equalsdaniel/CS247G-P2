#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using MurderVilla.Dialogue;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MurderVilla.Editor
{
    [InitializeOnLoad]
    public static class CharacterSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/VillaHorrorPrototype.unity";
        private const string CharacterRootName = "Story Characters ABCDEF";
        private const string OutfitRoot =
            "Assets/_Project/ThirdParty/Quaternius/UltimateModularCharacters";
        private const string AmyModel = OutfitRoot + "/Women/Amy_Formal.fbx";
        private const string BenModel = OutfitRoot + "/Men/Ben_Casual.fbx";
        private const string CocoModel = OutfitRoot + "/Women/Coco_Suit.fbx";
        private const string DeanFelixModel = OutfitRoot + "/Men/Dean_Felix_Suit.fbx";
        private const string EllaModel = OutfitRoot + "/Women/Ella_Worker.fbx";
        private const string AnimationModel =
            "Assets/_Project/ThirdParty/Quaternius/UniversalAnimationLibrary/Animations/UAL1_Standard.fbx";
        private const string ControllerPath =
            "Assets/_Project/Art/Animations/NPCIdle.controller";
        private const string PreferredIdle = "Armature|Idle_Loop";

        private static readonly string[] CharacterModels =
        {
            AmyModel, BenModel, CocoModel, DeanFelixModel, EllaModel,
        };

        private static bool buildQueued;

        static CharacterSceneBuilder()
        {
            QueueBuild();
        }

        [MenuItem("Murder in Old Villa/Add Story Characters (ABCDEF)")]
        public static void BuildFromMenu()
        {
            BuildCharacters(true);
        }

        [MenuItem("Murder in Old Villa/Validate Story Characters")]
        public static void ValidateFromMenu()
        {
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            string[] names = { "Amy", "Ben", "Coco", "Dean", "Ella", "Felix" };
            foreach (string characterName in names)
            {
                GameObject character = FindNamed("NPC " + characterName);
                if (character == null)
                    throw new InvalidOperationException($"Missing NPC {characterName}.");

                Renderer[] renderers = character.GetComponentsInChildren<Renderer>(true);
                if (renderers.Length == 0)
                    throw new InvalidOperationException($"NPC {characterName} has no renderer.");

                Bounds bounds = renderers[0].bounds;
                foreach (Renderer renderer in renderers.Skip(1))
                    bounds.Encapsulate(renderer.bounds);
                Animator animator = character.GetComponentInChildren<Animator>();
                string animationState = animator != null && animator.runtimeAnimatorController != null
                    ? "animated"
                    : "static";
                Debug.Log($"CAST CHECK {characterName}: position={character.transform.position}, " +
                    $"size={bounds.size}, {animationState}, nameTags=" +
                    character.GetComponentsInChildren<WorldNameTag>(true).Length);
            }
        }

        private static void QueueBuild()
        {
            if (buildQueued)
                return;

            buildQueued = true;
            EditorApplication.delayCall += TryBuildAfterImport;
        }

        private static void TryBuildAfterImport()
        {
            buildQueued = false;
            if (EditorApplication.isPlayingOrWillChangePlaymode ||
                EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                QueueBuild();
                return;
            }

            BuildCharacters(false);
        }

        private static void BuildCharacters(bool forceRebuild)
        {
            if (!File.Exists(ScenePath) || CharacterModels.Any(path => !File.Exists(path)))
                return;

            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject existing = FindNamed(CharacterRootName);
            if (existing != null && !forceRebuild)
                return;

            if (existing != null)
                UnityEngine.Object.DestroyImmediate(existing);

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            foreach (string modelPath in CharacterModels)
                ConfigureHumanoidImporter(modelPath);
            ConfigureHumanoidImporter(AnimationModel);

            RuntimeAnimatorController controller = CreateIdleController();
            GameObject amyModel = AssetDatabase.LoadAssetAtPath<GameObject>(AmyModel);
            GameObject benModel = AssetDatabase.LoadAssetAtPath<GameObject>(BenModel);
            GameObject cocoModel = AssetDatabase.LoadAssetAtPath<GameObject>(CocoModel);
            GameObject deanFelixModel = AssetDatabase.LoadAssetAtPath<GameObject>(DeanFelixModel);
            GameObject ellaModel = AssetDatabase.LoadAssetAtPath<GameObject>(EllaModel);
            if (CharacterModels.Any(path => AssetDatabase.LoadAssetAtPath<GameObject>(path) == null))
            {
                Debug.LogWarning("Character FBX files are still importing. Try the character menu again.");
                return;
            }

            GameObject root = new(CharacterRootName);
            Transform player = FindNamed("Player")?.transform;
            Vector3 playerPosition = player != null ? player.position : new Vector3(-9f, 1f, -10f);
            Vector3 forward = player != null
                ? Vector3.ProjectOnPlane(player.forward, Vector3.up).normalized
                : Vector3.forward;
            if (forward.sqrMagnitude < 0.1f)
                forward = Vector3.forward;
            Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

            // Keep the complete cast in a visible test formation near the spawn.
            // Once the dialogue flow is approved these positions can be distributed
            // through the villa without touching the character prefabs or dialogue.
            Vector3 amyPosition = PlaceOnFloor(playerPosition + forward * 3.8f - right * 3f);
            Vector3 benPosition = PlaceOnFloor(playerPosition + forward * 5.1f - right * 1.6f);
            Vector3 cocoPosition = PlaceOnFloor(playerPosition + forward * 5.8f);
            Vector3 deanPosition = PlaceOnFloor(playerPosition + forward * 5.1f + right * 1.6f);
            Vector3 ellaPosition = PlaceOnFloor(playerPosition + forward * 3.8f + right * 3f);
            Vector3 felixPosition = PlaceOnFloor(playerPosition + forward * 7.6f + right * 3.2f);

            CreateCharacter(root.transform, "Amy", amyModel, amyPosition,
                0.98f, new Color(0.46f, 0.18f, 0.20f), controller, true,
                new[]
                {
                    "Felix and I argued sometimes, but I stayed downstairs after 10:00.",
                    "I passed the master bedroom around 9:50. I never entered it.",
                    "And I never touched the milk at all.",
                });

            CreateCharacter(root.transform, "Ben", benModel, benPosition,
                1.02f, new Color(0.16f, 0.26f, 0.38f), controller, true,
                new[]
                {
                    "At 10:10 I passed Felix's bedroom. The bedside lamp was off.",
                    "I could not see anyone, but I heard a newspaper rustling continuously.",
                    "I assumed Uncle Felix was still awake and went downstairs.",
                });

            CreateCharacter(root.transform, "Coco", cocoModel, cocoPosition,
                1.01f, new Color(0.24f, 0.15f, 0.34f), controller, true,
                new[]
                {
                    "I was with Dean in the living room from 10:00 until 10:30.",
                    "I never went upstairs. Dean can confirm that.",
                    "I had no reason to hurt Felix.",
                });

            CreateCharacter(root.transform, "Dean", deanFelixModel, deanPosition,
                1.04f, new Color(0.15f, 0.28f, 0.22f), controller, true,
                new[]
                {
                    "I stayed with Coco all night.",
                    "At 10:30 I patrolled upstairs. Felix's room was quiet.",
                    "I assumed he had fallen asleep.",
                });

            CreateCharacter(root.transform, "Ella", ellaModel, ellaPosition,
                0.96f, new Color(0.42f, 0.36f, 0.22f), controller, true,
                new[]
                {
                    "I finished cleaning the second floor at 9:45 and prepared warm milk.",
                    "I saw someone hurry away near the stairs, but I could not identify them.",
                    "I delivered the milk at 10:00. Felix was reading with the lamp on.",
                    "After that, I remained in the kitchen.",
                });

            GameObject felix = CreateCharacter(root.transform, "Felix", deanFelixModel,
                felixPosition, 1.05f, new Color(0.22f, 0.22f, 0.24f),
                null, false,
                new[]
                {
                    "The curtain-cord marks indicate mechanical strangulation.",
                    "There are no signs that Felix willingly tightened the cord himself.",
                });
            Transform felixVisual = felix.transform.Find("Visual");
            if (felixVisual != null)
            {
                felixVisual.localRotation = Quaternion.Euler(0f, 0f, 90f);
                felixVisual.localPosition = new Vector3(0f, 0.55f, 0f);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log("Added Amy, Ben, Coco, Dean, Ella and Felix with dialogue and idle motion.");
        }

        private static GameObject CreateCharacter(Transform parent, string characterName,
            GameObject bodyPrefab, Vector3 position, float heightScale,
            Color accent, RuntimeAnimatorController controller, bool animate,
            string[] dialogue)
        {
            GameObject character = new($"NPC {characterName}");
            character.transform.SetParent(parent);
            character.transform.position = position;

            GameObject visual = new("Visual");
            visual.transform.SetParent(character.transform, false);
            visual.transform.localScale = Vector3.one * heightScale;

            GameObject body = (GameObject)PrefabUtility.InstantiatePrefab(bodyPrefab, visual.transform);
            body.name = characterName + " Body";
            body.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            ApplyOutfitMaterials(body, characterName, accent);
            ConfigureAnimator(body, controller, animate);

            CapsuleCollider collider = character.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, 0.9f * heightScale, 0f);
            collider.height = 1.8f * heightScale;
            collider.radius = 0.36f;

            NPCIdleMotion motion = character.AddComponent<NPCIdleMotion>();
            motion.Configure(visual.transform, animate);
            NPCDialogue npcDialogue = character.AddComponent<NPCDialogue>();
            npcDialogue.Configure(characterName, dialogue, motion, animate);
            CreateNameTag(character.transform, characterName, heightScale);

            Vector3 faceCenter = Camera.main != null
                ? Camera.main.transform.position
                : character.transform.position + Vector3.forward;
            Vector3 direction = faceCenter - character.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
                character.transform.rotation = Quaternion.LookRotation(direction.normalized);

            return character;
        }

        private static void CreateNameTag(Transform parent, string characterName,
            float heightScale)
        {
            GameObject tag = new("Name Tag", typeof(RectTransform), typeof(Canvas),
                typeof(CanvasScaler), typeof(WorldNameTag));
            tag.transform.SetParent(parent, false);
            tag.transform.localPosition = new Vector3(0f, 2.18f * heightScale, 0f);
            tag.transform.localScale = Vector3.one * 0.0045f;

            RectTransform tagRect = tag.GetComponent<RectTransform>();
            tagRect.sizeDelta = new Vector2(250f, 58f);
            Canvas canvas = tag.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 80;

            GameObject background = new("Background", typeof(RectTransform), typeof(Image));
            background.transform.SetParent(tag.transform, false);
            Stretch(background.GetComponent<RectTransform>());
            background.GetComponent<Image>().color = new Color(0.025f, 0.03f, 0.035f, 0.82f);

            GameObject label = new("Label", typeof(RectTransform), typeof(Text));
            label.transform.SetParent(tag.transform, false);
            RectTransform labelRect = label.GetComponent<RectTransform>();
            Stretch(labelRect);
            labelRect.offsetMin = new Vector2(12f, 4f);
            labelRect.offsetMax = new Vector2(-12f, -4f);
            Text text = label.GetComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = characterName.ToUpperInvariant();
            text.fontSize = 31;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(0.92f, 0.86f, 0.72f);
            text.raycastTarget = false;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void ConfigureAnimator(GameObject model,
            RuntimeAnimatorController controller, bool animate)
        {
            Animator animator = model.GetComponent<Animator>();
            if (animator == null)
                animator = model.AddComponent<Animator>();
            animator.runtimeAnimatorController = animate ? controller : null;
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }

        private static RuntimeAnimatorController CreateIdleController()
        {
            if (!File.Exists(AnimationModel))
                return null;

            EnsureFolder("Assets/_Project/Art/Animations");
            EnsureIdleClipsLoop();
            AnimatorController existing =
                AssetDatabase.LoadAssetAtPath<AnimatorController>(ControllerPath);
            if (existing != null)
            {
                bool valid = existing.layers.Length > 0 &&
                    existing.layers[0].stateMachine.defaultState != null &&
                    existing.layers[0].stateMachine.defaultState.motion != null &&
                    existing.layers[0].stateMachine.defaultState.motion.name == PreferredIdle;
                if (valid)
                    return existing;

                AssetDatabase.DeleteAsset(ControllerPath);
            }

            AnimationClip idle = AssetDatabase.LoadAllAssetsAtPath(AnimationModel)
                .OfType<AnimationClip>()
                .Where(clip => !clip.name.StartsWith("__preview__", StringComparison.Ordinal))
                .OrderBy(clip => clip.name == PreferredIdle ? 0 :
                    clip.name.Contains("Idle_Talking") ? 1 :
                    clip.name.Contains("Idle") ? 2 : 3)
                .FirstOrDefault();
            if (idle == null)
                return null;

            AnimatorController controller =
                AnimatorController.CreateAnimatorControllerAtPathWithClip(ControllerPath, idle);
            controller.layers[0].stateMachine.defaultState.speed = 0.82f;
            return controller;
        }

        private static void EnsureIdleClipsLoop()
        {
            ModelImporter importer = AssetImporter.GetAtPath(AnimationModel) as ModelImporter;
            if (importer == null)
                return;

            ModelImporterClipAnimation[] clips = importer.clipAnimations;
            if (clips.Length == 0)
                clips = importer.defaultClipAnimations;

            bool changed = false;
            foreach (ModelImporterClipAnimation clip in clips)
            {
                if (!clip.name.Contains("Idle") || clip.loopTime)
                    continue;
                clip.loopTime = true;
                clip.loopPose = true;
                changed = true;
            }

            if (!changed)
                return;

            importer.clipAnimations = clips;
            importer.SaveAndReimport();
        }

        private static void ConfigureHumanoidImporter(string assetPath)
        {
            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (importer == null || importer.animationType == ModelImporterAnimationType.Human)
                return;

            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.SaveAndReimport();
        }

        private static void ApplyOutfitMaterials(GameObject model, string characterName,
            Color accent)
        {
            const string materialFolder =
                "Assets/_Project/Art/Materials/OutfitCharacters";
            EnsureFolder(materialFolder);

            foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>(true))
            {
                Material[] source = renderer.sharedMaterials;
                Material[] replacements = new Material[source.Length];
                for (int i = 0; i < source.Length; i++)
                    replacements[i] = OutfitMaterial(characterName, source[i], accent, i);
                renderer.sharedMaterials = replacements;
            }
        }

        private static Material OutfitMaterial(string characterName, Material source,
            Color accent, int index)
        {
            string sourceName = source != null ? source.name : $"Part_{index}";
            string safeName = string.Concat(sourceName.Select(character =>
                char.IsLetterOrDigit(character) ? character : '_'));
            string path =
                $"Assets/_Project/Art/Materials/OutfitCharacters/{characterName}_{safeName}_{index}.mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            Shader shader = Shader.Find("HDRP/Lit") ?? Shader.Find("Standard");
            Material material = new(shader) { name = characterName + " " + sourceName };
            Color color = Color.white;
            if (source != null)
            {
                if (source.HasProperty("_BaseColor"))
                    color = source.GetColor("_BaseColor");
                else if (source.HasProperty("_Color"))
                    color = source.GetColor("_Color");
            }

            string lowerName = sourceName.ToLowerInvariant();
            bool naturalColor = lowerName.Contains("skin") || lowerName.Contains("face") ||
                lowerName.Contains("hair") || lowerName.Contains("eye");
            if (!naturalColor)
                color = Color.Lerp(color, accent, 0.22f);

            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", color);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", 0.24f);

            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Vector3 FindAnchorPosition(string objectName, Vector3 fallback,
            bool useTop)
        {
            GameObject anchor = FindClosestNamed(objectName, fallback);
            if (anchor == null)
                return PlaceOnFloor(fallback);

            Renderer[] renderers = anchor.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
                return PlaceOnFloor(anchor.transform.position);

            Bounds bounds = renderers[0].bounds;
            foreach (Renderer renderer in renderers.Skip(1))
                bounds.Encapsulate(renderer.bounds);

            if (useTop)
                return new Vector3(bounds.center.x, bounds.max.y + 0.05f, bounds.center.z);

            Vector3 edge = bounds.center + Vector3.right * Mathf.Min(1.2f, bounds.extents.x);
            return PlaceOnFloor(edge);
        }

        private static Vector3 PlaceOnFloor(Vector3 point)
        {
            Physics.SyncTransforms();
            Vector3 origin = point + Vector3.up * 7f;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 15f,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                return hit.point + Vector3.up * 0.02f;
            return point;
        }

        private static GameObject FindClosestNamed(string name, Vector3 near)
        {
            return UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .Where(candidate => candidate.name == name)
                .OrderBy(candidate => (candidate.position - near).sqrMagnitude)
                .Select(candidate => candidate.gameObject)
                .FirstOrDefault();
        }

        private static GameObject FindNamed(string name)
        {
            return UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include,
                    FindObjectsSortMode.None)
                .FirstOrDefault(candidate => candidate.name == name)?.gameObject;
        }

        private static void EnsureFolder(string path)
        {
            string[] pieces = path.Split('/');
            string current = pieces[0];
            for (int i = 1; i < pieces.Length; i++)
            {
                string next = current + "/" + pieces[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, pieces[i]);
                current = next;
            }
        }
    }
}
#endif
