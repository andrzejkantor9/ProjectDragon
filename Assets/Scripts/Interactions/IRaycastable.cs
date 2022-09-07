using UnityEngine;

namespace RPG.Interactions
{
    public interface IRaycastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(GameObject player);
    }
}