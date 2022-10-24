using System.Collections.Generic;
using System.Diagnostics;

using UnityEngine;

namespace RPG.Debug
{
    public enum LogFrequency
    {
        EveryFrame,
        MostFrames,
        Regular,
        Rare,
        Sporadic
    }

    public static class CustomLogger
    {
        private static Dictionary<LogFrequency, bool> _LogFrequencySettings = new Dictionary<LogFrequency, bool>()
        {
            {LogFrequency.EveryFrame, false},
            {LogFrequency.MostFrames, false},
            {LogFrequency.Regular, true},
            {LogFrequency.Rare, true},
            {LogFrequency.Sporadic, true},
        };

        [Conditional("UNITY_DEVELOPMENT"), Conditional("UNITY_EDITOR")]
        public static void Log(string message, LogFrequency logFrequency)
        {
#if UNITY_DEVELOPMENT || UNITY_EDITOR
            if(_LogFrequencySettings[logFrequency])
                UnityEngine.Debug.Log(message);
#endif
        }
    }
}
