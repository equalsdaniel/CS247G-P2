using MurderVilla.Interaction;
using UnityEngine;

namespace MurderVilla.Dialogue
{
    public sealed class NPCDialogue : MonoBehaviour, IInteractable
    {
        [SerializeField] private string characterName;
        [SerializeField] private DialogueBranch[] branches;
        [SerializeField] private bool canTalk = true;
        [SerializeField] private NPCIdleMotion idleMotion;

        public string CharacterName => characterName;
        public DialogueBranch[] Branches => branches;

        public string InteractionPrompt => canTalk
            ? $"Talk to {characterName} [E]"
            : $"Examine {characterName} [E]";

        public bool CanInteract => !DialogueManager.IsDialogueOpen;

        public void Interact()
        {
            if (DialogueManager.Instance == null) return;

            DialogueManager.Instance.Begin(this);
        }

        public void SetTalking(bool talking)
        {
            if (idleMotion != null)
                idleMotion.SetTalking(talking);
        }

        /// <summary>
        /// Called by CharacterSceneBuilder to populate dialogue data at edit time.
        /// </summary>
        public void Configure(string displayName, DialogueBranch[] dialogueBranches,
            NPCIdleMotion motion, bool dialogueEnabled = true)
        {
            characterName = displayName;
            branches = dialogueBranches;
            idleMotion = motion;
            canTalk = dialogueEnabled;
        }
    }
}
