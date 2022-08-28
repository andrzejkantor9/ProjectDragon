using System;

using UnityEngine;

using GameDevTV.Inventories;

using RPG.Stats;
using System.Collections.Generic;

namespace RPG.Inventories
{
    [CreateAssetMenu(fileName = "StatsEquipableItem", menuName = "StatsItem/StatsEquipableItem", order = 0)]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField]
        Modifier[] _additiveModifiers;
        [SerializeField]
        Modifier[] _percentageModifers;

        [Serializable]
        struct Modifier
        {
            public Stat _stat;
            public float _value;
        }

        ///////////////////////////////////////////////////////////////////////////

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach(var modifier in _additiveModifiers)
            {
                if(modifier._stat == stat)
                    yield return modifier._value;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach(var modifier in _percentageModifers)
            {
                if(modifier._stat == stat)
                    yield return modifier._value;
            }
        }
    }
}