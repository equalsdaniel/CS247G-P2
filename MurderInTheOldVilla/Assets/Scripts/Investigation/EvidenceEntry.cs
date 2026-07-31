using System;

namespace Investigation
{
    [Serializable]
    public class EvidenceEntry
    {
        public string id;
        public string title;
        [UnityEngine.TextArea(2, 6)]
        public string description;
        public SuspectId relatedSuspect = SuspectId.None;

        /// <summary>Id of another EvidenceEntry this one contradicts, if any.</summary>
        public string contradictsEntryId;

        public EvidenceEntry(string id, string title, string description,
            SuspectId relatedSuspect = SuspectId.None, string contradictsEntryId = null)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.relatedSuspect = relatedSuspect;
            this.contradictsEntryId = contradictsEntryId;
        }
    }
}
