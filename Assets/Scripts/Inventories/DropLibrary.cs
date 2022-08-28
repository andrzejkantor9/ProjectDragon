using System.Collections.Generic;

using UnityEngine;

using GameDevTV.Inventories;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = "Inventory/DropLibrary", order = 0)]
    public class DropLibrary : ScriptableObject
    { 
        #region Config
        [Header("CONFIG")]
        [SerializeField][Range(0f, 1f)]
        private float[] _dropChancePercentage;
        [SerializeField]
        private int[] _minDrops;
        [SerializeField]
        private int[] _maxDrops;
        [SerializeField]
        private DropConfig[] _potentialDrops;
        #endregion

        #region Data
        [System.Serializable]
        class DropConfig
        {
            public InventoryItem _item;
            public float[] _relativeChance;
            public int[] _minNumber;
            public int[] _maxNumber;

            public int GetRandomNumber(int level)
            {
                if(!_item.IsStackable())
                    return 1;

                int min = GetByLevel(_minNumber, level);
                int max = GetByLevel(_maxNumber, level);
                return Random.Range(min, max + 1);
            }
        }

        public struct Dropped
        {
            public InventoryItem _item;
            public int _number;
        }
        #endregion

        //////////////////////////////////////////////////////

        #region StaticMethods
        private static T GetByLevel<T>(T[] values, int level)
        {
            if(values.Length == 0 || level <= 0)
                return default;
            else if(level > values.Length)
                return values[values.Length -1];
            
            return values[level-1];
        }
        #endregion

        #region PublicMethods
        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            if(!ShouldRandomDrop(level))
                yield break;

            for(int i = 0; i < GetRandomNumberOfDrops(level); ++i)
                yield return GetRandomDrop(level);
        }
        #endregion

        #region PrivateMethods
        private bool ShouldRandomDrop(int level)
        {
            return Random.Range(0f, 1f) < GetByLevel(_dropChancePercentage, level);
        }

        private int GetRandomNumberOfDrops(int level)
        {
            int minInclusive = GetByLevel(_minDrops, level);
            int maxExclusive = GetByLevel(_maxDrops, level);
            return Random.Range(minInclusive, maxExclusive);
        }

        private Dropped GetRandomDrop(int level)
        {
            Dropped dropped = new Dropped();
            DropConfig dropConfig = SelectRandomItem(level);

            dropped._item = dropConfig._item;
            dropped._number = dropConfig.GetRandomNumber(level);

            return dropped;
        }

        private DropConfig SelectRandomItem(int level)
        {
            float totalChance = GetTotalChance(level);
            float randomRoll = Random.Range(0, totalChance);
            float chanceTotal = 0f;

            foreach (var drop in _potentialDrops)
            {
                chanceTotal += GetByLevel(drop._relativeChance, level);
                if(chanceTotal > randomRoll)
                    return drop;
            }
            
            return null;
        }

        private float GetTotalChance(int level)
        {
            float total = 0f;
            foreach(var drop in _potentialDrops)
                total += GetByLevel(drop._relativeChance, level);

            return total;
        }
        #endregion
    }
}