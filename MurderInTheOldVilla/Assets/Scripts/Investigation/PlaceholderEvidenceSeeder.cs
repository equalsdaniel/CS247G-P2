using UnityEngine;

namespace Investigation
{
    /// <summary>
    /// Temporary: drops a few evidence entries from the villa case into the
    /// log on start, so the investigation-log UI has real content to test
    /// against before pickups/triggers exist in a level. Delete once evidence
    /// is actually granted by in-world interaction.
    /// </summary>
    public class PlaceholderEvidenceSeeder : MonoBehaviour
    {
        private void Start()
        {
            var log = EvidenceLog.Instance;
            if (log == null) return;

            log.Collect(new EvidenceEntry(
                "villa_curtain_cord",
                "Curtain Cord",
                "The murder weapon. Only Su's fingerprints are on it.",
                SuspectId.Su));

            log.Collect(new EvidenceEntry(
                "villa_milk_cup",
                "Milk Cup",
                "Sedative residue found inside. The maid delivered it without any chance to poison it herself — the drug was already in it.",
                SuspectId.Mei));

            log.Collect(new EvidenceEntry(
                "villa_liny_statement",
                "Lin-Y's Statement",
                "\"I never touched the milk at all.\" Nobody asked her about the milk.",
                SuspectId.LinY,
                contradictsEntryId: "villa_milk_cup"));

        }
    }
}
