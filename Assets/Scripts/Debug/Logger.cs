using System.Collections.Generic;

using UnityEngine;

namespace RPG
{
    public enum LogFrequency
    {
        EveryFrame,
        MostFrames,
        Regular,
        Rare,
        Sporadic
    }

    public static class Logger
    {
        private static Dictionary<LogFrequency, bool> _LogFrequencySettings = new Dictionary<LogFrequency, bool>()
        {
            {LogFrequency.EveryFrame, false},
            {LogFrequency.MostFrames, false},
            {LogFrequency.Regular, true},
            {LogFrequency.Rare, true},
            {LogFrequency.Sporadic, true},
        };

#if UNITY_DEVELOPMENT || UNITY_EDITOR
        public static void Log(string message, LogFrequency logFrequency)
        {
            if(_LogFrequencySettings[logFrequency])
                Debug.Log(message);
        }
#endif
    }
}
