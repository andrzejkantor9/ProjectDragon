using System;

namespace RPG
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

    public enum WeaponNames
    {
        Unarmed,
        Fireball,
        Sword,
        RedBow,
        BlueBow
    }

    public static class Enums
    {
        ////////////////////////////////////////////////////////////////////////////////

        public static string EnumToString<T>(object value)
        {
            return Enum.GetName(typeof(T), value);
        }
    }
}
