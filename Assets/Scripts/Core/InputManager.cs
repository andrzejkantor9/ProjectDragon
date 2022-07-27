using UnityEngine;

using UnityEngine.InputSystem;

namespace RPG.Core
{
    public static class InputManager
    {
        public static bool WasPointerPressedThisFrame()
        {
            return Mouse.current.leftButton.wasPressedThisFrame;
        }

        public static bool IsPointerPressed()
        {
            return Mouse.current.leftButton.isPressed;
        }

        public static Vector2 GetPointerPosition()
        {
            return Mouse.current.position.ReadValue();
        }
    }
}

