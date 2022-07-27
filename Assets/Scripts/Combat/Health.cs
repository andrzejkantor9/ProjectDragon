using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Animator))]
    public class Health : MonoBehaviour
    {
        #region Parameters
        [SerializeField]
        private float _hitPoints = 100f;
        #endregion

        #region Cache
        [HideInInspector]
        private Animator _animator;

        private int _deathAnimId;
        #endregion

        #region States
        public bool IsDead {get; private set;} = false;
        #endregion

        /////////////////////////////////////////////////////////

        #region EngineFunctionality
        private void OnValidate()
        {
            _animator = GetComponent<Animator>();    
        }

        private void Awake()
        {
            _deathAnimId = Animator.StringToHash("Death");
        }
        #endregion

        #region PublicFunctionality
        public void TakeDamage(float damage)
        {
            _hitPoints = Mathf.Max(_hitPoints - damage, 0);
            Logger.Log($"health of {gameObject.name}: {_hitPoints.ToString()}");

            CheckDeath();
        }
        #endregion

        #region PrivateFuncionality
        private bool CheckDeath()
        {
            if (!IsDead && _hitPoints == 0)
            {
                _animator.SetTrigger(_deathAnimId);
                IsDead = true;
            }

            return IsDead;
        }
        #endregion
    }
}