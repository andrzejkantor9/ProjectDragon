using System.Collections.Generic;

using UnityEngine;

using GameDevTV.Inventories;

using RPG.Stats;

namespace RPG.Inventories
{
    public class StatsEquipment : Equipment, IModifierProvider
    {
        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach(var slot in GetAllPopulatedSlots())
            {
                IModifierProvider item = GetItemInSlot(slot) as IModifierProvider;
                if(item == null)
                    continue;

                foreach(var modifier in item.GetAdditiveModifiers(stat))
                    yield return modifier;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach(var slot in GetAllPopulatedSlots())
            {
                IModifierProvider item = GetItemInSlot(slot) as IModifierProvider;
                if(item == null)
                    continue;

                foreach(var modifier in item.GetPercentageModifiers(stat))
                    yield return modifier;
            }
        }
    }
}
