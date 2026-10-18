using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>
    /// M1 third-person character movement (UI-002). CharacterController-based,
    /// camera-relative, mobile-friendly. Reads an IPlayerInput provider.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _walkSpeed = 4.5f;
        [SerializeField] private float _sprintMultiplier = 1.6f;
        [SerializeField] private float _turnSpeed = 12f;
        [SerializeField] private float _jumpHeight = 1.2f;
        [SerializeField] private float _gravity = -22f;

        [Header("References")]
        [SerializeField] private Transform _cameraYaw;
        [SerializeField] private MonoBehaviour _inputProviderBehaviour;

        private CharacterController _controller;
        private IPlayerInput _input;
        private Vector3 _verticalVelocity;

        public Vector3 Velocity { get; private set; }
        public bool IsGrounded => _controller != null && _controller.isGrounded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = _inputProviderBehaviour as IPlayerInput
                ?? GetComponent<IPlayerInput>();
            if (_cameraYaw == null && Camera.main != null)
                _cameraYaw = Camera.main.transform;
        }

        public void SetInput(IPlayerInput input) => _input = input;
        public void SetCameraYaw(Transform yaw) => _cameraYaw = yaw;

        private void Update()
        {
            if (_input == null) return;

            Vector2 move = _input.Move;
            float speed = _walkSpeed * (_input.Sprint ? _sprintMultiplier : 1f);

            // Camera-relative planar movement.
            Vector3 forward = _cameraYaw != null ? _cameraYaw.forward : Vector3.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
            forward.Normalize();
            Vector3 right = new Vector3(forward.z, 0f, -forward.x);

            Vector3 planar = (forward * move.y + right * move.x);
            if (planar.sqrMagnitude > 1f) planar.Normalize();
            planar *= speed;

            // Facing.
            if (planar.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(planar.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target,
                    _turnSpeed * Time.deltaTime);
            }

            // Gravity + jump.
            if (_controller.isGrounded && _verticalVelocity.y < 0f)
                _verticalVelocity.y = -2f;
            if (_input.ConsumeJumpPressed() && _controller.isGrounded)
                _verticalVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            _verticalVelocity.y += _gravity * Time.deltaTime;

            Vector3 motion = (planar + _verticalVelocity) * Time.deltaTime;
            _controller.Move(motion);
            Velocity = planar + _verticalVelocity;
        }
    }
}
