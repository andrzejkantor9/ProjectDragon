using System;

using UnityEngine;

using GameDevTV.SavingAssetPack;

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