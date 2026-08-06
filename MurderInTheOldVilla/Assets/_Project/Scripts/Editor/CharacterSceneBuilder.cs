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

namespace MurderVilla.Editor
{
    [InitializeOnLoad]
    public static class CharacterSceneBuilder
    {
        private const string ScenePath = "Assets/_Project/Scenes/VillaHorrorPrototype.unity";
        private const string CharacterRootName = "Story Characters ABCDEF";
        private const string AssetRoot =
            "Assets/_Project/ThirdParty/Quaternius/UniversalBaseCharacters";
        private const string MaleModel = AssetRoot + "/Models/Superhero_Male_FullBody.fbx";
        private const string FemaleModel = AssetRoot + "/Models/Superhero_Female_FullBody.fbx";
        private const string AnimationModel =
            "Assets/_Project/ThirdParty/Quaternius/UniversalAnimationLibrary/Animations/UAL1_Standard.fbx";
        private const string ControllerPath =
            "Assets/_Project/Art/Animations/NPCIdle.controller";
        private const string PreferredIdle = "Armature|Idle_Loop";

        private static readonly string[] HairModels =
        {
            "Hair_Buns.fbx",
            "Hair_SimpleParted.fbx",
            "Hair_Long.fbx",
            "Hair_Buzzed.fbx",
            "Hair_BuzzedFemale.fbx",
            "Hair_Beard.fbx",
        };

        private static bool buildQueued;

        static CharacterSceneBuilder()
        {
            QueueBuild();
        }

