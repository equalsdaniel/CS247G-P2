using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Investigation
{
    /// <summary>
    /// Holds evidence the player has collected so far this playthrough.
    /// Persists across scene loads so the log survives moving between rooms.
    /// </summary>
    public class EvidenceLog : MonoBehaviour
    {
        public static EvidenceLog Instance { get; private set; }

        [Tooltip("Fires whenever a new entry is collected, passing that entry.")]
        public UnityEvent<EvidenceEntry> onEvidenceCollected;

        private readonly List<EvidenceEntry> _collected = new();
        private readonly HashSet<string> _collectedIds = new();

        public IReadOnlyList<EvidenceEntry> Collected => _collected;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool HasCollected(string entryId) => _collectedIds.Contains(entryId);

        public void Collect(EvidenceEntry entry)
        {
            if (entry == null || HasCollected(entry.id)) return;

            _collected.Add(entry);
            _collectedIds.Add(entry.id);
            onEvidenceCollected?.Invoke(entry);
        }

        /// <summary>True if the given entry's flagged contradiction has also been collected.</summary>
        public bool HasContradiction(EvidenceEntry entry)
        {
            return !string.IsNullOrEmpty(entry.contradictsEntryId) && HasCollected(entry.contradictsEntryId);
        }
    }
}
