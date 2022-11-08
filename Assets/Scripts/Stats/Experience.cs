using System;

using UnityEngine;

using RPG.Saving;

using RPG.Core;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        #region States
        [SerializeField]
        private float _experiencePoints;
        #endregion

        #region Events
        public event Action onExperienceGained;
        #endregion

        ///////////////////////////////////////

        #region EngineMethods
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void Update()
        {
            if(InputManager.IsDebugAddExperiencePressed())
            {
                GainExperience(Time.deltaTime * 1000f);
            }
        }
#endif
        #endregion

        #region PublicMethods
        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
            onExperienceGained();
        }
        public float GetPoints => _experiencePoints;
        public int GetLevel => GetComponent<BaseStats>().GetLevel();
        #endregion

        #region Interfaces
        public object CaptureState()
        {
            return _experiencePoints;
        }

        public void RestoreState(object state)
        {
            _experiencePoints = (float)state;
        }
        #endregion
    }
}