        [MenuItem("Murder in Old Villa/Add Story Characters (ABCDEF)")]
        public static void BuildFromMenu()
        {
            Debug.Log("[CharacterBuilder] Menu command received. isPlaying=" +
                EditorApplication.isPlayingOrWillChangePlaymode +
                " isCompiling=" + EditorApplication.isCompiling +
                " isUpdating=" + EditorApplication.isUpdating);
            BuildCharacters(true);
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
            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (!File.Exists(ScenePath) || !File.Exists(MaleModel) ||
                !File.Exists(FemaleModel))
                return;

            Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            GameObject existing = FindNamed(CharacterRootName);
            if (existing != null && !forceRebuild)
                return;

            if (existing != null)
                UnityEngine.Object.DestroyImmediate(existing);

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            RuntimeAnimatorController controller = CreateIdleController();
            GameObject male = AssetDatabase.LoadAssetAtPath<GameObject>(MaleModel);
            GameObject female = AssetDatabase.LoadAssetAtPath<GameObject>(FemaleModel);
            if (male == null || female == null)
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

            Vector3 amyPosition = FindAnchorPosition("Stairs",
                playerPosition + forward * 5f + right * 2.5f, false);
            Vector3 benPosition = FindAnchorPosition("Corridor_2",
                playerPosition + forward * 10f - right * 2f, false);
            Vector3 cocoPosition = PlaceOnFloor(playerPosition + forward * 7f - right * 2.4f);
            Vector3 deanPosition = PlaceOnFloor(playerPosition + forward * 7f + right * 2.4f);
            Vector3 ellaPosition = FindAnchorPosition("Kitchen",
                playerPosition + forward * 12f + right * 3f, false);
            Vector3 felixPosition = FindAnchorPosition("GothicBed",
                playerPosition + forward * 13f, true);

            CreateCharacter(root.transform, "Amy", female, HairModels[0], amyPosition,
                0.98f, new Color(0.46f, 0.18f, 0.20f), controller, true,
                new DialogueBranch[]
                {
                    new DialogueBranch
                    {
                        questionText = "Why aren't you wearing clothes?",
                        responseText = "That's very rude of you to ask.",
                    },
                    new DialogueBranch
                    {
                        questionText = "Where were you around 21:50 that night?",
                        qaSequence = new QAPair[]
                        {
                            new QAPair
                            {
                                question = "Where were you around 21:50 that night?",
                                answer = "I passed by the master bedroom door and went straight to the living room. I didn't stay long.",
                            },
                            new QAPair
                            {
                                question = "What do you mean by \"passed by\"? Did you go inside?",
                                answer = "No, I never went into that room. I didn't go in there once that whole night.",
                            },
                            new QAPair
                            {
                                question = "Okay, so you stayed in the living room after that?",
                                answer = "Yes, I didn't go back upstairs after returning to the living room. I didn't go up to the second floor at all after 22:00 — not once.",
                            },
                            new QAPair
                            {
                                question = "Dean's birthday cake seemed to be cut pretty late. Do you remember what was prepared that night?",
                                answer = "...(pause) I don't know. Anyway, I never touched the milk. I had nothing to do with that cup of milk at all.",
                            },
                            new QAPair
                            {
                                question = "(frowns slightly, doesn't press about the milk) ...Alright. Did you see Coco that night?",
                                answer = "We didn't really cross paths that night. I think she was in the living room talking with Felix the whole time — I wasn't really paying attention.",
                            },
                        },
                    },
                });

            CreateCharacter(root.transform, "Ben", male, HairModels[1], benPosition,
                1.02f, new Color(0.16f, 0.26f, 0.38f), controller, true,
                new DialogueBranch[]
                {
                    new DialogueBranch
                    {
                        questionText = "What did you see that night?",
                        responseText = "At 10:10 I passed Felix's bedroom. The bedside lamp was off. I could not see anyone, but I heard a newspaper rustling continuously. I assumed Uncle Felix was still awake and went downstairs.",
                    },
                });

            CreateCharacter(root.transform, "Coco", female, HairModels[2], cocoPosition,
                1.01f, new Color(0.24f, 0.15f, 0.34f), controller, true,
                new DialogueBranch[]
                {
                    new DialogueBranch
                    {
                        questionText = "Where were you that night?",
                        responseText = "I was with Dean in the living room from 10:00 until 10:30. I never went upstairs. Dean can confirm that. I had no reason to hurt Felix.",
                    },
                });

            CreateCharacter(root.transform, "Dean", male, HairModels[3], deanPosition,
                1.04f, new Color(0.15f, 0.28f, 0.22f), controller, true,
                new DialogueBranch[]
                {
                    new DialogueBranch
                    {
                        questionText = "What happened that night?",
                        responseText = "I stayed with Coco all night. At 10:30 I patrolled upstairs. Felix's room was quiet. I assumed he had fallen asleep.",
                    },
                });

            CreateCharacter(root.transform, "Ella", female, HairModels[4], ellaPosition,
                0.96f, new Color(0.42f, 0.36f, 0.22f), controller, true,
                new DialogueBranch[]
                {
                    new DialogueBranch
                    {
                        questionText = "What did you do that night?",
                        responseText = "I finished cleaning the second floor at 9:45 and prepared warm milk. I saw someone hurry away near the stairs, but I could not identify them. I delivered the milk at 10:00. Felix was reading with the lamp on. After that, I remained in the kitchen.",
                    },
                });

            GameObject felix = CreateCharacter(root.transform, "Felix", male,
                HairModels[5], felixPosition, 1.05f, new Color(0.22f, 0.22f, 0.24f),
                null, false,
                new DialogueBranch[]
                {
                    new DialogueBranch
                    {
                        questionText = "Examine the body",
                        responseText = "The curtain-cord marks indicate mechanical strangulation. There are no signs that Felix willingly tightened the cord himself.",
                    },
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
            GameObject bodyPrefab, string hairFile, Vector3 position, float heightScale,
            Color accent, RuntimeAnimatorController controller, bool animate,
            DialogueBranch[] dialogue)
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
            ApplyCharacterMaterials(body, characterName, accent, false);
            ConfigureAnimator(body, controller, animate);

            string hairPath = AssetRoot + "/Hairstyles/" + hairFile;
            GameObject hairPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(hairPath);
            if (hairPrefab != null)
            {
                GameObject hair = (GameObject)PrefabUtility.InstantiatePrefab(hairPrefab, visual.transform);
                hair.name = characterName + " Hair";
                hair.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                ApplyCharacterMaterials(hair, characterName, accent, true);
                ConfigureAnimator(hair, controller, animate);
            }

            CapsuleCollider collider = character.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, 0.9f * heightScale, 0f);
            collider.height = 1.8f * heightScale;
            collider.radius = 0.36f;

            NPCIdleMotion motion = character.AddComponent<NPCIdleMotion>();
            motion.Configure(visual.transform, animate);
            NPCDialogue npcDialogue = character.AddComponent<NPCDialogue>();
            npcDialogue.Configure(characterName, dialogue, motion, animate);

            Vector3 faceCenter = Camera.main != null
                ? Camera.main.transform.position
                : character.transform.position + Vector3.forward;
            Vector3 direction = faceCenter - character.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
                character.transform.rotation = Quaternion.LookRotation(direction.normalized);

            return character;
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

        private static void ApplyCharacterMaterials(GameObject model, string name,
            Color accent, bool hair)
        {
            EnsureFolder("Assets/_Project/Art/Materials/Characters");
            foreach (Renderer renderer in model.GetComponentsInChildren<Renderer>(true))
            {
                Material[] source = renderer.sharedMaterials;
                Material[] replacements = new Material[source.Length];
                for (int i = 0; i < source.Length; i++)
                {
                    string materialName = source[i] != null ? source[i].name : "Body";
                    string key = hair || materialName.ToLowerInvariant().Contains("hair")
                        ? "Hair"
                        : materialName.ToLowerInvariant().Contains("eye") ? "Eyes" : "Body";
                    replacements[i] = CharacterMaterial(name, key, accent);
                }
                renderer.sharedMaterials = replacements;
            }
        }

        private static Material CharacterMaterial(string characterName, string part,
            Color accent)
        {
            string path = $"Assets/_Project/Art/Materials/Characters/{characterName}_{part}.mat";
            Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            Shader shader = Shader.Find("HDRP/Lit") ?? Shader.Find("Standard");
            Material material = new(shader) { name = characterName + " " + part };
            Color color = part switch
            {
                "Hair" => Color.Lerp(accent, Color.black, 0.58f),
                "Eyes" => new Color(0.22f, 0.12f, 0.06f),
                _ => Color.Lerp(Color.white, accent, 0.12f),
            };
            if (material.HasProperty("_BaseColor"))
                material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color"))
                material.SetColor("_Color", color);
            if (material.HasProperty("_Smoothness"))
                material.SetFloat("_Smoothness", 0.28f);

            Texture2D texture = CharacterTexture(characterName, part);
            if (texture != null)
            {
                if (material.HasProperty("_BaseColorMap"))
                    material.SetTexture("_BaseColorMap", texture);
                if (material.HasProperty("_MainTex"))
                    material.SetTexture("_MainTex", texture);
            }
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Texture2D CharacterTexture(string characterName, string part)
        {
            const string textures = AssetRoot + "/Textures/";
            if (part == "Eyes")
                return AssetDatabase.LoadAssetAtPath<Texture2D>(textures + "T_Eye_Brown.png");
            if (part == "Hair")
            {
                string hairTexture = characterName is "Amy" or "Dean" or "Felix"
                    ? "T_Hair_1_BaseColor.png"
                    : "T_Hair_2_BaseColor.png";
                return AssetDatabase.LoadAssetAtPath<Texture2D>(textures + hairTexture);
            }

            bool female = characterName is "Amy" or "Coco" or "Ella";
            bool dark = characterName is "Coco" or "Dean";
            string bodyTexture = female
                ? dark ? "T_Superhero_Female_Dark_BaseColor.png"
                    : "T_Superhero_Female_Light_BaseColor.png"
                : dark ? "T_Superhero_Male_Dark.png"
                    : "T_Superhero_Male_Ligh.png";
            return AssetDatabase.LoadAssetAtPath<Texture2D>(textures + bodyTexture);
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
            return UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include)
                .Where(candidate => candidate.name == name)
                .OrderBy(candidate => (candidate.position - near).sqrMagnitude)
                .Select(candidate => candidate.gameObject)
                .FirstOrDefault();
        }

        private static GameObject FindNamed(string name)
        {
            return UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsInactive.Include)
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
