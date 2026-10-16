using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>
    /// Editor/PC input provider using default Input Manager axes
    /// (Horizontal/Vertical/Jump). Used for desktop testing of M1.
    /// </summary>
    public sealed class KeyboardPlayerInput : MonoBehaviour, IPlayerInput
    {
        public Vector2 Move
        {
            get
            {
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");
                Vector2 v = new Vector2(x, y);
                return v.sqrMagnitude > 1f ? v.normalized : v;
            }
        }

        public bool Sprint => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        public bool ConsumeJumpPressed() => Input.GetButtonDown("Jump");
    }
}
