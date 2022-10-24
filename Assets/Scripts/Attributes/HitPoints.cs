using System;

using UnityEngine;
using UnityEngine.Events;

using GameDevTV.Utils;

using RPG.Core;
using RPG.Stats;
using RPG.Debug;
using RPG.Saving;

namespace RPG.Attributes
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(BaseStats))]
    public class HitPoints : MonoBehaviour, ISaveable
    {
        #region Cache
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
        public event Action onRespawn;
        [SerializeField]
        private UnityEvent<float> OnTakeDamage;
        [SerializeField]
        private UnityEvent onDeathUnityEvent;
        // [Serializable]
        // public class TakeDamageEvent : UnityEvent<float>{}
        #endregion

        /////////////////////////////////////////////////////////

        #region EngineMethods
        private void Awake()
        {
            _animator = GetComponent<Animator>();

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
                GetComponent<BaseStats>().onLevelUp += LevelUp;
        }

        private void OnDisable()
        {   
            if(CompareTag(Enums.EnumToString<Tags>(Tags.Player)))
                GetComponent<BaseStats>().onLevelUp -= LevelUp;
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
            CustomLogger.Log($"hit points of {gameObject.name}: {HitPointsValue.value.ToString()}"
            + $", taken damage: {damage}, dealt by: {instigator.name}", LogFrequency.MostFrames);

            CheckDeath(instigator);
            OnTakeDamage.Invoke(damage);
        }

        public float GetPercentage()
        {
            CustomLogger.Log($"percentage for {gameObject.name}: {_hitPointsPercentage}", LogFrequency.EveryFrame);

            return _hitPointsPercentage;
        }

        public void Heal(float hitPointsToRestore)
        {
            HitPointsValue.value = Mathf.Min(HitPointsValue.value + hitPointsToRestore, GetMaxHitPoints());
            CheckDeath(null);
        }

        public void Respawn()
        {
            Debug.CustomLogger.Log($"respawn subscribers: {onRespawn.GetInvocationList().Length}", LogFrequency.Regular);
            if(onRespawn != null)
                onRespawn();
        }

        public float GetFraction() => _hitPointsPercentage / 100f;

        public float GetMaxHitPoints() => GetComponent<BaseStats>().GetStat(Stat.HitPoints);
        #endregion

        #region Interfaces
        public object CaptureState()
        {
            return HitPointsValue.value;
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
                if(OnDeath != null)
                    OnDeath();
                if(onDeathUnityEvent != null)
                    onDeathUnityEvent?.Invoke();
            }
            else if(IsDead && HitPointsValue.value > 0)
            {
                _animator.Rebind();
                Respawn();
                IsDead = false;
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