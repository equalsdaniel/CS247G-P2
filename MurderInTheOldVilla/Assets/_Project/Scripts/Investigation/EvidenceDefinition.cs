using Investigation;
using UnityEngine;

namespace MurderVilla.InvestigationSystem
{
    [CreateAssetMenu(menuName = "Murder Villa/Evidence", fileName = "Evidence")]
    public sealed class EvidenceDefinition : ScriptableObject
    {
        public string id;
        public string title;
        [TextArea(2, 6)] public string description;
        public SuspectId relatedSuspect;
        public string contradictsEvidenceId;

        public EvidenceEntry CreateEntry()
        {
            return new EvidenceEntry(id, title, description, relatedSuspect,
                contradictsEvidenceId);
        }
    }
}
