using UnityEngine;

namespace MurderVilla.Dialogue
{
    public sealed class NPCIdleMotion : MonoBehaviour
    {
        [SerializeField] private Transform visualRoot;
        [SerializeField] private bool animate = true;
        [SerializeField, Range(0.2f, 2f)] private float breathingSpeed = 0.75f;
        [SerializeField, Range(0f, 4f)] private float swayDegrees = 1.2f;

        private Vector3 baseScale;
        private Quaternion baseRotation;
        private float phase;
        private bool talking;

        private void Awake()
        {
            if (visualRoot == null)
                visualRoot = transform;

            baseScale = visualRoot.localScale;
            baseRotation = visualRoot.localRotation;
            phase = Mathf.Abs(name.GetHashCode() * 0.173f) % 6.28f;
        }

        private void Update()
        {
            if (!animate || visualRoot == null)
                return;

            float time = Time.time * breathingSpeed + phase;
            float breath = Mathf.Sin(time * 2f) * 0.004f;
            visualRoot.localScale = new Vector3(baseScale.x - breath * 0.25f,
                baseScale.y + breath, baseScale.z - breath * 0.25f);

            float sway = Mathf.Sin(time * 0.72f) * swayDegrees;
            visualRoot.localRotation = baseRotation * Quaternion.Euler(0f, sway, 0f);

            if (talking && Camera.main != null)
            {
                Vector3 direction = Camera.main.transform.position - transform.position;
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.01f)
                {
                    Quaternion target = Quaternion.LookRotation(direction.normalized);
                    transform.rotation = Quaternion.Slerp(transform.rotation, target,
                        Time.deltaTime * 2.8f);
                }
            }
        }

        public void Configure(Transform root, bool shouldAnimate)
        {
            visualRoot = root;
            animate = shouldAnimate;
            if (visualRoot != null)
            {
                baseScale = visualRoot.localScale;
                baseRotation = visualRoot.localRotation;
            }
        }

        public void SetTalking(bool value)
        {
            talking = value;
        }
    }
}
