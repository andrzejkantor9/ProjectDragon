using System;

using UnityEngine;

using GameDevTV.Utils;

using RPG.Core;
using RPG.Stats;

namespace RPG.Attributes
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BaseStats))]
    public class HitPoints : MonoBehaviour, ISaveable
    {
        #region Cache
        [HideInInspector]
        private Animator _animator;

        private int _deathAnimId;
        #endregion

        #region States
        public bool IsDead {get; private set;} = false;
        float _hitPointsPercentage = 100f;
        public LazyValue<float> HitPointsValue {get; private set;}
        #endregion

        #region Events
        public event Action OnDeath;
        #endregion

        /////////////////////////////////////////////////////////

        #region EngineMethods
        private void OnValidate()
        {
            _animator = GetComponent<Animator>();    
        }

        private void Awake()
        {
            _deathAnimId = Animator.StringToHash("Death");

            HitPointsValue = new LazyValue<float>(GetInitialHitPoints);
        }

        private void Start()
        {
            HitPointsValue.ForceInit();            
        }

        private void OnEnable()
        {
            if(CompareTag(Enums.EnumToString<Tags>(Tags.Player)))
                GetComponent<BaseStats>().OnLevelUp += LevelUp;
        }

        private void OnDisable()
        {   
            if(CompareTag(Enums.EnumToString<Tags>(Tags.Player)))
                GetComponent<BaseStats>().OnLevelUp -= LevelUp;
        }

        private void Update()
        {
            _hitPointsPercentage =  100 * (HitPointsValue.value / GetComponent<BaseStats>().GetStat(Stat.HitPoints));
        }
        #endregion

        #region PublicMethods
        public void TakeDamage(GameObject instigator, float damage)
        {
            HitPointsValue.value = Mathf.Max(HitPointsValue.value - damage, 0);
            Logger.Log($"health of {gameObject.name}: {HitPointsValue.ToString()}, taken damage: {damage}", LogFrequency.Regular);

            CheckDeath(instigator);
        }

        public float GetPercentage()
        {
            Logger.Log($"percentage for {gameObject.name}: {_hitPointsPercentage}", LogFrequency.EveryFrame);

            return _hitPointsPercentage;
        }

        public float GetMaxHitPoints() => GetComponent<BaseStats>().GetStat(Stat.HitPoints);
        #endregion

        #region Interfaces
        public object CaptureState()
        {
            return HitPointsValue;
        }

        public void RestoreState(object state)
        {
            HitPointsValue.value = (float)state;
            CheckDeath(null);
        }
        #endregion

        #region PrivateMethods
        private bool CheckDeath(GameObject instigator)
        {
            if (!IsDead && HitPointsValue.value == 0)
            {
                _animator.SetTrigger(_deathAnimId);
                IsDead = true;
                if(instigator && instigator.GetComponent<Experience>())
                    instigator.GetComponent<Experience>().GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));

                GetComponent<ActionScheduler>().CancelCurrentAction();
                OnDeath?.Invoke();
            }

            return IsDead;
        }

        private void LevelUp()
        {
            HitPointsValue.value = GetComponent<BaseStats>().GetStat(Stat.HitPoints) * (_hitPointsPercentage / 100f);
        }

        private float GetInitialHitPoints() => GetComponent<BaseStats>().GetStat(Stat.HitPoints);
        #endregion
    }
}