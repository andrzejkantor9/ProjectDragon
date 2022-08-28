using UnityEngine;

namespace RPG.Core
{
    public class GameManager
    {
        public static GameObject PlayerGameObject => GameObject.FindWithTag(Enums.EnumToString<Tags>(Tags.Player));

        public static bool HasPlayerTag(GameObject other) => other.CompareTag(Enums.EnumToString<Tags>(Tags.Player));
    }
}