using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>M1 touch input provider: joystick + HUD buttons (UI-002).</summary>
    public sealed class TouchPlayerInput : MonoBehaviour, IPlayerInput
    {
        [SerializeField] private TouchJoystick _joystick;
        [SerializeField] private bool _sprintToggle;

        private bool _jumpPressed;

        public Vector2 Move => _joystick != null ? _joystick.Output : Vector2.zero;
        public bool Sprint => _sprintToggle;

        public bool ConsumeJumpPressed()
        {
            bool v = _jumpPressed;
            _jumpPressed = false;
            return v;
        }

        // Wired to HUD buttons (Jump / Sprint toggle).
        public void OnJumpButton() => _jumpPressed = true;
        public void SetSprint(bool sprint) => _sprintToggle = sprint;
        public void SetJoystick(TouchJoystick joystick) => _joystick = joystick;
    }
}
