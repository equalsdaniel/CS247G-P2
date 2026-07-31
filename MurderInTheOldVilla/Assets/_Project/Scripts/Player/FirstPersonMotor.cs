using UnityEngine;
using UnityEngine.InputSystem;

namespace MurderVilla.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class FirstPersonMotor : MonoBehaviour
    {
        [SerializeField] private Camera viewCamera;
        [SerializeField, Min(0.1f)] private float moveSpeed = 4f;
        [SerializeField, Min(1f)] private float lookSensitivity = 18f;
        [SerializeField] private float gravity = -20f;
        [SerializeField, Range(45f, 89f)] private float verticalLookLimit = 85f;

        private CharacterController _controller;
        private float _verticalVelocity;
        private float _pitch;
        private bool _cursorCaptured = true;

        public Camera ViewCamera => viewCamera;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (viewCamera == null)
                viewCamera = GetComponentInChildren<Camera>();
        }

        private void Start()
        {
            SetCursorCaptured(true);
        }

        private void Update()
        {
            if (Keyboard.current == null)
                return;

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
                SetCursorCaptured(!_cursorCaptured);

            Move();
            if (_cursorCaptured)
                Look();
        }

        private void Move()
        {
            Vector2 input = Vector2.zero;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            input = Vector2.ClampMagnitude(input, 1f);

            Vector3 horizontal = transform.right * input.x + transform.forward * input.y;
            if (_controller.isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;
            _verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = horizontal * moveSpeed + Vector3.up * _verticalVelocity;
            _controller.Move(velocity * Time.deltaTime);
        }

        private void Look()
        {
            if (Mouse.current == null || viewCamera == null)
                return;

            Vector2 delta = Mouse.current.delta.ReadValue() *
                            (lookSensitivity * 0.01f);
            transform.Rotate(Vector3.up, delta.x);
            _pitch = Mathf.Clamp(_pitch - delta.y, -verticalLookLimit, verticalLookLimit);
            viewCamera.transform.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void SetCursorCaptured(bool captured)
        {
            _cursorCaptured = captured;
            Cursor.lockState = captured ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !captured;
        }

        public void Configure(Camera camera)
        {
            viewCamera = camera;
        }
    }
}
