using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MurderVilla.Dialogue
{
    public sealed class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }
        public static bool IsDialogueOpen => Instance != null && Instance.isOpen;

        private Canvas canvas;
        private Text speakerLabel;
        private Text dialogueLabel;
        private Text continueLabel;
        private NPCDialogue currentSpeaker;
        private string[] currentLines = Array.Empty<string>();
        private int lineIndex;
        private float acceptInputAfter;
        private bool isOpen;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (Instance != null)
                return;

            GameObject manager = new(nameof(DialogueManager));
            DontDestroyOnLoad(manager);
            manager.AddComponent<DialogueManager>();
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            BuildUi();
        }

        private void Update()
        {
            if (!isOpen || Time.unscaledTime < acceptInputAfter)
                return;

            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;

            if (keyboard != null && keyboard.qKey.wasPressedThisFrame)
            {
                Close();
                return;
            }

            bool advance = keyboard != null &&
                (keyboard.eKey.wasPressedThisFrame || keyboard.spaceKey.wasPressedThisFrame);
            advance |= mouse != null && mouse.leftButton.wasPressedThisFrame;
            if (advance)
                Advance();
        }

        public void Begin(NPCDialogue speaker, string displayName, string[] lines)
        {
            if (speaker == null || lines == null || lines.Length == 0)
                return;

            currentSpeaker = speaker;
            currentLines = lines;
            lineIndex = 0;
            isOpen = true;
            acceptInputAfter = Time.unscaledTime + 0.18f;

            speakerLabel.text = displayName.ToUpperInvariant();
            dialogueLabel.text = currentLines[0];
            continueLabel.text = currentLines.Length > 1
                ? "E / SPACE / CLICK  Continue     Q  Leave"
                : "E / SPACE / CLICK  Finish     Q  Leave";
            canvas.gameObject.SetActive(true);

            currentSpeaker.SetTalking(true);
            SetPlayerInteraction(true);
            SetCursor(false);
        }

        public void Close()
        {
            if (!isOpen)
                return;

            isOpen = false;
            canvas.gameObject.SetActive(false);
            currentSpeaker?.SetTalking(false);
            currentSpeaker = null;
            currentLines = Array.Empty<string>();
            SetPlayerInteraction(false);
            SetCursor(true);
        }

        private void Advance()
        {
            lineIndex++;
            if (lineIndex >= currentLines.Length)
            {
                Close();
                return;
            }

            dialogueLabel.text = currentLines[lineIndex];
            continueLabel.text = lineIndex == currentLines.Length - 1
                ? "E / SPACE / CLICK  Finish     Q  Leave"
                : "E / SPACE / CLICK  Continue     Q  Leave";
            acceptInputAfter = Time.unscaledTime + 0.08f;
        }

        private void BuildUi()
        {
            GameObject canvasObject = new("Dialogue UI");
            canvasObject.transform.SetParent(transform, false);
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 500;
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject shade = UiObject("Shade", canvasObject.transform);
            Image shadeImage = shade.AddComponent<Image>();
            shadeImage.color = new Color(0f, 0f, 0f, 0.35f);
            Stretch(shade.GetComponent<RectTransform>());

            GameObject panel = UiObject("Dialogue Panel", canvasObject.transform);
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.035f, 0.045f, 0.055f, 0.96f);
            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.08f, 0.06f);
            panelRect.anchorMax = new Vector2(0.92f, 0.34f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;

            speakerLabel = CreateText(panel.transform, "Speaker", 32, FontStyle.Bold,
                TextAnchor.UpperLeft, new Vector2(42f, -28f), new Vector2(-42f, -76f));
            speakerLabel.color = new Color(0.78f, 0.18f, 0.13f);

            dialogueLabel = CreateText(panel.transform, "Dialogue", 27, FontStyle.Normal,
                TextAnchor.UpperLeft, new Vector2(42f, -86f), new Vector2(-42f, -62f));
            dialogueLabel.horizontalOverflow = HorizontalWrapMode.Wrap;
            dialogueLabel.verticalOverflow = VerticalWrapMode.Truncate;

            continueLabel = CreateText(panel.transform, "Continue", 17, FontStyle.Normal,
                TextAnchor.LowerRight, new Vector2(42f, 20f), new Vector2(-42f, 54f));
            continueLabel.color = new Color(1f, 1f, 1f, 0.62f);
            canvasObject.SetActive(false);
        }

        private static GameObject UiObject(string name, Transform parent)
        {
            GameObject target = new(name, typeof(RectTransform));
            target.transform.SetParent(parent, false);
            return target;
        }

        private static Text CreateText(Transform parent, string name, int size,
            FontStyle style, TextAnchor alignment, Vector2 offsetMin, Vector2 offsetMax)
        {
            GameObject target = UiObject(name, parent);
            Text text = target.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = Color.white;
            text.raycastTarget = false;

            RectTransform rect = target.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return text;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void SetCursor(bool captured)
        {
            Cursor.lockState = captured ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !captured;
        }

        private static void SetPlayerInteraction(bool interacting)
        {
            foreach (MonoBehaviour behaviour in
                     FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                Type type = behaviour.GetType();
                if (type.FullName != "FpsHorrorKit.FpsController")
                    continue;

                FieldInfo field = type.GetField("isInteracting");
                field?.SetValue(behaviour, interacting);
                break;
            }
        }
    }
}
