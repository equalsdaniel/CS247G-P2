using System.Reflection;
using MurderVilla.Dialogue;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MurderInTheOldVilla.Player
{
    /// <summary>
    /// Applies project-owned comfort settings to the imported FPS controller.
    /// This stays outside the ignored Asset Store package so re-importing it
    /// does not remove the settings.
    /// </summary>
    public sealed class PlayerComfortSettings : MonoBehaviour
    {
        private const float LookSensitivity = 2.5f;
        private const string ControllerTypeName = "FpsHorrorKit.FpsController";
        private const string InputsTypeName = "FpsHorrorKit.FpsAssetsInputs";

        private Component fpsInputs;
        private FieldInfo lookField;
        private FieldInfo cursorLockedField;
        private bool cursorCaptured = true;
        private float nextControllerSearchTime;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var settingsObject = new GameObject(nameof(PlayerComfortSettings));
            DontDestroyOnLoad(settingsObject);
            settingsObject.AddComponent<PlayerComfortSettings>();
        }

        private void Start()
        {
            SetCursorCaptured(true);
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (DialogueManager.IsDialogueOpen)
                {
                    DialogueManager.Instance.Close();
                    return;
                }

                SetCursorCaptured(!cursorCaptured);
            }

            if (Time.unscaledTime >= nextControllerSearchTime)
            {
                FindAndConfigureController();
                nextControllerSearchTime = Time.unscaledTime + 0.5f;
            }

            if (!cursorCaptured)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                lookField?.SetValue(fpsInputs, Vector2.zero);
            }
        }

        private void FindAndConfigureController()
        {
            foreach (MonoBehaviour behaviour in FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None))
            {
                string typeName = behaviour.GetType().FullName;

                if (typeName == ControllerTypeName)
                {
                    FieldInfo sensitivityField = behaviour.GetType().GetField("rotationSpeed");
                    sensitivityField?.SetValue(behaviour, LookSensitivity);
                }
                else if (typeName == InputsTypeName)
                {
                    fpsInputs = behaviour;
                    lookField = behaviour.GetType().GetField("look");
                    cursorLockedField = behaviour.GetType().GetField("cursorLocked");
                    cursorLockedField?.SetValue(fpsInputs, cursorCaptured);
                }
            }
        }

        private void SetCursorCaptured(bool captured)
        {
            cursorCaptured = captured;
            Cursor.lockState = captured ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !captured;
            cursorLockedField?.SetValue(fpsInputs, captured);

            if (!captured)
            {
                lookField?.SetValue(fpsInputs, Vector2.zero);
            }
        }
    }
}
