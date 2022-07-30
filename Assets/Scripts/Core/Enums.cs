using System;

namespace RPG.Core
{
    public static class Enums
    {
        public enum Tags
        {
            Untagged,
            Respawn,
            Finish,
            EditorOnly,
            MainCamera,
            Player,
            GameController
        }

        ////////////////////////////////////////////////////////////////////////////////

        public static string EnumToString<T>(object value)
        {
            return Enum.GetName(typeof(T), value);
        }
    }
}
