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
        [SerializeField] private bool isMonologue;
        [SerializeField] [TextArea(3, 10)] private string monologueText;

        private const float CooldownSeconds = 30f;
        private float cooldownEndTime = -1f;

        public string CharacterName => characterName;
        public DialogueBranch[] Branches => branches;
        public bool IsMonologue => isMonologue;
        public string MonologueText => monologueText;

        /// <summary>Whether this specific NPC is currently on cooldown.</summary>
        public bool IsOnCooldown => Time.unscaledTime < cooldownEndTime;

        /// <summary>Seconds remaining on this NPC's cooldown (0 if not on cooldown).</summary>
        public float CooldownRemaining => Mathf.Max(0f, cooldownEndTime - Time.unscaledTime);

        /// <summary>Start a 30-second cooldown for this NPC.</summary>
        public void StartCooldown()
        {
            cooldownEndTime = Time.unscaledTime + CooldownSeconds;
        }

        public string InteractionPrompt => isMonologue
            ? $"Examine {characterName} [E]"
            : canTalk
                ? $"Talk to {characterName} [E]"
                : $"Examine {characterName} [E]";

        /// <summary>
        /// Always allow interaction — the cooldown check happens inside DialogueManager.Begin().
        /// </summary>
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

        /// <summary>
        /// Configure this NPC as a monologue (e.g. a deceased character).
        /// No branching, no cooldown — player reads a single text then exits.
        /// </summary>
        public void SetMonologue(string text)
        {
            isMonologue = true;
            monologueText = text;
            canTalk = false;
            branches = null;
        }
    }
}
