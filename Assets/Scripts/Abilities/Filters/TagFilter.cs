using System.Collections.Generic;

using UnityEngine;

namespace RPG.Abilities.Filters
{
    [CreateAssetMenu(fileName = "TagFilter", menuName = "Abilities/Filters/Tag")]
    public class TagFilter : FilterStrategy
    {
        #region Config
        [Header("CONFIG")]
        [SerializeField]
        private Tags _tagToFilter = Tags.Enemy;
        #endregion

        #region Cache
        //[Header("CACHE")]
        //[Space(8f)]
        #endregion

        #region States
        #endregion

        #region Events
        //[Header("EVENTS")]
        //[Space(8f)]
        #endregion

        #region Statics
        #endregion

        #region Data
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////

        #region EngineMethods
        #endregion

        #region PublicMethods
        #endregion

        #region Interfaces & Inheritance
        public override IEnumerable<GameObject> Filter(IEnumerable<GameObject> objectsToFilter)
        {
            foreach(GameObject possibleObject in objectsToFilter)
            {
                if(possibleObject.CompareTag(Enums.EnumToString<Tags>(_tagToFilter)))
                {
                    yield return possibleObject;
                }
            }
        }
        #endregion

        #region Events
        #endregion

        #region StaticMethods
        #endregion

        #region PrivateMethods
        #endregion
    }
}
