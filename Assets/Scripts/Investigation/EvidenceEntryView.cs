using TMPro;
using UnityEngine;

namespace Investigation
{
    /// <summary>Visual for a single row in the investigation log.</summary>
    public class EvidenceEntryView : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text suspectTagText;
        [SerializeField] private GameObject contradictionFlag;

        public void Populate(EvidenceEntry entry, bool hasContradiction)
        {
            titleText.text = entry.title;
            descriptionText.text = entry.description;

            if (suspectTagText != null)
            {
                suspectTagText.gameObject.SetActive(entry.relatedSuspect != SuspectId.None);
                suspectTagText.text = SuspectNames.DisplayName(entry.relatedSuspect);
            }

            if (contradictionFlag != null)
                contradictionFlag.SetActive(hasContradiction);
        }
    }
}
