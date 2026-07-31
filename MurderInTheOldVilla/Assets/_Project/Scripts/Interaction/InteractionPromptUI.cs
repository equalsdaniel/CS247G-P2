using UnityEngine;
using UnityEngine.UI;

namespace MurderVilla.Interaction
{
    public sealed class InteractionPromptUI : MonoBehaviour
    {
        [SerializeField] private FirstPersonInteractor interactor;
        [SerializeField] private Text promptText;

        private void Update()
        {
            if (promptText == null || interactor == null)
                return;

            string prompt = interactor.CurrentPrompt;
            promptText.text = prompt;
            promptText.enabled = !string.IsNullOrWhiteSpace(prompt);
        }

        public void Configure(FirstPersonInteractor source, Text label)
        {
            interactor = source;
            promptText = label;
        }
    }
}
