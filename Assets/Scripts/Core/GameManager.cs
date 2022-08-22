using UnityEngine;

namespace RPG.Core
{
    class GameManager
    {
        public static GameObject PlayerGameObject => GameObject.FindWithTag(Enums.EnumToString<Tags>(Tags.Player));
    }
}