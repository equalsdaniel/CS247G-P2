using Investigation;
using UnityEngine;
using UnityEngine.UI;

namespace MurderVilla.InvestigationSystem
{
    public sealed class EvidenceCounterUI : MonoBehaviour
    {
        [SerializeField] private Text label;
        [SerializeField, Min(1)] private int totalEvidence = 4;

        private void Update()
        {
            if (label == null)
                return;

            int collected = EvidenceLog.Instance == null
                ? 0
                : EvidenceLog.Instance.Collected.Count;
            label.text = $"Evidence: {collected} / {totalEvidence}";
        }

        public void Configure(Text target, int total)
        {
            label = target;
            totalEvidence = total;
        }
    }
}
