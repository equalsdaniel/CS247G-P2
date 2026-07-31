using System.Collections;
using UnityEngine;

namespace MurderVilla.Interaction
{
    public sealed class SimpleDoor : MonoBehaviour, IInteractable
    {
        [SerializeField] private Transform doorPanel;
        [SerializeField] private float openAngle = 95f;
        [SerializeField, Min(0.05f)] private float duration = 0.5f;

        private bool _isOpen;
        private bool _isMoving;
        private Quaternion _closedRotation;

        public string InteractionPrompt => _isOpen ? "[E] Close door" : "[E] Open door";
        public bool CanInteract => !_isMoving;

        private void Awake()
        {
            if (doorPanel == null)
                doorPanel = transform;
            _closedRotation = doorPanel.localRotation;
        }

        public void Interact()
        {
            if (!_isMoving)
                StartCoroutine(AnimateDoor(!_isOpen));
        }

        private IEnumerator AnimateDoor(bool opening)
        {
            _isMoving = true;
            Quaternion start = doorPanel.localRotation;
            Quaternion end = _closedRotation * Quaternion.Euler(0f, opening ? openAngle : 0f, 0f);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                doorPanel.localRotation = Quaternion.Slerp(start, end, t);
                yield return null;
            }

            doorPanel.localRotation = end;
            _isOpen = opening;
            _isMoving = false;
        }

        public void Configure(Transform panel)
        {
            doorPanel = panel;
        }
    }
}
