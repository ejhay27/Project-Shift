using UnityEngine;

namespace ProjectShift.Game
{
    /// <summary>
    /// Input abstraction so PlayerController works with touch, keyboard, or
    /// (later) the Input System without changes. M1: Touch + Keyboard.
    /// </summary>
    public interface IPlayerInput
    {
        /// <summary>Move vector, x = strafe, y = forward, magnitude 0..1.</summary>
        Vector2 Move { get; }
        bool Sprint { get; }
        /// <summary>Edge-triggered jump; consumes the press.</summary>
        bool ConsumeJumpPressed();
    }
}
