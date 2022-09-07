using System;

using UnityEngine;

using GameDevTV.Utils;

using RPG.Debug;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        #region Parameters
        [SerializeField][Range(1, 99)]
        private int _startingLevel = 1;
        [SerializeField]
        private CharacterClass _characterClass;
        [SerializeField]
        private Progression _progression = null;
        [SerializeField]
        private bool _shouldUseModifiers;
        #endregion

        #region Cache
        [SerializeField]
        private GameObject _levelUpParticleEffect = null;
        private Experience _experience;
        #endregion

        #region States
        private LazyValue<int> _currentLevel;
        #endregion

        #region Events
        public event Action OnLevelUp;
        #endregion

        /////////////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            _experience = GetComponent<Experience>();
            _currentLevel = new LazyValue<int>(CalculateLevel); 
        }

        private void Start()
        {
            _currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if(_experience != null)
                _experience.onExperienceGained += UpdateLevel;
        }

        private void OnDisable()
        {
            if(_experience != null)
                _experience.onExperienceGained -= UpdateLevel;
        }
        #endregion

        #region PublicMethods
        public float GetStat(Stat stat)
        {
            float statValue = _progression.GetStat(stat, _characterClass, GetLevel()) + GetAdditiveModifier(stat);
            CustomLogger.Log($"Stat {Enums.EnumToString<Stat>(stat)} value: {statValue}", LogFrequency.EveryFrame);

            //all percentage modifiers in below function
            return statValue * GetMultiplicativeModifier(stat);
        }

        public int GetLevel()
        {            
            return _currentLevel.value;
        }
        #endregion

        #region PrivateMethods        
        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > _currentLevel.value)
            {
                _currentLevel.value = newLevel;
                CustomLogger.Log("Leveled up", LogFrequency.Regular);
                LevelUpEffect();
                OnLevelUp();
            }
            // if(gameObject.CompareTag(Enums.EnumToString<Tags>(Tags.Player)))
            //     Logger.Log(GetLevel().ToString(), LogFrequency.EveryFrame);
        }

        private void LevelUpEffect()
        {
            Instantiate(_levelUpParticleEffect, transform);
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if(!_shouldUseModifiers)
                return 0f;

            float modifierSum = 0f;            

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float singleModifier in provider.GetAdditiveModifiers(stat))
                {
                    modifierSum += singleModifier;   
                }
            }

            return modifierSum;
        }

        private float GetMultiplicativeModifier(Stat stat)
        {
            if(!_shouldUseModifiers)
                return 1f;

            float percentageSum = 0f;
                
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float singleModifier in provider.GetPercentageModifiers(stat))
                {
                    percentageSum += singleModifier;   
                }
            }

            return 1f + percentageSum / 100f;
        }

        private int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();
            
            if(!experience)
                return _startingLevel;

            float currentXp = experience.GetPoints;
            int penultimateLevel = _progression.GetLevels(Stat.ExperienceToLevelUp, _characterClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                float XPToLevelUp = _progression.GetStat(Stat.ExperienceToLevelUp, _characterClass, level);
                if(XPToLevelUp > currentXp)
                    return level;
            }
            return penultimateLevel + 1;
        }
        #endregion
    }
}