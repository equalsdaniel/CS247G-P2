using MurderVilla.Interaction;
using UnityEngine;

namespace MurderVilla.Dialogue
{
    public sealed class NPCDialogue : MonoBehaviour, IInteractable
    {
        [SerializeField] private string characterName;
        [SerializeField, TextArea(2, 5)] private string[] lines;
        [SerializeField] private bool canTalk = true;
        [SerializeField] private NPCIdleMotion idleMotion;

        public string InteractionPrompt => canTalk
            ? $"Talk to {characterName} [E]"
            : $"Examine {characterName} [E]";

        public bool CanInteract => !DialogueManager.IsDialogueOpen;

        public void Interact()
        {
            if (DialogueManager.Instance == null)
                return;

            DialogueManager.Instance.Begin(this, characterName, lines);
        }

        public void SetTalking(bool talking)
        {
            if (idleMotion != null)
                idleMotion.SetTalking(talking);
        }

        public void Configure(string displayName, string[] dialogueLines,
            NPCIdleMotion motion, bool dialogueEnabled = true)
        {
            characterName = displayName;
            lines = dialogueLines;
            idleMotion = motion;
            canTalk = dialogueEnabled;
        }
    }
}
