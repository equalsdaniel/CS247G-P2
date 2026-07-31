using UnityEngine;
using UnityEngine.InputSystem;

namespace MurderVilla.Interaction
{
    public sealed class FirstPersonInteractor : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField, Min(0.5f)] private float range = 3f;
        [SerializeField] private LayerMask interactionLayers = ~0;

        private IInteractable _current;

        public string CurrentPrompt =>
            _current != null && _current.CanInteract ? _current.InteractionPrompt : string.Empty;

        private void Update()
        {
            FindTarget();
            if (_current != null && _current.CanInteract &&
                Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                _current.Interact();
            }
        }

        private void FindTarget()
        {
            _current = null;
            if (viewCamera == null)
                return;

            Ray ray = new(viewCamera.transform.position, viewCamera.transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, range, interactionLayers,
                    QueryTriggerInteraction.Collide))
                return;

            foreach (MonoBehaviour behaviour in hit.collider.GetComponentsInParent<MonoBehaviour>())
            {
                if (behaviour is IInteractable interactable && interactable.CanInteract)
                {
                    _current = interactable;
                    return;
                }
            }
        }

        public void Configure(Camera camera)
        {
            viewCamera = camera;
        }
    }
}
