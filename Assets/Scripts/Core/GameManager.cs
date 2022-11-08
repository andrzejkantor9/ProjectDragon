using System;

using UnityEngine;

namespace RPG.Core
{
    public class GameManager
    {
        static GameObject s_playerGameObject;

        static Action<bool> _onPlayerControllerSetEnabled;

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

        public static void InjectPlayerControllerSetEnabledCallback(Action<bool> setEnabledCallback)
        {
            _onPlayerControllerSetEnabled = setEnabledCallback;
        }

        public static void PlayerControllerSetEnabled(bool enabled)
        {
            _onPlayerControllerSetEnabled?.Invoke(enabled);
        }

        public static bool HasPlayerTag(GameObject other) => other.CompareTag(Enums.EnumToString<Tags>(Tags.Player));
    }
}