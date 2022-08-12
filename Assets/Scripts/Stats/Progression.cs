using System;
using System.Collections.Generic;

using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/NewProgression")]
    public class Progression : ScriptableObject
    {
        #region Parameters
        [SerializeField]
        private ProgressionCharacterClass[] _characterClases;
        private Dictionary<CharacterClass, Dictionary<Stat, float[]>> _statByLevel = null;
        
        #endregion

        #region Structures
        [Serializable]
        private class ProgressionCharacterClass
        {
            [field: SerializeField]
            public CharacterClass CharacterClass {get; private set;}
            [field: SerializeField]
            public ProgressionStat[] Stats {get; private set;}
        }
        [Serializable]
        class ProgressionStat
        {
            [field: SerializeField]
            public Stat Stat {get; private set;}
            [field: SerializeField]
            public float[] Levels {get; private set;}
        }
        //     [field: SerializeField]
        //     public CharacterClass _characterClass {get; private set;}
        //     [SerializeField]
        //     private float[] _hitPointsByLevel;

        //     ///////////////////////////////////////////////////

        //     public float GetHitPointsAtLevel(int level) => _hitPointsByLevel[level -1];
        // }
        #endregion

        ////////////////////////////////////////////////////////////////

        #region PublicMethods
        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            CheckBuildLookup();
            Logger.Log($"look for stat {Enums.EnumToString<Stat>(stat)}, of class {characterClass.ToString()}, at level {level}", LogFrequency.EveryFrame);

            if(_statByLevel.ContainsKey(characterClass) && _statByLevel[characterClass].ContainsKey(stat))
            {
                float[] levels = _statByLevel[characterClass][stat];

                if(levels.Length >= level)
                    return levels[level-1];
                    
            }

            Debug.LogError("could not get stat with given parameters");
            return 0f;
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            CheckBuildLookup();

            float[] levels = _statByLevel[characterClass][stat];
            return levels.Length;
        }

        private void CheckBuildLookup()
        {
            if(_statByLevel != null)
                return;

            _statByLevel = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            foreach (ProgressionCharacterClass progressionCharacterClass in _characterClases)
            {
                var statLookupTable = new Dictionary<Stat, float[]>();
                
                foreach (ProgressionStat progressionStat in progressionCharacterClass.Stats)
                {
                    statLookupTable[progressionStat.Stat] =  progressionStat.Levels;
                }

                _statByLevel[progressionCharacterClass.CharacterClass] = statLookupTable;
            }
        }
        #endregion
    }
}