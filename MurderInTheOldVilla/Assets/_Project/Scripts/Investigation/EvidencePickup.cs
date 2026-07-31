using Investigation;
using MurderVilla.Interaction;
using UnityEngine;

namespace MurderVilla.InvestigationSystem
{
    public sealed class EvidencePickup : MonoBehaviour, IInteractable
    {
        [SerializeField] private EvidenceDefinition evidence;
        [SerializeField] private bool hideAfterCollection = true;

        public string InteractionPrompt =>
            evidence == null ? "[E] Inspect" : $"[E] Collect: {evidence.title}";

        public bool CanInteract =>
            evidence != null && EvidenceLog.Instance != null &&
            !EvidenceLog.Instance.HasCollected(evidence.id);

        public void Interact()
        {
            if (!CanInteract)
                return;

            EvidenceLog.Instance.Collect(evidence.CreateEntry());
            Debug.Log($"Evidence collected: {evidence.title}");

            if (hideAfterCollection)
                gameObject.SetActive(false);
        }

        public void Configure(EvidenceDefinition definition)
        {
            evidence = definition;
        }
    }
}
