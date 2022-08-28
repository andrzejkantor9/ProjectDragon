using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

using GameDevTV.Inventories;

using RPG.Stats;

namespace RPG.Inventories
{
    public class RandomDropper : ItemDropper 
    {
        #region Statics
        const int PLACE_ATTEMPTS = 30;
        #endregion

        #region Config
        [Header("CONFIG")]
        [SerializeField][Range(0f, 100f)]
        private float _scatterDistance = 4f;
        [SerializeField]
        private float _minScatterDistance = 2f;
        [SerializeField]
        private DropLibrary _dropLibrary;
        #endregion

        ///////////////////////////////////////////////////////////////////////////

        #region PublicMethods
        public void RandomDrop()
        {
            BaseStats baseStats = GetComponent<BaseStats>();

            IEnumerable<DropLibrary.Dropped> drops = _dropLibrary.GetRandomDrops(baseStats.GetLevel());
            foreach (var drop in drops)
                DropItem(drop._item, drop._number);
        }
        #endregion

        #region OverrideMethods
        protected override Vector3 GetDropLocation()
        {
            for(int i =0; i < PLACE_ATTEMPTS; ++i)
            {    
                Vector3 randomPoint = base.GetDropLocation() + Random.insideUnitSphere * _scatterDistance;
                NavMeshHit hit;

                if(NavMesh.SamplePosition(randomPoint, out hit, .1f, NavMesh.AllAreas) && Vector3.Distance(transform.position, hit.position) > _minScatterDistance)
                    return hit.position;
            }

            return base.GetDropLocation();
        }
        #endregion
    }
}
