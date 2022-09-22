using System;
using System.Collections.Generic;

using UnityEngine;

using RPG.Core;
using RPG.Saving;

using GameDevTV.Utils;

namespace RPG.Stats
{
    public class TraitStore : MonoBehaviour, IModifierProvider, ISaveable, IPredicateEvaluator
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        TraitBonus[] _bonusConfig;
        #endregion

        #region Cache
        //[Header("CACHE")]
    	//[Space(8f)]
        #endregion

        #region States
        Dictionary<Trait, int> _stagedPoints = new Dictionary<Trait, int>();
        Dictionary<Trait, int> _assignedPoints = new Dictionary<Trait, int>();
        
        Dictionary<Stat, Dictionary<Trait, float>> _additiveBonusCache;
        Dictionary<Stat, Dictionary<Trait, float>> _percentageBonusCache;

        BaseStats _playerBaseStats;
        #endregion

        #region Events & Statics
        public event Action onUpdateUI;
        #endregion

        #region Data
        [System.Serializable]
        class TraitBonus
        {
            public Trait TraitType;
            public Stat StatType;
            public float AdditiveBonusPerPoint = 0;
            public float PercentageBonusPerPoint = 0;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods & Contructors
        private void Awake()
        {
            _playerBaseStats = GameManager.PlayerGameObject().GetComponent<BaseStats>();
            _playerBaseStats.onLevelUp += UpdateUI;

            SetupTrainBonusDictionaries();
        }
        private void OnDestroy() 
        {
            _playerBaseStats.onLevelUp -= UpdateUI;
        }
        #endregion

        #region PublicMethods
        public int GetPoints(Trait trait) 
            => _assignedPoints.ContainsKey(trait) ? _assignedPoints[trait] : 0;
        public int GetStagedPoints(Trait trait) 
            => _stagedPoints.ContainsKey(trait) ? _stagedPoints[trait] : 0;
        public int GetProposedPoints(Trait trait)
            => GetPoints(trait) + GetStagedPoints(trait);

        public bool AssignPoints(Trait trait, int points)
        {
            if(!CanAssignPoints(trait, points))
                return false;

            _stagedPoints[trait] = GetStagedPoints(trait) + points;

            UpdateUI();
            return true;
        }

        public bool CanAssignPoints(Trait trait, int points)
            => !(GetStagedPoints(trait) + points < 0 || GetUnassignedPoints() < points);

        public void FinalizeAssigment()
        {
            foreach(Trait trait in _stagedPoints.Keys)
            {
                _assignedPoints[trait] = GetProposedPoints(trait);
            }
            _stagedPoints.Clear();

            UpdateUI();
        }

        public int GetUnassignedPoints()
            => GetAssignablePoints() - GetTotalProposedPoints();

        public int GetTotalProposedPoints()
        {
            int total = 0;
            foreach(int points in _assignedPoints.Values)
            {
                total += points;
            }
            foreach(int points in _stagedPoints.Values)
            {
                total += points;
            }

            return total;
        }

        public int GetAssignablePoints()
            => (int) GetComponent<BaseStats>().GetStat(Stat.TotaTraitPoints);
        #endregion

        #region Interfaces & Inheritance
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if(!_additiveBonusCache.ContainsKey(stat))
                yield break;

            foreach(Trait trait in _additiveBonusCache[stat].Keys)
            {
                float bonus = _additiveBonusCache[stat][trait];
                yield return bonus * GetPoints(trait);
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if(!_percentageBonusCache.ContainsKey(stat))
                yield break;

            foreach(Trait trait in _percentageBonusCache[stat].Keys)
            {
                float bonus = _percentageBonusCache[stat][trait];
                yield return bonus * GetPoints(trait);
            }
        }

        public object CaptureState()
        {
            return _assignedPoints;
        }

        public void RestoreState(object state)
        {
            _assignedPoints = new Dictionary<Trait, int>((Dictionary<Trait, int>)state);
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            if(predicate == "MinimumTrait")
            {
                if(Enum.TryParse<Trait>(parameters[0], out Trait trait))
                {
                    return GetPoints(trait) >= Int32.Parse(parameters[1]);
                }
            }

            return null;
        }
        #endregion

        #region Events & Statics
        #endregion

        #region PrivateMethods
        private void UpdateUI()
        {
            if(onUpdateUI != null)
                onUpdateUI();
        }

        private void SetupTrainBonusDictionaries()
        {
            _additiveBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();
            _percentageBonusCache = new Dictionary<Stat, Dictionary<Trait, float>>();

            foreach (TraitBonus bonus in _bonusConfig)
            {
                if (!_additiveBonusCache.ContainsKey(bonus.StatType))
                {
                    _additiveBonusCache[bonus.StatType] = new Dictionary<Trait, float>();
                }
                if (!_percentageBonusCache.ContainsKey(bonus.StatType))
                {
                    _percentageBonusCache[bonus.StatType] = new Dictionary<Trait, float>();
                }

                _additiveBonusCache[bonus.StatType][bonus.TraitType] = bonus.AdditiveBonusPerPoint;
                _percentageBonusCache[bonus.StatType][bonus.TraitType] = bonus.PercentageBonusPerPoint;
            }
        }
        #endregion
    }
}
