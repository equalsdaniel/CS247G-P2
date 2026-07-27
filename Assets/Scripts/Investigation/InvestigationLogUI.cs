using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Investigation
{
    /// <summary>
    /// Toggleable full-screen panel listing collected evidence. Rebuilds its
    /// list whenever new evidence comes in or the panel is opened.
    /// </summary>
    public class InvestigationLogUI : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform entryListParent;
        [SerializeField] private EvidenceEntryView entryViewPrefab;
        [SerializeField] private Key toggleKey = Key.Tab;

        private bool _isOpen;

        private void OnEnable()
        {
            if (EvidenceLog.Instance != null)
                EvidenceLog.Instance.onEvidenceCollected.AddListener(OnEvidenceCollected);
        }

        private void OnDisable()
        {
            if (EvidenceLog.Instance != null)
                EvidenceLog.Instance.onEvidenceCollected.RemoveListener(OnEvidenceCollected);
        }

        private void Start()
        {
            SetOpen(false);
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
            {
                SetOpen(!_isOpen);
            }
        }

        private void OnEvidenceCollected(EvidenceEntry _)
        {
            if (_isOpen) Rebuild();
        }

        public void SetOpen(bool open)
        {
            _isOpen = open;
            panelRoot.SetActive(open);
            Time.timeScale = open ? 0f : 1f;

            if (open) Rebuild();
        }

        private void Rebuild()
        {
            foreach (Transform child in entryListParent)
                Destroy(child.gameObject);

            if (EvidenceLog.Instance == null) return;

            foreach (var entry in EvidenceLog.Instance.Collected)
            {
                var view = Instantiate(entryViewPrefab, entryListParent);
                bool contradiction = EvidenceLog.Instance.HasContradiction(entry);
                view.Populate(entry, contradiction);
            }
        }
    }
}
