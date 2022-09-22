using UnityEngine;

namespace RPG.Core
{
    public class GameManager
    {
        static GameObject s_playerGameObject;

        //////////////////////////////////////////////////////

        public static GameObject PlayerGameObject()
        {
            GameObject foundPlayerObject = GameObject.FindWithTag(Enums.EnumToString<Tags>(Tags.Player));
            if(foundPlayerObject)
            {
                s_playerGameObject = foundPlayerObject;
            }

            return s_playerGameObject;
        }

        public static bool HasPlayerTag(GameObject other) => other.CompareTag(Enums.EnumToString<Tags>(Tags.Player));
    }
}